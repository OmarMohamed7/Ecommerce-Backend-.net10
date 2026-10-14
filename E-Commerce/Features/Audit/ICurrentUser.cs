using System.Security.Claims;

namespace E_Commerce.Features.Audit;

public interface ICurrentUser
{
    string? Id { get; }

    bool IsInRole(string role);
}

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsInRole(string role)
    {
        return accessor.HttpContext?.User.IsInRole(role) ?? false;
    }
}