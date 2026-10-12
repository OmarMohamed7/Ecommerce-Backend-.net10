namespace E_Commerce.DTOs;

public sealed record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock
);