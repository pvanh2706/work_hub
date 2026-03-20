using WorkHub.Modules.Jira.Domain.Enums;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.CreateIssue;

public record CreateIssueCommand(
    Guid OrganizationId,
    Guid UserId,
    string ProjectKey,
    string Summary,
    string Description,
    IssueType IssueType,
    IssuePriority Priority,
    string IssueTypeId,
    string PriorityId,
    string? AssigneeAccountId,
    List<string> Labels
) : IRequest<Result<CreateIssueResult>>;

public record CreateIssueResult(
    Guid SyncId,
    string JiraIssueId,
    string JiraIssueKey);
