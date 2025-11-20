using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.API.Hubs;

[Authorize]
public class TicketHub : Hub
{
    private readonly ApplicationDbContext _context;

    public TicketHub(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// SECURITY: Validates user has access to ticket before joining the room
    /// Prevents unauthorized users from listening to ticket updates
    /// </summary>
    public async Task JoinTicket(string ticketId)
    {
        if (!int.TryParse(ticketId, out var ticketIdInt))
        {
            throw new HubException("Invalid ticket ID");
        }

        // Get current user
        var userIdClaim = Context.User?.FindFirst("userId")?.Value;
        var roleClaim = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("Unauthorized");
        }

        // Check if user has access to this ticket
        var hasAccess = false;

        // Admins and Technicians can access all tickets
        if (roleClaim == "Admin" || roleClaim == "Technician")
        {
            hasAccess = true;
        }
        else
        {
            // Check if user is ticket owner or assigned technician
            var ticket = await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == ticketIdInt);

            if (ticket == null)
            {
                throw new HubException("Ticket not found");
            }

            hasAccess = ticket.CustomerId == userId ||
                       (ticket.AssignedTechnicianId.HasValue && ticket.AssignedTechnicianId.Value == userId);
        }

        if (!hasAccess)
        {
            throw new HubException("Forbidden: You do not have access to this ticket");
        }

        // User is authorized, add to group
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
    }

    public async Task LeaveTicket(string ticketId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
    }

    /// <summary>
    /// SECURITY: DISABLED - Messages should only be sent through the REST API endpoint
    /// which has proper authorization and persistence logic
    /// </summary>
    [Obsolete("Use POST /api/ticket/messages endpoint instead")]
    public Task SendMessage(int ticketId, string message, string senderName)
    {
        throw new HubException("This method is disabled. Use POST /api/ticket/messages endpoint instead.");
    }

    public async Task NotifyStatusChange(int ticketId, string newStatus, string changedBy)
    {
        await Clients.Group($"ticket_{ticketId}").SendAsync("StatusChanged", new
        {
            ticketId,
            newStatus,
            changedBy,
            changedAt = DateTime.UtcNow
        });
    }

    public async Task UserTyping(int ticketId, string userName)
    {
        await Clients.OthersInGroup($"ticket_{ticketId}").SendAsync("UserIsTyping", new
        {
            ticketId,
            userName
        });
    }

    public async Task UserStoppedTyping(int ticketId, string userName)
    {
        await Clients.OthersInGroup($"ticket_{ticketId}").SendAsync("UserStoppedTyping", new
        {
            ticketId,
            userName
        });
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("userId")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("userId")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
