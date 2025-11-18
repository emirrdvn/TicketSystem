using TicketSystem.Domain.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public int? TicketId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    // Navigation properties
    public User User { get; set; } = null!;
    public Ticket? Ticket { get; set; }
}
