namespace E_Commerce.Models;

public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; }  = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
}
