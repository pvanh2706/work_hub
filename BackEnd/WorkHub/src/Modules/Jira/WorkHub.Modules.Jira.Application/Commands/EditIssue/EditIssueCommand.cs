using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.EditIssue;

public record EditIssueCommand(
    string JiraIssueKey,
    Guid UserId,
    string? Summary,
    string? Description,
    string? PriorityId,
    string? AssigneeAccountId,
    List<string>? LabelsToAdd,
    List<string>? LabelsToRemove
) : IRequest<Result>;
