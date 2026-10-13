namespace E_Commerce.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public DateTime ExpiresAt { get; set; }
}