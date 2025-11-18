using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TicketMessage : BaseEntity
{
    public int TicketId { get; set; }
    public Guid SenderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public Ticket Ticket { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
