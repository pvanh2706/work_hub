using WorkHub.Modules.Jira.Domain.ValueObjects;

namespace WorkHub.Modules.Jira.Application.Abstractions;

/// <summary>
/// Interface giao tiếp với Jira REST API v2.
/// Infrastructure implement bằng HttpClient.
/// Application chỉ biết interface này, không biết HTTP.
/// </summary>
public interface IJiraClient
{
    /// <summary>POST /rest/api/2/issue — Tạo issue, trả về { id, key }</summary>
    Task<JiraCreatedIssue> CreateIssueAsync(CreateJiraIssueRequest request, CancellationToken ct = default);

    /// <summary>PUT /rest/api/2/issue/{key} — Cập nhật fields của issue</summary>
    Task EditIssueAsync(string issueKey, EditJiraIssueRequest request, CancellationToken ct = default);

    /// <summary>GET /rest/api/2/issue/{key} — Lấy chi tiết issue</summary>
    Task<JiraIssueDetail> GetIssueAsync(string issueKey, CancellationToken ct = default);

    /// <summary>GET /rest/api/2/issue/{key}/transitions — Lấy danh sách transition khả dụng</summary>
    Task<IReadOnlyList<JiraTransition>> GetTransitionsAsync(string issueKey, CancellationToken ct = default);

    /// <summary>POST /rest/api/2/issue/{key}/transitions — Chuyển trạng thái issue</summary>
    Task TransitionIssueAsync(string issueKey, string transitionId, string? comment = null, CancellationToken ct = default);
}

public record CreateJiraIssueRequest(
    string ProjectKey,
    string Summary,
    string Description,
    string IssueTypeId,
    string PriorityId,
    string? AssigneeAccountId,
    IReadOnlyList<string> Labels);

public record EditJiraIssueRequest(
    string? Summary,
    string? Description,
    string? PriorityId,
    string? AssigneeAccountId,
    IReadOnlyList<string>? LabelsToAdd,
    IReadOnlyList<string>? LabelsToRemove);
