namespace TeamOps.Application.Tasks;

public sealed record CreateTaskCommand(Guid ProjectId, string Title);