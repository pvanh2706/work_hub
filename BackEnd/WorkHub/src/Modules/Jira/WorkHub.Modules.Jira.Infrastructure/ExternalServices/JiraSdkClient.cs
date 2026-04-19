using AtlassianJira = Atlassian.Jira.Jira;
using Atlassian.Jira;
using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.ValueObjects;

namespace WorkHub.Modules.Jira.Infrastructure.ExternalServices;

/// <summary>
/// Implement IJiraClient dùng thư viện Atlassian.SDK thay vì gọi REST API thủ công.
/// Được kích hoạt khi config "Jira:UseSDK" = true.
/// </summary>
internal sealed class JiraSdkClient : IJiraClient
{
    private readonly AtlassianJira _jira;

    // Atlassian.Jira.Jira được tạo từ DI (singleton) — không new trực tiếp
    public JiraSdkClient(AtlassianJira jira) => _jira = jira;

    public async Task<JiraCreatedIssue> CreateIssueAsync(CreateJiraIssueRequest req, CancellationToken ct = default)
    {
        var issue = _jira.CreateIssue(req.ProjectKey);
        issue.Summary = req.Summary;
        issue.Description = req.Description;
        issue.Type = new IssueType(req.IssueTypeId);
        issue.Priority = new IssuePriority(req.PriorityId);

        if (req.AssigneeAccountId is not null)
            issue.Assignee = req.AssigneeAccountId;

        foreach (var label in req.Labels)
            issue.Labels.Add(label);

        // SDK v13: CreateIssueAsync trả về string (issue key), không phải Issue object.
        // Phải fetch lại để lấy JiraIdentifier (numeric ID).
        var createdKey = await _jira.Issues.CreateIssueAsync(issue, ct);
        var createdIssue = await _jira.Issues.GetIssueAsync(createdKey, ct);
        return new JiraCreatedIssue(createdIssue.JiraIdentifier, createdKey, string.Empty);
    }

    public async Task EditIssueAsync(string issueKey, EditJiraIssueRequest req, CancellationToken ct = default)
    {
        var issue = await _jira.Issues.GetIssueAsync(issueKey, ct);

        if (req.Summary is not null)         issue.Summary = req.Summary;
        if (req.Description is not null)     issue.Description = req.Description;
        if (req.PriorityId is not null)      issue.Priority = new IssuePriority(req.PriorityId);
        if (req.AssigneeAccountId is not null) issue.Assignee = req.AssigneeAccountId;

        foreach (var label in req.LabelsToAdd    ?? []) issue.Labels.Add(label);
        foreach (var label in req.LabelsToRemove ?? []) issue.Labels.Remove(label);

        await issue.SaveChangesAsync(ct);
    }

    public async Task<JiraIssueDetail> GetIssueAsync(string issueKey, CancellationToken ct = default)
    {
        var issue = await _jira.Issues.GetIssueAsync(issueKey, ct);

        return new JiraIssueDetail(
            Id:                   issue.JiraIdentifier,
            Key:                  issue.Key.Value,
            Summary:              issue.Summary ?? string.Empty,
            Description:          issue.Description,
            StatusName:           issue.Status.Name,
            StatusCategory:       issue.Status.StatusCategory?.Name ?? string.Empty,
            AssigneeAccountId:    issue.Assignee,
            AssigneeDisplayName:  issue.AssigneeUser?.DisplayName,
            PriorityName:         issue.Priority?.Name ?? "Medium",
            IssueTypeName:        issue.Type?.Name ?? string.Empty,
            OriginalEstimate:     issue.TimeTrackingData?.OriginalEstimate,
            TimeSpent:            issue.TimeTrackingData?.TimeSpent,
            Worklogs:             []);
    }

    public async Task<IReadOnlyList<JiraTransition>> GetTransitionsAsync(string issueKey, CancellationToken ct = default)
    {
        var issue = await _jira.Issues.GetIssueAsync(issueKey, ct);
        var actions = await issue.GetAvailableActionsAsync(ct);

        // SDK trả về IssueTransition với Id và Name
        // ToStatusName/ToStatusCategory không expose qua SDK — để trống, dùng Name là đủ cho UI
        return actions.Select(a => new JiraTransition(
            Id:               a.Id,
            Name:             a.Name,
            ToStatusName:     a.Name,
            ToStatusCategory: string.Empty)).ToList();
    }

    public async Task TransitionIssueAsync(string issueKey, string transitionId, string? comment = null, CancellationToken ct = default)
    {
        var issue = await _jira.Issues.GetIssueAsync(issueKey, ct);

        var updates = comment is not null
            ? new WorkflowTransitionUpdates { Comment = comment }
            : null;

        // SDK nhận transition name — cần resolve từ Id trước
        var actions = await issue.GetAvailableActionsAsync(ct);
        var action = actions.FirstOrDefault(a => a.Id == transitionId)
            ?? throw new InvalidOperationException($"Transition '{transitionId}' không tồn tại trên issue '{issueKey}'.");

        await issue.WorkflowTransitionAsync(action.Name, updates, ct);
    }

    /// <summary>
    /// Override tối ưu: tái sử dụng Issue object đã fetch, tránh round-trip thừa.
    /// Tạo issue rồi chuyển ngay sang Done trong cùng một luồng.
    /// </summary>
    public async Task<JiraCreatedIssue> CreateIssueAndTransitionToDoneAsync(
        CreateJiraIssueRequest req,
        CancellationToken ct = default)
    {
        // Bước 1: Tạo issue
        var issue = _jira.CreateIssue(req.ProjectKey);
        issue.Summary     = req.Summary;
        issue.Description = req.Description;
        issue.Type        = new IssueType(req.IssueTypeId);
        issue.Priority    = new IssuePriority(req.PriorityId);

        if (req.AssigneeAccountId is not null)
            issue.Assignee = req.AssigneeAccountId;

        foreach (var label in req.Labels)
            issue.Labels.Add(label);

        var createdKey   = await _jira.Issues.CreateIssueAsync(issue, ct);
        var createdIssue = await _jira.Issues.GetIssueAsync(createdKey, ct);

        // Bước 2: Tìm và áp dụng transition "Done" ngay trên Issue object vừa fetch
        var actions    = await createdIssue.GetAvailableActionsAsync(ct);
        var doneAction = actions.FirstOrDefault(
            a => a.Name.Equals("Done", StringComparison.OrdinalIgnoreCase));

        if (doneAction is not null)
            await createdIssue.WorkflowTransitionAsync(doneAction.Name, null, ct);

        // Bước 3: Log work nếu được cung cấp
        if (req.WorklogTimeSpent is not null)
        {
            var worklog = new Worklog(
                req.WorklogTimeSpent,
                req.WorklogStarted ?? DateTime.UtcNow,
                req.WorklogComment);

            await createdIssue.AddWorklogAsync(
                worklog,
                WorklogStrategy.AutoAdjustRemainingEstimate,
                null,
                ct);
        }

        return new JiraCreatedIssue(createdIssue.JiraIdentifier, createdKey, string.Empty);
    }
}
