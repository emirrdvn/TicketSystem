using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Application.Services.User;
using TicketSystem.Domain.Enums;
using TicketSystem.API.Authorization.Requirements;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public UserController(IUserService userService, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Only the user themselves or admin can view user profile
    /// Prevents unauthorized access to other users' personal information
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            // Authorization check: verify user can access this profile (self or admin)
            var authResult = await _authorizationService.AuthorizeAsync(User, userId, new ResourceOwnerRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
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

    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return Ok(user);
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

    [HttpGet("type/{userType}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersByType(UserType userType)
    {
        try
        {
            var users = await _userService.GetUsersByTypeAsync(userType);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SECURITY: Only the user themselves or admin can update user profile
    /// Prevents unauthorized modification of other users' data
    /// </summary>
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserResponse userDto)
    {
        try
        {
            // Authorization check: verify user can modify this profile (self or admin)
            var authResult = await _authorizationService.AuthorizeAsync(User, userId, new ResourceOwnerRequirement());
            if (!authResult.Succeeded)
            {
                return Forbid(); // 403 Forbidden
            }

            // Additional security: prevent non-admins from changing their user type
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId == userId && currentUserRole != "Admin")
            {
                // Regular users cannot change their own user type
                var existingUser = await _userService.GetUserByIdAsync(userId);
                userDto.UserType = existingUser.UserType;
            }

            var user = await _userService.UpdateUserAsync(userId, userDto);
            return Ok(user);
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

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{userId}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(Guid userId)
    {
        try
        {
            var result = await _userService.ActivateUserAsync(userId);
            if (!result)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            return Ok(new { message = "Kullanıcı aktif edildi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{userId}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(Guid userId)
    {
        try
        {
            var result = await _userService.DeactivateUserAsync(userId);
            if (!result)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            return Ok(new { message = "Kullanıcı pasif edildi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{technicianId}/categories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTechnicianCategories(Guid technicianId)
    {
        try
        {
            var categories = await _userService.GetTechnicianCategoriesAsync(technicianId);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{technicianId}/categories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignCategoriesToTechnician(Guid technicianId, [FromBody] IEnumerable<int> categoryIds)
    {
        try
        {
            await _userService.AssignCategoriesToTechnicianAsync(technicianId, categoryIds);
            return Ok(new { message = "Kategoriler atandı" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
