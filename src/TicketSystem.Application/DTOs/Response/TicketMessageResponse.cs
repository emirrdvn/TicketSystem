namespace TicketSystem.Application.DTOs.Response;

public class TicketMessageResponse
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public List<AttachmentResponse>? Attachments { get; set; }
}
