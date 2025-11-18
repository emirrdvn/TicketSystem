using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.DTOs.Request;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
