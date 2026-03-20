namespace WorkHub.Modules.Jira.Domain.ValueObjects;

/// <summary>Kết quả trả về sau khi tạo issue trên Jira.</summary>
public record JiraCreatedIssue(string Id, string Key, string Self);

/// <summary>Transition có thể thực hiện từ trạng thái hiện tại.</summary>
public record JiraTransition(string Id, string Name, string ToStatusName, string ToStatusCategory);

/// <summary>Chi tiết issue từ Jira API.</summary>
public record JiraIssueDetail(
    string Id,
    string Key,
    string Summary,
    string? Description,
    string StatusName,
    string StatusCategory,
    string? AssigneeAccountId,
    string? AssigneeDisplayName,
    string PriorityName,
    string IssueTypeName,
    string? OriginalEstimate,
    string? TimeSpent,
    IReadOnlyList<JiraWorklogEntry> Worklogs);

public record JiraWorklogEntry(
    string Id,
    string AuthorAccountId,
    string AuthorDisplayName,
    string TimeSpent,
    int TimeSpentSeconds,
    DateTime Started);
