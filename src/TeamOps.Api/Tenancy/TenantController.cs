using Microsoft.AspNetCore.Mvc;
using TeamOps.Application.Common;

namespace TeamOps.Api.Tenancy;

[ApiController]
[Route("api/tenant")]
public sealed class TenantController(ICurrentTenant currentTenant) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            tenantId = currentTenant.TenantId
        });
    }
}