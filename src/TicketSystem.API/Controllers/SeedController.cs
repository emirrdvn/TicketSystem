using Microsoft.AspNetCore.Mvc;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;
using TicketSystem.Infrastructure.Identity;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SeedController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin()
    {
        // Check if admin already exists
        var existingAdmin = _context.Users.FirstOrDefault(u => u.Email == "admin@ticketsystem.com");
        if (existingAdmin != null)
        {
            return BadRequest(new { message = "Admin already exists" });
        }

        // Create admin user
        var admin = new User
        {
            UserId = Guid.NewGuid(),
            Email = "admin@ticketsystem.com",
            PasswordHash = PasswordHasher.HashPassword("Admin123"),
            FullName = "System Admin",
            UserType = UserType.Admin,
            PhoneNumber = "05551234567",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Admin created successfully",
            email = "admin@ticketsystem.com",
            password = "Admin123",
            userType = "Admin"
        });
    }

    [HttpPost("create-technician")]
    public async Task<IActionResult> CreateTechnician()
    {
        // Check if technician already exists
        var existingTech = _context.Users.FirstOrDefault(u => u.Email == "tech@ticketsystem.com");
        if (existingTech != null)
        {
            return BadRequest(new { message = "Technician already exists" });
        }

        // Create technician user
        var technician = new User
        {
            UserId = Guid.NewGuid(),
            Email = "tech@ticketsystem.com",
            PasswordHash = PasswordHasher.HashPassword("Tech123"),
            FullName = "Test Technician",
            UserType = UserType.Technician,
            PhoneNumber = "05559876543",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(technician);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Technician created successfully",
            email = "tech@ticketsystem.com",
            password = "Tech123",
            userType = "Technician"
        });
    }
}
