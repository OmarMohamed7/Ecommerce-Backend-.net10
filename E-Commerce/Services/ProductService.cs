using E_Commerce.Db;
using E_Commerce.DTOs;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services;

public class ProductService(AppDbContext db) : IProductService
{
    public async Task<IEnumerable<Product>> GetAll()
    {
        return await db.Products
            .AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetById(int id)
    {
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