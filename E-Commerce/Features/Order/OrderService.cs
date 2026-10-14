using System.Linq.Expressions;
using E_Commerce.Common;
using E_Commerce.Db;
using E_Commerce.Features.Audit;
using E_Commerce.Features.Auth;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Features.Order;

public class OrderService(AppDbContext db, ICurrentUser currentUser) : IOrderService
{
    // Single mapping used for queries (translated to SQL) and for in-memory orders
    private static readonly Expression<Func<Order, OrderDto>> ToDtoExpression = o => new OrderDto
    {
        Id = o.Id,
        OrderDate = o.CreatedAt,
        OrderStatus = o.Status,
        TotalAmount = o.TotalPrice,
        TotalQuantity = o.Quantity,
        Items = o.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };

    private static readonly Func<Order, OrderDto> ToDto = ToDtoExpression.Compile();


    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Paid, OrderStatus.Cancelled, OrderStatus.Rejected],
        [OrderStatus.Paid] = [OrderStatus.Shipped, OrderStatus.Cancelled],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [OrderStatus.Returned]
    };

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        var query = db.Orders.AsNoTracking().Where(o => o.Id == orderId);

        // Admins can read any order. Everyone else only their own; fails closed
        // because CurrentUserId() throws when there is no authenticated user.
        if (!currentUser.IsInRole(Roles.Admin))
        {
            var userId = CurrentUserId();
            query = query.Where(o => o.UserId == userId);
        }

        return await query.Select(ToDtoExpression).FirstOrDefaultAsync();
    }

    public async Task<OrderDto?> CheckoutAsync()
    {
        var userId = CurrentUserId();

        var cart = await db.Cart
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || cart.Items.Count == 0) return null;

        // Snapshot name and price from the database, never from the client
        var items = cart.Items.Select(item => new OrderItem
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.Product.Price
        }).ToList();

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            Items = items,
            // OrderItem.TotalPrice is computed by the database, so calculate it here
            TotalPrice = items.Sum(i => i.Quantity * i.UnitPrice),
            Quantity = items.Sum(i => i.Quantity)
        };

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items); // one SaveChanges: order created and cart emptied together
        await db.SaveChangesAsync();

        return ToDto(order);
    }

    public async Task<(OrderDto? Order, string? Error)> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return (null, null);

        if (!AllowedTransitions.TryGetValue(order.Status, out var allowed) || !allowed.Contains(status))
            return (null, $"Cannot change an order from {order.Status} to {status}.");

        order.Status = status;
        await db.SaveChangesAsync();

        return (ToDto(order), null);
    }

    public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(OrderDtoQueryParams queryParams)
    {
        var query = db.Orders.AsNoTracking();

        // 1. Filter by price
        if (queryParams.MinPrice.HasValue) query = query.Where(o => o.TotalPrice >= queryParams.MinPrice.Value);
        if (queryParams.MaxPrice.HasValue) query = query.Where(o => o.TotalPrice <= queryParams.MaxPrice.Value);

        // 2. Filter by quantity
        if (queryParams.MinQuantity.HasValue) query = query.Where(o => o.Quantity >= queryParams.MinQuantity.Value);
        if (queryParams.MaxQuantity.HasValue) query = query.Where(o => o.Quantity <= queryParams.MaxQuantity.Value);

        // 3. Filter by status (only when provided)
        if (queryParams.OrderStatus.HasValue) query = query.Where(o => o.Status == queryParams.OrderStatus.Value);

        var totalCount = await query.CountAsync();

        // 4. Sorting, with Id as the tie-breaker so pages stay stable
        var ordered = queryParams.SortBy.ToLowerInvariant() switch
        {
            "price" => queryParams.Descending
                ? query.OrderByDescending(o => o.TotalPrice).ThenBy(o => o.Id)
                : query.OrderBy(o => o.TotalPrice).ThenBy(o => o.Id),

            "quantity" => queryParams.Descending
                ? query.OrderByDescending(o => o.Quantity).ThenBy(o => o.Id)
                : query.OrderBy(o => o.Quantity).ThenBy(o => o.Id),

            "status" or "orderstatus" => queryParams.Descending
                ? query.OrderByDescending(o => o.Status).ThenBy(o => o.Id)
                : query.OrderBy(o => o.Status).ThenBy(o => o.Id),

            _ => queryParams.Descending
                ? query.OrderByDescending(o => o.Id)
                : query.OrderBy(o => o.Id)
        };

        // 5. Page, then project to DTOs
        var orders = await ordered
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(ToDtoExpression)
            .ToListAsync();

        return new PagedResult<OrderDto>(orders, totalCount, queryParams.Page, queryParams.PageSize);
    }

    public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize)
    {
        var userId = CurrentUserId();

        var query = db.Orders.AsNoTracking().Where(o => o.UserId == userId);

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDtoExpression)
            .ToListAsync();

        return new PagedResult<OrderDto>(orders, totalCount, page, pageSize);
    }

    private int CurrentUserId()
    {
        return int.TryParse(currentUser.Id, out var id)
            ? id
            : throw new UnauthorizedAccessException("User is not authenticated.");
    }
}