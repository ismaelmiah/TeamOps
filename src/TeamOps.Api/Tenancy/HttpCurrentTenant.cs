using System.Security.Claims;
using TeamOps.Application.Common;

namespace TeamOps.Api.Tenancy;

public sealed class HttpCurrentTenant(IHttpContextAccessor httpContextAccessor) : ICurrentTenant
{
    public Guid TenantId
    {
        get
        {
            var value = httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue("tenant_id");

            if (!Guid.TryParse(value, out var tenantId))
                throw new InvalidOperationException(
                    "Tenant ID is missing from the current user.");

            return tenantId;
        }
    }
}