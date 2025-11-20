using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Entities;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _context.TicketCategories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt,
                    TicketCount = c.Tickets.Count,
                    TechnicianCount = c.TechnicianCategories.Count
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            var category = await _context.TicketCategories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt,
                    TicketCount = c.Tickets.Count,
                    TechnicianCount = c.TechnicianCategories.Count
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı" });

            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Kategori adı gereklidir" });

            // Check if category with same name already exists
            var existingCategory = await _context.TicketCategories
                .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());

            if (existingCategory)
                return BadRequest(new { message = "Bu isimde bir kategori zaten mevcut" });

            var category = new TicketCategory
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TicketCategories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _context.TicketCategories.FindAsync(id);

            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı" });

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                // Check if another category with same name exists
                var existingCategory = await _context.TicketCategories
                    .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != id);

                if (existingCategory)
                    return BadRequest(new { message = "Bu isimde bir kategori zaten mevcut" });

                category.Name = request.Name.Trim();
            }

            if (request.Description != null)
                category.Description = request.Description.Trim();

            if (request.IsActive.HasValue)
                category.IsActive = request.IsActive.Value;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.TicketCategories
                .Include(c => c.Tickets)
                .Include(c => c.TechnicianCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = "Kategori bulunamadı" });

            // Check if category has tickets
            if (category.Tickets.Any())
                return BadRequest(new { message = "Bu kategoriye ait ticketlar var. Önce ticketları başka bir kategoriye taşıyın." });

            // Remove technician associations
            if (category.TechnicianCategories.Any())
            {
                _context.TechnicianCategories.RemoveRange(category.TechnicianCategories);
            }

            _context.TicketCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategori başarıyla silindi" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
