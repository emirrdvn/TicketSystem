using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Services.Notification;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, int? ticketId, NotificationType type, string title, string message);
    Task SendEmailAsync(string to, string subject, string body);
    Task MarkAsReadAsync(int notificationId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
