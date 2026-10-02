namespace TeamOps.Application.Users;

public sealed class DeleteUserHandler(IUserRepository repository)
{
    public async Task Handle(Guid userId)
    {
        var user = await repository.GetByIdAsync(userId);

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        await repository.DeleteAsync(user);
    }
}