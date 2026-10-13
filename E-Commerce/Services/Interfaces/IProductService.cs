using E_Commerce.DTOs;
using E_Commerce.Models;

namespace E_Commerce.Services;

public interface IProductService
{
    public Task<PagedResult<Product>> GetAll(ProductQueryParams queryParams);
    public Task<Product?> GetById(int id);

    public Task<Product> Create(Product request);

    public Task<Product?> Update(int id, UpdateProductRequest request);

    public Task<bool> Delete(int id);
}