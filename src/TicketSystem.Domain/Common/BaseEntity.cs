namespace TicketSystem.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTimeProvider.Now;
    public DateTime? UpdatedAt { get; set; }
}
