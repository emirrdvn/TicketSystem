using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("seed-all")]
    public async Task<IActionResult> SeedAll()
    {
        try
        {
            var results = new List<string>();

            // 1. Create Admin
            if (!await _context.Users.AnyAsync(u => u.Email == "admin@ticketsystem.com"))
            {
                var admin = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = "admin@ticketsystem.com",
                    PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                    FullName = "System Admin",
                    UserType = UserType.Admin,
                    PhoneNumber = "05551234567",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(admin);
                results.Add("✅ Admin created: admin@ticketsystem.com / Admin123!");
            }
            else
            {
                results.Add("⚠️ Admin already exists");
            }

            // 2. Create Categories
            var categoryNames = new[] { "Teknik Destek", "Yazılım", "Donanım", "Ağ", "Güvenlik" };
            var createdCategories = new List<TicketCategory>();

            foreach (var name in categoryNames)
            {
                if (!await _context.TicketCategories.AnyAsync(c => c.Name == name))
                {
                    var category = new TicketCategory
                    {
                        Name = name,
                        Description = $"{name} kategorisi",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.TicketCategories.Add(category);
                    createdCategories.Add(category);
                    results.Add($"✅ Category created: {name}");
                }
            }

            await _context.SaveChangesAsync();

            // 3. Create Technicians
            var technicianData = new[]
            {
                new { Email = "tech1@ticketsystem.com", Name = "Ahmet Yılmaz", Phone = "05551111111" },
                new { Email = "tech2@ticketsystem.com", Name = "Ayşe Kaya", Phone = "05552222222" },
                new { Email = "tech3@ticketsystem.com", Name = "Mehmet Demir", Phone = "05553333333" }
            };

            var technicians = new List<User>();
            foreach (var data in technicianData)
            {
                if (!await _context.Users.AnyAsync(u => u.Email == data.Email))
                {
                    var tech = new User
                    {
                        UserId = Guid.NewGuid(),
                        Email = data.Email,
                        PasswordHash = PasswordHasher.HashPassword("Tech123!"),
                        FullName = data.Name,
                        UserType = UserType.Technician,
                        PhoneNumber = data.Phone,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(tech);
                    technicians.Add(tech);
                    results.Add($"✅ Technician created: {data.Email} / Tech123!");
                }
            }

            await _context.SaveChangesAsync();

            // 4. Assign Technicians to Categories
            if (technicians.Count > 0 && createdCategories.Count > 0)
            {
                var allCategories = await _context.TicketCategories.ToListAsync();
                var allTechnicians = await _context.Users.Where(u => u.UserType == UserType.Technician).ToListAsync();

                foreach (var tech in allTechnicians)
                {
                    // Her teknisyene 2-3 kategori ata
                    var assignedCategories = allCategories.Take(2).ToList();
                    foreach (var category in assignedCategories)
                    {
                        if (!await _context.TechnicianCategories.AnyAsync(tc =>
                            tc.TechnicianId == tech.UserId && tc.CategoryId == category.Id))
                        {
                            var techCategory = new TechnicianCategory
                            {
                                TechnicianId = tech.UserId,
                                CategoryId = category.Id,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.TechnicianCategories.Add(techCategory);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                results.Add("✅ Technicians assigned to categories");
            }

            // 5. Create Demo Customer
            if (!await _context.Users.AnyAsync(u => u.Email == "customer@test.com"))
            {
                var customer = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = "customer@test.com",
                    PasswordHash = PasswordHasher.HashPassword("Customer123!"),
                    FullName = "Demo Customer",
                    UserType = UserType.Customer,
                    PhoneNumber = "05559999999",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(customer);
                await _context.SaveChangesAsync();
                results.Add("✅ Demo customer created: customer@test.com / Customer123!");
            }

            return Ok(new
            {
                message = "Seed completed successfully",
                results = results,
                credentials = new
                {
                    admin = new { email = "admin@ticketsystem.com", password = "Admin123!" },
                    technicians = new[]
                    {
                        new { email = "tech1@ticketsystem.com", password = "Tech123!" },
                        new { email = "tech2@ticketsystem.com", password = "Tech123!" },
                        new { email = "tech3@ticketsystem.com", password = "Tech123!" }
                    },
                    customer = new { email = "customer@test.com", password = "Customer123!" }
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Seed failed", error = ex.Message });
        }
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
