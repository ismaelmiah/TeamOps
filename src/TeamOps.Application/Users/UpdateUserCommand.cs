using TeamOps.Domain.Users;

namespace TeamOps.Application.Users;

public sealed record UpdateUserCommand(
    string Email,
    string Name,
    UserRole Role);