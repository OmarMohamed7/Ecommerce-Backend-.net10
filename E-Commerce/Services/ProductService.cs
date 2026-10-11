using E_Commerce.DTOs;
using E_Commerce.Models;


namespace E_Commerce.Services;

public class ProductService: IProductService
{
    private readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Screen", Description = "Full HD Screen", Stock = 100, Price = 5000 },
        new Product { Id = 2, Name = "Laptop", Description = "Gaming Laptop", Stock = 100, Price = 35000 },
        new Product { Id = 3, Name = "Laptop Charger", Description = "150 Watt Charger", Stock = 70, Price = 1800 },
    ];

    public IReadOnlyList<Product> GetAll() => _products;
    
    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public Product Create(Product request)
    {
        _products.Add(request);
        
        return request;
    }

    public Product? Update(int id, UpdateProductRequest request)
    {
        var product = GetById(id);
        if (product is null)
        {
            return null;
        }

        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.Price = request.Price ?? product.Price;
        product.Stock = request.Stock ?? product.Stock;
        
        return product;
        
    }
    
    public void Delete(int id) => _products.Remove(GetById(id));
}