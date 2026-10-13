namespace E_Commerce.DTOs;

public sealed record RegisterDto(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword
);