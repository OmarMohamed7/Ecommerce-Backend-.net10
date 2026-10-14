namespace E_Commerce.Features.Audit;

public static class AuditActions
{
    public const string Created = "Created";
    public const string Updated = "Updated";
    public const string Deleted = "Deleted";
}

public class AuditLog
{
    public long Id { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    // Null for anonymous requests and background work (e.g. the seeder)
    public string? UserId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // JSON of only the properties that changed (all properties for Created/Deleted)
    public string? OldValues { get; set; }

    public string? NewValues { get; set; }
}