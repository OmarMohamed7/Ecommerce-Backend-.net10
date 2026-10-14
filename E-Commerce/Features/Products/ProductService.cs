using E_Commerce.Common;
using E_Commerce.Db;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Features.Products;

public class ProductService(AppDbContext db, ILogger<ProductService> logger) : IProductService
{
    public async Task<PagedResult<Product>> GetAll(ProductQueryParams queryParams)
    {
        logger.LogInformation("Getting all products for {ProductQueryParams}", queryParams);
        var query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{queryParams.Search}%"));

        if (queryParams.MinPrice.HasValue) query = query.Where(p => p.Price >= queryParams.MinPrice);

        if (queryParams.MaxPrice.HasValue) query = query.Where(p => p.Price <= queryParams.MaxPrice);

        if (queryParams.MinQuantity.HasValue) query = query.Where(p => p.Stock >= queryParams.MinQuantity);

        if (queryParams.MaxQuantity.HasValue) query = query.Where(p => p.Stock <= queryParams.MaxQuantity);

        var totalCount = await query.CountAsync();


        var ordered = queryParams.SortBy.ToLowerInvariant() switch
        {
            "price" => queryParams.Descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "stock" => queryParams.Descending
                ? query.OrderByDescending(p => p.Stock)
                : query.OrderBy(p => p.Stock),
            "name" => queryParams.Descending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            _ => queryParams.Descending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
        };

        var items = await ordered
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PagedResult<Product>(items, totalCount, queryParams.Page, queryParams.PageSize);
    }

    public async Task<Product?> GetById(int id)
    {
        logger.LogInformation("Getting Product With Id {id}", id);

        return await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }


    public async Task<Product> Create(Product request)
    {
        db.Products.Add(request);

        await db.SaveChangesAsync();

        return request;
    }

    public async Task<Product?> Update(int id, UpdateProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.Price = request.Price ?? product.Price;
        product.Stock = request.Stock ?? product.Stock;

        await db.SaveChangesAsync();

        return product;
    }

    public async Task<bool> Delete(int id)
    {
        var product = await GetById(id);

        if (product is null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return true;
    }
}