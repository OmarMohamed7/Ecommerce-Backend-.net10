namespace E_Commerce.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    int UserId,
    string Name,
    string Email
);