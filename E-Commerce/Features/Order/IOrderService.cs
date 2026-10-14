using E_Commerce.Common;

namespace E_Commerce.Features.Order;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetAllOrdersAsync(OrderDtoQueryParams queryParams);

    Task<OrderDto?> GetOrderByIdAsync(int orderId);

    Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize);

    // Creates an order from the current user's cart and empties the cart. Null when the cart is empty.
    Task<OrderDto?> CheckoutAsync();

    // Order is null when not found; Error is set when the status change is not allowed
    Task<(OrderDto? Order, string? Error)> UpdateOrderStatusAsync(int orderId, OrderStatus status);
}