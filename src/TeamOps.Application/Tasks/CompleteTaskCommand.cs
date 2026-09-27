using TeamOps.Domain.Tasks;

namespace TeamOps.Application.Tasks;

public sealed record CompleteTaskCommand(TaskItem Task);