using WorkHub.Modules.Jira.Domain.Enums;
using WorkHub.Shared;

namespace WorkHub.Modules.Jira.Domain.Entities;

/// <summary>
/// Lưu trạng thái sync với Jira. Đây là bản ghi nội bộ,
/// không phải bản sao toàn bộ dữ liệu từ Jira.
/// </summary>
public class JiraIssueSync : AuditableEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }

    // Jira identifiers
    public string JiraProjectKey { get; private set; } = default!;
    public string? JiraIssueId { get; private set; }
    public string? JiraIssueKey { get; private set; }

    // Issue metadata
    public string Summary { get; private set; } = default!;
    public IssueType IssueType { get; private set; }
    public IssuePriority Priority { get; private set; }
    public IssueSyncStatus SyncStatus { get; private set; }
    public string? LastSyncError { get; private set; }
    public DateTime? LastSyncedAt { get; private set; }

    private JiraIssueSync() { }

    public static JiraIssueSync CreatePending(
        Guid organizationId,
        Guid userId,
        string jiraProjectKey,
        string summary,
        IssueType issueType,
        IssuePriority priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraProjectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(summary);

        return new JiraIssueSync
        {
            OrganizationId = organizationId,
            UserId = userId,
            JiraProjectKey = jiraProjectKey.ToUpperInvariant().Trim(),
            Summary = summary.Trim(),
            IssueType = issueType,
            Priority = priority,
            SyncStatus = IssueSyncStatus.PendingCreate,
            CreatedBy = userId
        };
    }

    public void MarkSynced(string jiraIssueId, string jiraIssueKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraIssueId);
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraIssueKey);

        JiraIssueId = jiraIssueId;
        JiraIssueKey = jiraIssueKey.ToUpperInvariant();
        SyncStatus = IssueSyncStatus.Synced;
        LastSyncedAt = DateTime.UtcNow;
        LastSyncError = null;
    }

    public void MarkFailed(string error)
    {
        SyncStatus = IssueSyncStatus.Failed;
        LastSyncError = error;
    }

    public void MarkPendingUpdate()
    {
        if (SyncStatus != IssueSyncStatus.Synced)
            return;
        SyncStatus = IssueSyncStatus.PendingUpdate;
    }
}
