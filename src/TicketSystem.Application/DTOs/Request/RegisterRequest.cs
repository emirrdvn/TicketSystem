using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.DTOs.Request;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserType UserType { get; set; } = UserType.Customer;
}
