using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.TransitionIssue;

public record TransitionIssueCommand(
    string JiraIssueKey,
    string TransitionId,
    Guid UserId,
    string? Comment
) : IRequest<Result>;
