using FluentValidation;

namespace E_Commerce.Features.Order;

public class OrderDto
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus OrderStatus { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];

    public decimal TotalQuantity { get; set; }

    public decimal TotalAmount { get; set; }
}

internal class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(o => o.Items)
            .NotEmpty();
    }
}

public class OrderDtoQueryParams
{
    private const int MaxPageSize = 50;

    private int _page = 1;
    private int _pageSize = 20;

    public int Page
    {
        get => _page;
        set => _page = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public string SortBy { get; set; } = string.Empty;

    public OrderStatus? OrderStatus { get; set; }

    public bool Descending { get; set; } = false;
}