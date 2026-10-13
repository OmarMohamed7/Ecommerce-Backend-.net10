namespace E_Commerce.DTOs;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword
);