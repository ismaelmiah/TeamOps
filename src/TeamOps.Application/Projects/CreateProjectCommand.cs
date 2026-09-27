namespace TeamOps.Application.Projects;

public sealed record CreateProjectCommand(Guid TenantId, string Name);