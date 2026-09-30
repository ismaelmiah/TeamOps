namespace TeamOps.Application.Projects;

public sealed record RemoveProjectMemberCommand(Guid ProjectId, Guid UserId);