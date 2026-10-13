using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Features.Products;

public class Product
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(150)]
    public string Description { get; set; }  = string.Empty;
    
    [Column(TypeName = "decimal(18, 2)")] 
    public decimal Price { get; set; }
    
    public int Stock { get; set; } = 0;
    
    public DateTime CreatedAt { get; init; } =  DateTime.UtcNow;
    
}
