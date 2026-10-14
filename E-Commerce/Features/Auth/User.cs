using E_Commerce.Features.Audit;

namespace E_Commerce.Features.Auth;

public class User : IAuditLoggable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [NotAudited] public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "USER";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}