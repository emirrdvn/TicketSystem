using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Services.User;

public interface IUserService
{
    Task<UserResponse> GetUserByIdAsync(Guid userId);
    Task<UserResponse> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<IEnumerable<UserResponse>> GetUsersByTypeAsync(UserType userType);
    Task<UserResponse> UpdateUserAsync(Guid userId, UserResponse userDto);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ActivateUserAsync(Guid userId);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<IEnumerable<int>> GetTechnicianCategoriesAsync(Guid technicianId);
    Task<bool> AssignCategoriesToTechnicianAsync(Guid technicianId, IEnumerable<int> categoryIds);
}
