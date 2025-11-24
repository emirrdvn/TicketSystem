using Microsoft.AspNetCore.Http;
using TicketSystem.Application.DTOs.Response;

namespace TicketSystem.Application.Services.Attachment;

public interface IAttachmentService
{
    Task<AttachmentResponse> UploadAttachmentAsync(IFormFile file, int ticketId, int? messageId, Guid uploaderId);
    Task<List<AttachmentResponse>> GetTicketAttachmentsAsync(int ticketId, Guid userId);
    Task DeleteAttachmentAsync(int attachmentId);
}
