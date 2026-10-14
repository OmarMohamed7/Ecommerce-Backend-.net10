namespace E_Commerce.Features.Audit;

public class AuditLogDto
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
}

public class AuditQueryParams
{
    private const int MaxPageSize = 100;
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

    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? UserId { get; set; }
    public string? Action { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}