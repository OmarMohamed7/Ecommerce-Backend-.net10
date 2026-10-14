using E_Commerce.Features.Audit;
using E_Commerce.Features.Auth;

namespace E_Commerce.Features.Order;

public class Order : IAuditLoggable
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalPrice { get; set; }
    public decimal Quantity { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}