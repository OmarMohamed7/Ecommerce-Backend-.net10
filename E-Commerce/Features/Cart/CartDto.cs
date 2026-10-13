namespace E_Commerce.Features.Cart;

public class CartDto
{
    public int CartId { get; set; }

    public List<CartItemDto> Items { get; set; } = [];

    // Derived from the items so they can never go stale
    public int Quantity => Items.Sum(i => i.Quantity);

    public decimal Total => Items.Sum(i => i.Total);
}