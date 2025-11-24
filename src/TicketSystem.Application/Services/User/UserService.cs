using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Common;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Application.Services.User;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new KeyNotFoundException($"Kullanıcı bulunamadı: {userId}");

        return MapToResponse(user);
    }

    public async Task<UserResponse> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
            throw new KeyNotFoundException($"Kullanıcı bulunamadı: {email}");

        return MapToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return users.Select(MapToResponse);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersByTypeAsync(UserType userType)
    {
        var users = await _context.Users
            .Where(u => u.UserType == userType)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return users.Select(MapToResponse);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid userId, UserResponse userDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new KeyNotFoundException($"Kullanıcı bulunamadı: {userId}");

        user.FullName = userDto.FullName;
        user.PhoneNumber = userDto.PhoneNumber;
        user.UpdatedAt = DateTimeProvider.Now;

        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        user.IsActive = true;
        user.UpdatedAt = DateTimeProvider.Now;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTimeProvider.Now;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<int>> GetTechnicianCategoriesAsync(Guid technicianId)
    {
        var categoryIds = await _context.TechnicianCategories
            .Where(tc => tc.TechnicianId == technicianId)
            .Select(tc => tc.CategoryId)
            .ToListAsync();

        return categoryIds;
    }

    public async Task<bool> AssignCategoriesToTechnicianAsync(Guid technicianId, IEnumerable<int> categoryIds)
    {
        var technician = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == technicianId && u.UserType == UserType.Technician);

        if (technician == null)
            throw new InvalidOperationException("Teknisyen bulunamadı");

        // Remove existing categories
        var existingCategories = await _context.TechnicianCategories
            .Where(tc => tc.TechnicianId == technicianId)
            .ToListAsync();

        _context.TechnicianCategories.RemoveRange(existingCategories);

        // Add new categories
        foreach (var categoryId in categoryIds)
        {
            var technicianCategory = new TechnicianCategory
            {
                TechnicianId = technicianId,
                CategoryId = categoryId,
                CreatedAt = DateTimeProvider.Now
            };

            _context.TechnicianCategories.Add(technicianCategory);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private UserResponse MapToResponse(Domain.Entities.User user)
    {
        return new UserResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
