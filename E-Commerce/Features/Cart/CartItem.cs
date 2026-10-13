using E_Commerce.Features.Products;

namespace E_Commerce.Features.Cart;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public Cart Cart { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}