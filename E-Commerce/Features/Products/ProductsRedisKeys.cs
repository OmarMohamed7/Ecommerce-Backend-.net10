namespace E_Commerce.Features.Products;

public static class ProductsRedisKeys
{
    public const string ProductById = "product:{id}";

    public static string Products(ProductQueryParams p)
    {
        return
            $"products:page={p.Page}:size={p.PageSize}:search={p.Search}:sort={p.SortBy}:minPrice={p.MinPrice}:maxPrice={p.MaxPrice}:minQuantity={p.MinQuantity}:maxQuantity={p.MaxQuantity}";
    }
}