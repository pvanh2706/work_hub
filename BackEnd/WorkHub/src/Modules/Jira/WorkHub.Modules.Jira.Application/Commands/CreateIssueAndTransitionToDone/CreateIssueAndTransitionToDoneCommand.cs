using WorkHub.Modules.Jira.Domain.Enums;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.CreateIssueAndTransitionToDone;

/// <summary>
/// Tạo Jira issue rồi tự động chuyển trạng thái sang "Done" trong cùng một lệnh.
/// Sử dụng <see cref="Application.Abstractions.IJiraClient.CreateIssueAndTransitionToDoneAsync"/>.
/// </summary>
public record CreateIssueAndTransitionToDoneCommand(
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
    List<string> Labels,
    string? WorklogTimeSpent = null,
    string? WorklogComment = null,
    DateTime? WorklogStarted = null
) : IRequest<Result<CreateIssueAndTransitionToDoneResult>>;

public record CreateIssueAndTransitionToDoneResult(
    Guid SyncId,
    string JiraIssueId,
    string JiraIssueKey);
