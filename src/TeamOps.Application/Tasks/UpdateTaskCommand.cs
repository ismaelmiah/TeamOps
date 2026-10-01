namespace TeamOps.Application.Tasks;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title);