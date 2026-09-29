namespace TeamOps.Application.Users;

public sealed record CreateUserCommand(
    Guid TenantId,
    string Email,
    string Name,
    UserRole Role);