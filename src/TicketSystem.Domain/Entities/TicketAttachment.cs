using TicketSystem.Domain.Common;

namespace TicketSystem.Domain.Entities;

public class TicketAttachment : BaseEntity
{
    public int? MessageId { get; set; }
    public int TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public TicketMessage? Message { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public User Uploader { get; set; } = null!;
}
