namespace E_Commerce.DTOs;

public class ProductQueryParams
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

    public string Search { get; set; } = string.Empty;

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    public string SortBy { get; set; } = string.Empty;

    public bool Descending { get; set; } = false;
}