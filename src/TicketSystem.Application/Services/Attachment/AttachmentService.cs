using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Common;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Application.Services.Attachment;

public class AttachmentService : IAttachmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private const string UploadFolder = "uploads";

    public AttachmentService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<AttachmentResponse> UploadAttachmentAsync(IFormFile file, int ticketId, int? messageId, Guid uploaderId)
    {
        // Verify ticket exists and user has access
        var ticket = await _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {ticketId}");

        var user = await _context.Users.FindAsync(uploaderId);
        if (user == null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı");

        // Check if user has access to this ticket
        if (user.UserType == UserType.Customer && ticket.CustomerId != uploaderId)
            throw new UnauthorizedAccessException("Bu ticket'a erişim yetkiniz yok");

        if (user.UserType == UserType.Technician && ticket.AssignedTechnicianId != uploaderId)
        {
            // Check if technician is assigned to the ticket's category
            var hasCategoryAccess = await _context.TechnicianCategories
                .AnyAsync(tc => tc.TechnicianId == uploaderId && tc.CategoryId == ticket.CategoryId);

            if (!hasCategoryAccess)
                throw new UnauthorizedAccessException("Bu ticket'a erişim yetkiniz yok");
        }

        // If messageId is provided, verify it belongs to this ticket
        if (messageId.HasValue)
        {
            var message = await _context.TicketMessages.FindAsync(messageId.Value);
            if (message == null || message.TicketId != ticketId)
                throw new KeyNotFoundException("Mesaj bulunamadı");
        }

        // Create upload directory if it doesn't exist
        var uploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadFolder, ticketId.ToString());
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create database record
        var attachment = new TicketAttachment
        {
            MessageId = messageId,
            TicketId = ticketId,
            FileName = file.FileName,
            FileUrl = $"/{UploadFolder}/{ticketId}/{fileName}",
            FileSize = file.Length,
            FileType = file.ContentType,
            UploadedBy = uploaderId,
            UploadedAt = DateTimeProvider.Now
        };

        _context.TicketAttachments.Add(attachment);
        await _context.SaveChangesAsync();

        return new AttachmentResponse
        {
            Id = attachment.Id,
            MessageId = attachment.MessageId,
            TicketId = attachment.TicketId,
            FileName = attachment.FileName,
            FileUrl = attachment.FileUrl,
            FileSize = attachment.FileSize,
            FileType = attachment.FileType,
            UploadedBy = attachment.UploadedBy,
            UploaderName = user.FullName,
            UploadedAt = attachment.UploadedAt
        };
    }

    public async Task<List<AttachmentResponse>> GetTicketAttachmentsAsync(int ticketId, Guid userId)
    {
        // Verify ticket exists and user has access
        var ticket = await _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedTechnician)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket bulunamadı: {ticketId}");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı");

        // Check if user has access to this ticket
        if (user.UserType == UserType.Customer && ticket.CustomerId != userId)
            throw new UnauthorizedAccessException("Bu ticket'a erişim yetkiniz yok");

        if (user.UserType == UserType.Technician && ticket.AssignedTechnicianId != userId)
        {
            var hasCategoryAccess = await _context.TechnicianCategories
                .AnyAsync(tc => tc.TechnicianId == userId && tc.CategoryId == ticket.CategoryId);

            if (!hasCategoryAccess)
                throw new UnauthorizedAccessException("Bu ticket'a erişim yetkiniz yok");
        }

        var attachments = await _context.TicketAttachments
            .Where(a => a.TicketId == ticketId)
            .Include(a => a.Uploader)
            .OrderByDescending(a => a.UploadedAt)
            .Select(a => new AttachmentResponse
            {
                Id = a.Id,
                MessageId = a.MessageId,
                TicketId = a.TicketId,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                FileSize = a.FileSize,
                FileType = a.FileType,
                UploadedBy = a.UploadedBy,
                UploaderName = a.Uploader.FullName,
                UploadedAt = a.UploadedAt
            })
            .ToListAsync();

        return attachments;
    }

    public async Task DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await _context.TicketAttachments.FindAsync(attachmentId);

        if (attachment == null)
            throw new KeyNotFoundException("Dosya bulunamadı");

        // Delete physical file
        var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, attachment.FileUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        // Delete database record
        _context.TicketAttachments.Remove(attachment);
        await _context.SaveChangesAsync();
    }
}
