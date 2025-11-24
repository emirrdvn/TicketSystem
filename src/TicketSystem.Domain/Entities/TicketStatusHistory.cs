using TicketSystem.Domain.Common;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class TicketStatusHistory : BaseEntity
{
    public int TicketId { get; set; }
    public TicketStatus OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTimeProvider.Now;
    public string? Comment { get; set; }

    // Navigation properties
    public Ticket Ticket { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}
