using E_Commerce.Common;
using E_Commerce.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Features.Audit;

[Route("api/audit-logs")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class AuditController(IAuditService auditService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] AuditQueryParams queryParams)
    {
        return Ok(ApiResponse<PagedResult<AuditLogDto>>.Ok(await auditService.GetLogs(queryParams)));
    }
}