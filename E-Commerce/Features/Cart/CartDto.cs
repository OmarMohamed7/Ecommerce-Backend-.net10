namespace E_Commerce.Features.Cart;

public class CartDto
{
    public int CartId { get; set; }

    public List<CartItemDto> Items { get; set; } = [];

    public decimal Price { get; set; }

    public int Quantity { get; set; } = 0;

    public decimal Total { get; set; } = 0;
}