namespace E_Commerce.DTOs;

public sealed record UpdateProductRequest(
    string? Name = null,
    string? Description = null,
    decimal? Price = null,
    int? Stock = null
);