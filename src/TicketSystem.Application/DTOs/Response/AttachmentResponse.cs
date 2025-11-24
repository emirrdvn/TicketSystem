namespace TicketSystem.Application.DTOs.Response;

public class AttachmentResponse
{
    public int Id { get; set; }
    public int? MessageId { get; set; }
    public int TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public string UploaderName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
