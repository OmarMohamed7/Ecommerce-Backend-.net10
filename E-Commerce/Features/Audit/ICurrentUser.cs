using System.Security.Claims;

namespace E_Commerce.Features.Audit;

public interface ICurrentUser
{
    string? Id { get; }
}

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}