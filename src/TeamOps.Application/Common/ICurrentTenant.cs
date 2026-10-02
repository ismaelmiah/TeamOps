namespace TeamOps.Application.Common;

public interface ICurrentTenant
{
    Guid TenantId { get; }
}