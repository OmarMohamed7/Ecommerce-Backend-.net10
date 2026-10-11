using E_Commerce.DTOs;
using E_Commerce.Models;

namespace E_Commerce.Services;

public interface IProductService
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(int id);
    
    Product Create(Product request);

    public Product? Update(int id, UpdateProductRequest request);
    
    public void Delete(int id);

}
