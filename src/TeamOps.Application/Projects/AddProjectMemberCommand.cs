using TeamOps.Domain.Projects;
using TeamOps.Domain.Users;

namespace TeamOps.Application.Projects;

public sealed record AddProjectMemberCommand(Project Project, User User);