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

    /// <summary>
    /// Tạo issue rồi tự động chuyển sang trạng thái Done nếu tạo thành công.
    /// Implementation mặc định: gọi CreateIssueAsync → GetTransitionsAsync → TransitionIssueAsync.
    /// Các class implement có thể override để tối ưu hơn.
    /// </summary>
    async Task<JiraCreatedIssue> CreateIssueAndTransitionToDoneAsync(
        CreateJiraIssueRequest request,
        CancellationToken ct = default)
    {
        var created = await CreateIssueAsync(request, ct);

        var transitions = await GetTransitionsAsync(created.Key, ct);
        var done = transitions.FirstOrDefault(
            t => t.Name.Equals("Done", StringComparison.OrdinalIgnoreCase));

        if (done is not null)
            await TransitionIssueAsync(created.Key, done.Id, null, ct);

        return created;
    }
}

public record CreateJiraIssueRequest(
    string ProjectKey,
    string Summary,
    string Description,
    string IssueTypeId,
    string PriorityId,
    string? AssigneeAccountId,
    IReadOnlyList<string> Labels,
    string? WorklogTimeSpent = null,
    string? WorklogComment = null,
    DateTime? WorklogStarted = null);

public record EditJiraIssueRequest(
    string? Summary,
    string? Description,
    string? PriorityId,
    string? AssigneeAccountId,
    IReadOnlyList<string>? LabelsToAdd,
    IReadOnlyList<string>? LabelsToRemove);
