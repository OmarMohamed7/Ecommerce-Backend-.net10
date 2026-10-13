namespace E_Commerce.Features.Auth;

public sealed record LoginDto(
    string Email,
    string Password
);