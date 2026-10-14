namespace E_Commerce.Features.Audit;

/// Marker: changes to entities implementing this are written to the audit log.
public interface IAuditLoggable;

/// Property is left out of audit values (e.g. password hashes).
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotAuditedAttribute : Attribute;