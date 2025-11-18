using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs.Request;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;
using TicketSystem.Infrastructure.Data;
using TicketSystem.Infrastructure.Identity;

namespace TicketSystem.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(ApplicationDbContext context, JwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Geçersiz email veya şifre");

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Geçersiz email veya şifre");

        var token = _tokenGenerator.GenerateToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        // RefreshToken'ı database'e kaydetmek isterseniz buraya ekleyin

        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (existingUser != null)
            throw new InvalidOperationException("Bu email adresi zaten kullanılıyor");

        var user = new Domain.Entities.User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            UserType = request.UserType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenGenerator.GenerateToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new AuthResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // RefreshToken validation burada yapılır
        // Basit implementasyon için şimdilik throw ediyoruz
        throw new NotImplementedException("RefreshToken henüz implement edilmedi");
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var principal = _tokenGenerator.ValidateToken(token);
        return principal != null;
    }
}
