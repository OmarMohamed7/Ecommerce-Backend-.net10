using E_Commerce.Common;
using E_Commerce.Db;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Features.Audit;

public interface IAuditService
{
    Task<PagedResult<AuditLogDto>> GetLogs(AuditQueryParams q);
}

public class AuditService(AppDbContext db) : IAuditService
{
    public async Task<PagedResult<AuditLogDto>> GetLogs(AuditQueryParams q)
    {
        var query = db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q.EntityName)) query = query.Where(a => a.EntityName == q.EntityName);
        if (!string.IsNullOrWhiteSpace(q.EntityId)) query = query.Where(a => a.EntityId == q.EntityId);
        if (!string.IsNullOrWhiteSpace(q.UserId)) query = query.Where(a => a.UserId == q.UserId);
        if (!string.IsNullOrWhiteSpace(q.Action)) query = query.Where(a => a.Action == q.Action);
        if (q.From.HasValue) query = query.Where(a => a.Timestamp >= q.From.Value.ToUniversalTime());
        if (q.To.HasValue) query = query.Where(a => a.Timestamp <= q.To.Value.ToUniversalTime());

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Timestamp).ThenByDescending(a => a.Id)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                UserId = a.UserId,
                Timestamp = a.Timestamp,
                OldValues = a.OldValues,
                NewValues = a.NewValues
            })
            .ToListAsync();

        return new PagedResult<AuditLogDto>(items, total, q.Page, q.PageSize);
    }
}