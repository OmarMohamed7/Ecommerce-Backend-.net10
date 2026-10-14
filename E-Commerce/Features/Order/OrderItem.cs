using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Features.Order;

// Represent One product into the order
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")] public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; private set; }
}