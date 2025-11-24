using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs.Request;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Common;
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
            Status = TicketStatus.New,
            CreatedAt = DateTimeProvider.Now
        };

        // NOTE: No auto-assignment at creation time
        // Ticket will be assigned when a technician sends the first message

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

    public async Task<IEnumerable<TicketResponse>> GetTicketsByTechnicianCategoriesAsync(Guid technicianId)
    {
        // Get technician's category IDs
        var technicianCategoryIds = await _context.TechnicianCategories
            .Where(tc => tc.TechnicianId == technicianId)
            .Select(tc => tc.CategoryId)
            .ToListAsync();

        if (!technicianCategoryIds.Any())
        {
            return new List<TicketResponse>();
        }

        // Get all tickets in those categories
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .Include(t => t.Messages)
            .Where(t => technicianCategoryIds.Contains(t.CategoryId))
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
        ticket.UpdatedAt = DateTimeProvider.Now;

        if (request.NewStatus == TicketStatus.Closed)
        {
            ticket.ClosedAt = DateTimeProvider.Now;
        }

        // Add status history
        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,
            OldStatus = oldStatus,
            NewStatus = request.NewStatus,
            ChangedBy = userId,
            ChangedAt = DateTimeProvider.Now,
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
        ticket.UpdatedAt = DateTimeProvider.Now;

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
            .Include(m => m.Attachments)
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
            IsRead = m.IsRead,
            Attachments = m.Attachments?.Select(a => new AttachmentResponse
            {
                Id = a.Id,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                FileSize = a.FileSize,
                FileType = a.FileType,
                UploadedAt = a.UploadedAt
            }).ToList()
        });
    }

    public async Task<TicketMessageResponse> SendMessageAsync(SendMessageRequest request, Guid senderId, Microsoft.AspNetCore.Http.IFormFile? attachment = null)
    {
        var message = new TicketMessage
        {
            TicketId = request.TicketId,
            SenderId = senderId,
            Message = request.Message,
            SentAt = DateTimeProvider.Now,
            IsRead = false
        };

        _context.TicketMessages.Add(message);

        // Update ticket's UpdatedAt
        var ticket = await _context.Tickets
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId);

        if (ticket != null)
        {
            ticket.UpdatedAt = DateTimeProvider.Now;

            // AUTO-ASSIGN LOGIC: If sender is technician/admin and ticket is not assigned yet
            var sender = await _context.Users.FindAsync(senderId);
            if (sender != null &&
                (sender.UserType == UserType.Technician || sender.UserType == UserType.Admin) &&
                ticket.AssignedTechnicianId == null)
            {
                // Assign the first responder
                ticket.AssignedTechnicianId = senderId;

                // If ticket is still "New", change to "InProgress"
                if (ticket.Status == TicketStatus.New)
                {
                    ticket.Status = TicketStatus.InProgress;
                }

                // Log status history
                var statusHistory = new TicketStatusHistory
                {
                    TicketId = ticket.Id,
                    OldStatus = TicketStatus.New,
                    NewStatus = TicketStatus.InProgress,
                    ChangedBy = senderId,
                    ChangedAt = DateTimeProvider.Now,
                    Comment = "Otomatik atama - İlk yanıt"
                };
                _context.TicketStatusHistories.Add(statusHistory);
            }
        }

        await _context.SaveChangesAsync();

        // Handle file attachment if provided
        if (attachment != null && attachment.Length > 0)
        {
            var uploadFolder = "uploads";
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadPath = Path.Combine(webRootPath, uploadFolder, request.TicketId.ToString());

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(attachment.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }

            var ticketAttachment = new TicketAttachment
            {
                MessageId = message.Id,
                TicketId = request.TicketId,
                FileName = attachment.FileName,
                FileUrl = $"/{uploadFolder}/{request.TicketId}/{fileName}",
                FileSize = attachment.Length,
                FileType = attachment.ContentType,
                UploadedBy = senderId,
                UploadedAt = DateTimeProvider.Now
            };

            _context.TicketAttachments.Add(ticketAttachment);
            await _context.SaveChangesAsync();
        }

        var senderUser = await _context.Users.FindAsync(senderId);

        // Reload message with attachments
        var messageWithAttachments = await _context.TicketMessages
            .Include(m => m.Attachments)
            .FirstOrDefaultAsync(m => m.Id == message.Id);

        return new TicketMessageResponse
        {
            Id = message.Id,
            TicketId = message.TicketId,
            SenderId = message.SenderId,
            SenderName = senderUser?.FullName ?? "Unknown",
            Message = message.Message,
            SentAt = message.SentAt,
            IsRead = message.IsRead,
            Attachments = messageWithAttachments?.Attachments?.Select(a => new AttachmentResponse
            {
                Id = a.Id,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                FileSize = a.FileSize,
                FileType = a.FileType,
                UploadedAt = a.UploadedAt
            }).ToList()
        };
    }

    // NOTE: Auto-assignment removed from ticket creation
    // Tickets are now assigned when a technician sends the first message (see SendMessageAsync method)
    // This ensures technicians only get assigned to tickets they actively engage with

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
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ClosedAt = ticket.ClosedAt,
            MessageCount = messageCount,
            UnreadMessageCount = unreadCount
        };
    }
}
