namespace E_Commerce.DTOs;

public sealed record LoginRequest(
    string Email,
    string Password
);