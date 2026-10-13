using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_Commerce.Db;
using E_Commerce.DTOs;
using E_Commerce.Models;
using E_Commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Services;

public class AuthService(AppDbContext db, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        if (await IsUserExists(registerDto.Email)) return null;

        var user = new User
        {
            Email = registerDto.Email,
            Name = registerDto.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        var (token, expiresAt) = GenerateJwtToken(user);

        db.Users.Add(user);
        await db.SaveChangesAsync();


        return new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Name = user.Name,
            UserId = user.Id,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null) return null;

        var isPasswordValid =
            BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

        if (!isPasswordValid) return null;


        var (token, expiresAt) = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Name = user.Name,
            UserId = user.Id,
            Role = user.Role
        };
    }

    private async Task<bool> IsUserExists(string email)
    {
        return await db.Users.AnyAsync(u => u.Email == email);
    }

    private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var expiresInMinutes = Convert.ToDouble(configuration["Jwt:ExpiresInMinutes"]);

        var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            expires: expires,
            claims: claims,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}