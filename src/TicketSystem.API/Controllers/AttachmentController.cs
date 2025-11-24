using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.Application.Services.Attachment;
using TicketSystem.Domain.Enums;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<AttachmentController> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".txt" };

    public AttachmentController(IAttachmentService attachmentService, ILogger<AttachmentController> logger)
    {
        _attachmentService = attachmentService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAttachment([FromForm] IFormFile file, [FromForm] int ticketId, [FromForm] int? messageId = null)
    {
        try
        {
            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = $"Dosya boyutu {MaxFileSize / (1024 * 1024)}MB'dan büyük olamaz" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = "Desteklenmeyen dosya formatı. İzin verilen: " + string.Join(", ", AllowedExtensions) });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Geçersiz kullanıcı" });

            var attachment = await _attachmentService.UploadAttachmentAsync(file, ticketId, messageId, userId);

            return Ok(attachment);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading attachment");
            return StatusCode(500, new { message = "Dosya yüklenirken hata oluştu" });
        }
    }

    [HttpGet("ticket/{ticketId}")]
    public async Task<IActionResult> GetTicketAttachments(int ticketId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Geçersiz kullanıcı" });

            var attachments = await _attachmentService.GetTicketAttachmentsAsync(ticketId, userId);
            return Ok(attachments);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attachments");
            return StatusCode(500, new { message = "Dosyalar yüklenirken hata oluştu" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        try
        {
            await _attachmentService.DeleteAttachmentAsync(id);
            return Ok(new { message = "Dosya başarıyla silindi" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment");
            return StatusCode(500, new { message = "Dosya silinirken hata oluştu" });
        }
    }
}
