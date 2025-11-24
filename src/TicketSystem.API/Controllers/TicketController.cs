using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TicketSystem.API.Hubs;
using TicketSystem.Application.DTOs.Request;
using TicketSystem.Application.Services.Ticket;
using TicketSystem.Domain.Enums;
using TicketSystem.API.Authorization.Requirements;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IHubContext<TicketHub> _hubContext;
    private readonly IAuthorizationService _authorizationService;

    public TicketController(
        ITicketService ticketService,
        IHubContext<TicketHub> hubContext,
        IAuthorizationService authorizationService)
    {
        _ticketService = ticketService;
        _hubContext = hubContext;
        _authorizationService = authorizationService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    /// <summary>
    /// SECURITY: Admin-only endpoint to retrieve all tickets in the system
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTickets()
    {
        try
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Requires authorization - only ticket owner, assigned technician, or admin can access
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
    {
        try
        {
            // Authorization check: verify user can access this ticket
            var authResult = await _authorizationService.AuthorizeAsync(User, id, new TicketAccessRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticket);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Requires authorization - only ticket owner, assigned technician, or admin can access
    /// This endpoint is vulnerable to enumeration attacks, authorization is critical
    /// </summary>
    [HttpGet("number/{ticketNumber}")]
    public async Task<IActionResult> GetTicketByNumber(string ticketNumber)
    {
        try
        {
            var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);

            // Authorization check: verify user can access this ticket
            var authResult = await _authorizationService.AuthorizeAsync(User, ticket.Id, new TicketAccessRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            return Ok(ticket);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        try
        {
            var userId = GetCurrentUserId();
            var tickets = await _ticketService.GetTicketsByCustomerAsync(userId);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssignedTickets()
    {
        try
        {
            var userId = GetCurrentUserId();
            var tickets = await _ticketService.GetTicketsByTechnicianAsync(userId);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-categories")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GetTicketsByMyCategories()
    {
        try
        {
            var userId = GetCurrentUserId();
            var tickets = await _ticketService.GetTicketsByTechnicianCategoriesAsync(userId);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Admin/Technician only - filters tickets by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> GetTicketsByCategory(int categoryId)
    {
        try
        {
            var tickets = await _ticketService.GetTicketsByCategoryAsync(categoryId);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Admin/Technician only - filters tickets by status
    /// </summary>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> GetTicketsByStatus(TicketStatus status)
    {
        try
        {
            var tickets = await _ticketService.GetTicketsByStatusAsync(status);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var ticket = await _ticketService.CreateTicketAsync(request, userId);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Only ticket owner, assigned technician, or admin can update status
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
    {
        try
        {
            // Authorization check: verify user can modify this ticket
            var authResult = await _authorizationService.AuthorizeAsync(User, id, new TicketAccessRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var userId = GetCurrentUserId();
            request.TicketId = id;
            var ticket = await _ticketService.UpdateTicketStatusAsync(request, userId);
            return Ok(ticket);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/assign/{technicianId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTicket(int id, Guid technicianId)
    {
        try
        {
            var ticket = await _ticketService.AssignTicketAsync(id, technicianId);
            return Ok(ticket);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        try
        {
            var result = await _ticketService.DeleteTicketAsync(id);
            if (!result)
                return NotFound(new { message = "Ticket bulunamadı" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Only ticket owner, assigned technician, or admin can read messages
    /// Prevents unauthorized access to private conversations
    /// </summary>
    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetTicketMessages(int id)
    {
        try
        {
            // Authorization check: verify user can access this ticket's messages
            var authResult = await _authorizationService.AuthorizeAsync(User, id, new TicketAccessRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var messages = await _ticketService.GetTicketMessagesAsync(id);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Only ticket owner, assigned technician, or admin can send messages
    /// Prevents unauthorized users from injecting messages into conversations
    /// </summary>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request, [FromForm] IFormFile? attachment)
    {
        try
        {
            // Authorization check: verify user can send messages to this ticket
            var authResult = await _authorizationService.AuthorizeAsync(User, request.TicketId, new TicketAccessRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var userId = GetCurrentUserId();
            var message = await _ticketService.SendMessageAsync(request, userId, attachment);

            // Broadcast message to all users in the ticket room via SignalR
            await _hubContext.Clients.Group($"ticket_{request.TicketId}")
                .SendAsync("ReceiveMessage", message);

            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
