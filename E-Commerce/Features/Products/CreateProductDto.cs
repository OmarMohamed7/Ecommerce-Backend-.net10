namespace E_Commerce.Features.Products;

public sealed record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock
);