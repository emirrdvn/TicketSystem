using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Application.Services.User;
using TicketSystem.Domain.Enums;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
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

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserResponse userDto)
    {
        try
        {
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
