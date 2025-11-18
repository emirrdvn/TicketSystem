using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TechnicianCategory : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public User Technician { get; set; } = null!;
    public TicketCategory Category { get; set; } = null!;
}
