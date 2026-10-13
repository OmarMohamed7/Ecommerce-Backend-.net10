namespace E_Commerce.Features.Cart;

// Represents Cart item in cart response
public class CartItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total => Price * Quantity;
}