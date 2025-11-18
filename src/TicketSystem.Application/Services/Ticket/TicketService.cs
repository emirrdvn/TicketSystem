using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs.Request;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Application.Services.Ticket;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, Guid customerId)
    {
        // Generate ticket number
        var ticketCount = await _context.Tickets.CountAsync();
        var ticketNumber = $"#{(ticketCount + 1):D7}";

        var ticket = new Domain.Entities.Ticket
        {
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            CustomerId = customerId,
            Priority = request.Priority,
            Status = TicketStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        // Auto-assign technician
        var assignedTechnician = await AutoAssignTechnicianAsync(request.CategoryId);
        if (assignedTechnician != null)
        {
            ticket.AssignedTechnicianId = assignedTechnician.UserId;
        }

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return await MapToResponseAsync(ticket);
    }

    public async Task<TicketResponse> GetTicketByIdAsync(int ticketId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {ticketId}");

        return await MapToResponseAsync(ticket);
    }

    public async Task<TicketResponse> GetTicketByNumberAsync(string ticketNumber)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {ticketNumber}");

        return await MapToResponseAsync(ticket);
    }

    public async Task<IEnumerable<TicketResponse>> GetAllTicketsAsync()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var responses = new List<TicketResponse>();
        foreach (var ticket in tickets)
        {
            responses.Add(await MapToResponseAsync(ticket));
        }

        return responses;
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByCustomerAsync(Guid customerId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var responses = new List<TicketResponse>();
        foreach (var ticket in tickets)
        {
            responses.Add(await MapToResponseAsync(ticket));
        }

        return responses;
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByTechnicianAsync(Guid technicianId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .Where(t => t.AssignedTechnicianId == technicianId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var responses = new List<TicketResponse>();
        foreach (var ticket in tickets)
        {
            responses.Add(await MapToResponseAsync(ticket));
        }

        return responses;
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByCategoryAsync(int categoryId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var responses = new List<TicketResponse>();
        foreach (var ticket in tickets)
        {
            responses.Add(await MapToResponseAsync(ticket));
        }

        return responses;
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByStatusAsync(TicketStatus status)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var responses = new List<TicketResponse>();
        foreach (var ticket in tickets)
        {
            responses.Add(await MapToResponseAsync(ticket));
        }

        return responses;
    }

    public async Task<TicketResponse> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, Guid userId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {request.TicketId}");

        var oldStatus = ticket.Status;
        ticket.Status = request.NewStatus;
        ticket.UpdatedAt = DateTime.UtcNow;

        if (request.NewStatus == TicketStatus.Closed)
        {
            ticket.ClosedAt = DateTime.UtcNow;
        }

        // Add status history
        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            OldStatus = oldStatus,
            NewStatus = request.NewStatus,
            ChangedBy = userId,
            ChangedAt = DateTime.UtcNow,
            Comment = request.Comment
        };

        _context.TicketStatusHistories.Add(history);
        await _context.SaveChangesAsync();

        return await MapToResponseAsync(ticket);
    }

    public async Task<TicketResponse> AssignTicketAsync(int ticketId, Guid technicianId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {ticketId}");

        ticket.AssignedTechnicianId = technicianId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapToResponseAsync(ticket);
    }

    public async Task<bool> DeleteTicketAsync(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<TicketMessageResponse>> GetTicketMessagesAsync(int ticketId)
    {
        var messages = await _context.TicketMessages
            .Include(m => m.Sender)
            .Where(m => m.TicketId == ticketId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m => new TicketMessageResponse
        {
            Id = m.Id,
            TicketId = m.TicketId,
            SenderId = m.SenderId,
            SenderName = m.Sender.FullName,
            Message = m.Message,
            SentAt = m.SentAt,
            IsRead = m.IsRead
        });
    }

    public async Task<TicketMessageResponse> SendMessageAsync(SendMessageRequest request, Guid senderId)
    {
        var message = new TicketMessage
        {
            TicketId = request.TicketId,
            SenderId = senderId,
            Message = request.Message,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.TicketMessages.Add(message);

        // Update ticket's UpdatedAt
        var ticket = await _context.Tickets.FindAsync(request.TicketId);
        if (ticket != null)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);

        return new TicketMessageResponse
        {
            Id = message.Id,
            TicketId = message.TicketId,
            SenderId = message.SenderId,
            SenderName = sender?.FullName ?? "Unknown",
            Message = message.Message,
            SentAt = message.SentAt,
            IsRead = message.IsRead
        };
    }

    // Helper: Auto-assign technician based on category
    private async Task<Domain.Entities.User?> AutoAssignTechnicianAsync(int categoryId)
    {
        // Get all technicians assigned to this category
        var technicians = await _context.TechnicianCategories
            .Where(tc => tc.CategoryId == categoryId)
            .Include(tc => tc.Technician)
            .Select(tc => tc.Technician)
            .ToListAsync();

        if (!technicians.Any())
            return null;

        // Simple round-robin: Get technician with least assigned tickets
        var technicianIds = technicians.Select(t => t.UserId).ToList();

        var ticketCounts = await _context.Tickets
            .Where(t => technicianIds.Contains(t.AssignedTechnicianId!.Value) && t.Status != TicketStatus.Closed)
            .GroupBy(t => t.AssignedTechnicianId)
            .Select(g => new { TechnicianId = g.Key, Count = g.Count() })
            .ToListAsync();

        // Find technician with minimum tickets
        var assignedTechnician = technicians
            .Select(t => new
            {
                Technician = t,
                Count = ticketCounts.FirstOrDefault(tc => tc.TechnicianId == t.UserId)?.Count ?? 0
            })
            .OrderBy(x => x.Count)
            .FirstOrDefault()?.Technician;

        return assignedTechnician;
    }

    // Helper: Map to response
    private async Task<TicketResponse> MapToResponseAsync(Domain.Entities.Ticket ticket)
    {
        var messageCount = await _context.TicketMessages.CountAsync(m => m.TicketId == ticket.Id);
        var unreadCount = await _context.TicketMessages.CountAsync(m => m.TicketId == ticket.Id && !m.IsRead);

        return new TicketResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,
            CategoryId = ticket.CategoryId,
            CategoryName = ticket.Category?.Name ?? "Unknown",
            CustomerId = ticket.CustomerId,
            CustomerName = ticket.Customer?.FullName ?? "Unknown",
            AssignedTechnicianId = ticket.AssignedTechnicianId,
            AssignedTechnicianName = ticket.AssignedTechnician?.FullName,
            Status = ticket.Status,
            Priority = ticket.Priority,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ClosedAt = ticket.ClosedAt,
            MessageCount = messageCount,
            UnreadMessageCount = unreadCount
        };
    }
}
