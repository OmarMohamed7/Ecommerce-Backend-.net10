namespace E_Commerce.Features.Products;

public sealed record UpdateProductRequest(
    string? Name = null,
    string? Description = null,
    decimal? Price = null,
    int? Stock = null
);