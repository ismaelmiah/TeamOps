using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests;

internal sealed record ProjectResponse(
        Guid Id,
        Guid TenantId,
        string Name);


internal sealed record UserResponse(
        Guid Id,
        Guid TenantId,
        string Email,
        string Name,
        UserRole Role);

internal sealed record TaskResponse(
        Guid Id,
        Guid ProjectId,
        string Title);