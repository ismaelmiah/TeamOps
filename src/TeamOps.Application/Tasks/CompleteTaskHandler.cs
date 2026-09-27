namespace TeamOps.Application.Tasks;

public sealed class CompleteTaskHandler
{
    public Task Handle(CompleteTaskCommand command)
    {
        command.Task.Complete();

        return Task.CompletedTask;
    }
}