using E_Commerce.Models;
using E_Commerce.Common;

namespace E_Commerce.Features.Products;

public interface IProductService
{
    public Task<PagedResult<Product>> GetAll(ProductQueryParams queryParams);
    public Task<Product?> GetById(int id);

    public Task<Product> Create(Product request);

    public Task<Product?> Update(int id, UpdateProductRequest request);

    public Task<bool> Delete(int id);
}