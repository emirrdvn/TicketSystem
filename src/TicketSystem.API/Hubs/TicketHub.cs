using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TicketSystem.API.Hubs;

[Authorize]
public class TicketHub : Hub
{
    public async Task JoinTicket(string ticketId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
    }

    public async Task LeaveTicket(string ticketId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
    }

    public async Task SendMessage(int ticketId, string message, string senderName)
    {
        await Clients.Group($"ticket_{ticketId}").SendAsync("ReceiveMessage", new
        {
            ticketId,
            message,
            senderName,
            sentAt = DateTime.UtcNow
        });
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
