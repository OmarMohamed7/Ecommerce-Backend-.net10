using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace E_Commerce.Features.Audit;

/// Writes an AuditLog row for every added/modified/deleted IAuditLoggable entity.
/// Created rows need the generated key, so they are written right after the save.
public class AuditInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    private readonly List<(EntityEntry Entry, AuditLog Log)> _pendingCreates = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Collect(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Collect(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (TakeCreates(eventData.Context)) eventData.Context!.SaveChanges();
        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        if (TakeCreates(eventData.Context)) await eventData.Context!.SaveChangesAsync(cancellationToken);
        return result;
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingCreates.Clear();
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendingCreates.Clear();
        return Task.CompletedTask;
    }

    private void Collect(DbContext? db)
    {
        if (db is null) return;

        _pendingCreates.Clear();
        var userId = currentUser.Id;

        foreach (var entry in db.ChangeTracker.Entries().Where(e => e.Entity is IAuditLoggable).ToList())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted)) continue;

            var log = new AuditLog
            {
                EntityName = entry.Metadata.ClrType.Name,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    log.Action = AuditActions.Created;
                    _pendingCreates.Add((entry, log)); // key and values are final only after the save
                    continue;

                case EntityState.Deleted:
                    log.Action = AuditActions.Deleted;
                    log.OldValues = Serialize(Values(entry, p => p.OriginalValue));
                    break;

                default:
                    var changed = entry.Properties
                        .Where(p => p.IsModified && IsAudited(p) && !Equals(p.OriginalValue, p.CurrentValue))
                        .ToList();
                    if (changed.Count == 0) continue;

                    log.Action = AuditActions.Updated;
                    log.OldValues = Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                    log.NewValues = Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                    break;
            }

            log.EntityId = KeyOf(entry);
            db.Set<AuditLog>().Add(log);
        }
    }

    private bool TakeCreates(DbContext? db)
    {
        if (db is null || _pendingCreates.Count == 0) return false;

        foreach (var (entry, log) in _pendingCreates)
        {
            log.EntityId = KeyOf(entry);
            log.NewValues = Serialize(Values(entry, p => p.CurrentValue));
            db.Set<AuditLog>().Add(log);
        }

        _pendingCreates.Clear();
        return true;
    }

    private static Dictionary<string, object?> Values(EntityEntry entry, Func<PropertyEntry, object?> pick)
    {
        return entry.Properties.Where(IsAudited).ToDictionary(p => p.Metadata.Name, pick);
    }

    private static bool IsAudited(PropertyEntry p)
    {
        return p.Metadata.PropertyInfo?.GetCustomAttribute<NotAuditedAttribute>() is null;
    }

    private static string KeyOf(EntityEntry entry)
    {
        return string.Join(",", entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue));
    }

    private static string Serialize(Dictionary<string, object?> values)
    {
        return JsonSerializer.Serialize(values);
    }
}