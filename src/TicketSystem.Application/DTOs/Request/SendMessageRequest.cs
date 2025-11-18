namespace TicketSystem.Application.DTOs.Request;

public class SendMessageRequest
{
    public int TicketId { get; set; }
    public string Message { get; set; } = string.Empty;
}
