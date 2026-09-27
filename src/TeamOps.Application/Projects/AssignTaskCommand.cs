using TeamOps.Domain.Projects;
using TeamOps.Domain.Tasks;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Projects;

public sealed record AssignTaskCommand(Project Project, TaskItem Task, User User);