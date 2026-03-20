namespace WorkHub.Modules.Jira.Domain.Enums;

public enum IssueType
{
    Bug = 1,
    Feature = 2,
    Hotfix = 3,
    SupportTicket = 4,
    Refactor = 5,
    TechDebt = 6,
    SubTask = 7
}

public enum IssuePriority
{
    Highest = 1,
    High = 2,
    Medium = 3,
    Low = 4,
    Lowest = 5
}

public enum IssueSyncStatus
{
    PendingCreate = 0,
    Synced = 1,
    PendingUpdate = 2,
    Failed = 3
}
