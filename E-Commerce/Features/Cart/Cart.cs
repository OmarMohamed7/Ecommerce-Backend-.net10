using E_Commerce.Features.Auth;

namespace E_Commerce.Features.Cart;

public class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public List<CartItem> Items { get; set; } = [];
}