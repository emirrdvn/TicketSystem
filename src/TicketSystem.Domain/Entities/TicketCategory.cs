using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TicketCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<TechnicianCategory> TechnicianCategories { get; set; } = new List<TechnicianCategory>();
}
