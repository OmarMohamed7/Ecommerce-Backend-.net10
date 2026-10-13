namespace E_Commerce.Features.Auth;

public sealed record RegisterDto(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword
);