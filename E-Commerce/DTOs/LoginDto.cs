namespace E_Commerce.DTOs;

public sealed record LoginDto(
    string Email,
    string Password
);