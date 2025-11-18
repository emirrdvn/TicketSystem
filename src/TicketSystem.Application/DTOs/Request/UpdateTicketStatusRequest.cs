using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.DTOs.Request;

public class UpdateTicketStatusRequest
{
    public int TicketId { get; set; }
    public TicketStatus NewStatus { get; set; }
    public string? Comment { get; set; }
}
