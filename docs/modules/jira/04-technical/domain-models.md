# Domain Models - Jira Module

Entity and value object implementations following DDD and Clean Architecture.

## Entities

### IssueTemplate

**Purpose:** Represents an issue creation template

```csharp
public class IssueTemplate : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public IssueType Type { get; private set; }
    public string? Description { get; private set; }
    public TemplateFields Fields { get; private set; } = default!;
    public TemplateVisibility Visibility { get; private set; }
    public int UsageCount { get; private set; }
    public bool IsActive { get; private set; }
    
    // Private constructor - Cannot instantiate directly
    private IssueTemplate() { }
    
    // Factory method - Only way to create
    public static IssueTemplate Create(
        string name,
        IssueType type,
        TemplateFields fields,
        TemplateVisibility visibility,
        Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(fields);
        
        if (name.Length > 200)
            throw new ArgumentException(\"Template name cannot exceed 200 characters\");
        
        var template = new IssueTemplate
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Fields = fields,
            Visibility = visibility,
            UsageCount = 0,
            IsActive = true
        };
        
        template.SetCreated(createdBy);
        
        return template;
    }
    
    // Domain methods
    public void UpdateName(string newName, Guid updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        
        if (newName.Length > 200)
            throw new ArgumentException(\"Name too long\");
        
        Name = newName;
        SetUpdated(updatedBy);
    }
    
    public void UpdateFields(TemplateFields newFields, Guid updatedBy)
    {
        ArgumentNullException.ThrowIfNull(newFields);
        
        Fields = newFields;
        SetUpdated(updatedBy);
    }
    
    public void IncrementUsage()
    {
        UsageCount++;
    }
    
    public void Archive(Guid updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }
    
    public void Restore(Guid updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }
}
```

### JiraIssueSync

**Purpose:** Tracks Jira issue sync state

```csharp
public class JiraIssueSync : AuditableEntity
{
    public JiraKey JiraKey { get; private set; } = default!;
    public string JiraId { get; private set; } = default!;
    public string ProjectKey { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public IssueType Type { get; private set; }
    public IssuePriority? Priority { get; private set; }
    public string Status { get; private set; } = default!;
    public Guid? AssigneeId { get; private set; }
    public Guid ReporterId { get; private set; }
    public List<string> Labels { get; private set; } = new();
    public List<string> Components { get; private set; } = new();
    public string? SprintId { get; private set; }
    public decimal? StoryPoints { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime? LastSyncedAt { get; private set; }
    public string? SyncErrorMessage { get; private set; }
    public string JiraUrl { get; private set; } = default!;
    
    private JiraIssueSync() { }
    
    public static JiraIssueSync Create(
        string jiraKey,
        string jiraId,
        string projectKey,
        string title,
        IssueType type,
        Guid reporterId,
        string jiraUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jiraKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        
        var issue = new JiraIssueSync
        {
            Id = Guid.NewGuid(),
            JiraKey = JiraKey.Create(jiraKey),
            JiraId = jiraId,
            ProjectKey = projectKey,
            Title = title,
            Type = type,
            Status = \"To Do\",
            ReporterId = reporterId,
            SyncStatus = SyncStatus.Synced,
            LastSyncedAt = DateTime.UtcNow,
            JiraUrl = jiraUrl
        };
        
        issue.SetCreated(reporterId);
        
        return issue;
    }
    
    // Domain methods
    public void UpdateFromJira(
        string title,
        string? description,
        string status,
        IssuePriority? priority,
        Guid? assigneeId)
    {
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        AssigneeId = assigneeId;
        
        MarkSynced();
    }
    
    public void MarkSynced()
    {
        SyncStatus = SyncStatus.Synced;
        LastSyncedAt = DateTime.UtcNow;
        SyncErrorMessage = null;
    }
    
    public void MarkPending()
    {
        SyncStatus = SyncStatus.Pending;
    }
    
    public void MarkFailed(string errorMessage)
    {
        SyncStatus = SyncStatus.Failed;
        SyncErrorMessage = errorMessage;
    }
    
    public void AddLabel(string label)
    {
        if (!Labels.Contains(label))
            Labels.Add(label);
    }
    
    public void RemoveLabel(string label)
    {
        Labels.Remove(label);
    }
}
```

### WorkLogEntry

```csharp
public class WorkLogEntry : AuditableEntity
{
    public JiraKey IssueKey { get; private set; } = default!;
    public string? JiraWorklogId { get; private set; }
    public int TimeSpentSeconds { get; private set; }
    public DateTime StartedAt { get; private set; }
    public Guid AuthorId { get; private set; }
    public string? Comment { get; private set; }
    public WorkLogSource Source { get; private set; }
    public string? CommitHash { get; private set; }
    public bool SyncedToJira { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    
    private WorkLogEntry() { }
    
    public static WorkLogEntry Create(
        JiraKey issueKey,
        int timeSpentSeconds,
        DateTime startedAt,
        Guid authorId,
        WorkLogSource source,
        string? comment = null,
        string? commitHash = null)
    {
        ArgumentNullException.ThrowIfNull(issueKey);
        
        if (timeSpentSeconds <= 0)
            throw new ArgumentException(\"Time must be positive\");
        
        var entry = new WorkLogEntry
        {
            Id = Guid.NewGuid(),
            IssueKey = issueKey,
            TimeSpentSeconds = timeSpentSeconds,
            StartedAt = startedAt,
            AuthorId = authorId,
            Source = source,
            Comment = comment,
            CommitHash = commitHash,
            SyncedToJira = false
        };
        
        entry.SetCreated(authorId);
        
        return entry;
    }
    
    public void MarkSyncedToJira(string jiraWorklogId)
    {
        JiraWorklogId = jiraWorklogId;
        SyncedToJira = true;
        SyncedAt = DateTime.UtcNow;
    }
    
    public void UpdateTime(int newTimeSpentSeconds, Guid updatedBy)
    {
        if (newTimeSpentSeconds <= 0)
            throw new ArgumentException(\"Time must be positive\");
        
        TimeSpentSeconds = newTimeSpentSeconds;
        SetUpdated(updatedBy);
    }
}
```

## Value Objects

### JiraKey

```csharp
public sealed record JiraKey
{
    public string Value { get; }
    
    private JiraKey(string value)
    {
        Value = value;
    }
    
    public static JiraKey Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        // Validate format: PROJ-123
        if (!Regex.IsMatch(value, @\"^[A-Z]+-\d+$\"))
            throw new ArgumentException($\"Invalid Jira key format: {value}\");
        
        return new JiraKey(value);
    }
    
    public override string ToString() => Value;
    
    public static implicit operator string(JiraKey key) => key.Value;
}
```

### TemplateFields

```csharp
public sealed record TemplateFields
{
    public IssuePriority? DefaultPriority { get; init; }
    public List<string> DefaultLabels { get; init; } = new();
    public List<string> DefaultComponents { get; init; } = new();
    public string? DescriptionTemplate { get; init; }
    public Dictionary<string, string> CustomFields { get; init; } = new();
    
    public static TemplateFields Create(
        IssuePriority? priority = null,
        IEnumerable<string>? labels = null,
        IEnumerable<string>? components = null,
        string? descriptionTemplate = null,
        Dictionary<string, string>? customFields = null)
    {
        return new TemplateFields
        {
            DefaultPriority = priority,
            DefaultLabels = labels?.ToList() ?? new(),
            DefaultComponents = components?.ToList() ?? new(),
            DescriptionTemplate = descriptionTemplate,
            CustomFields = customFields ?? new()
        };
    }
}
```

## Enums

### IssueType

```csharp
public enum IssueType
{
    Bug = 1,
    Feature = 2,
    Task = 3,
    Story = 4,
    Epic = 5,
    Subtask = 6
}
```

### IssuePriority

```csharp
public enum IssuePriority
{
    Lowest = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    Highest = 5
}
```

### SyncStatus

```csharp
public enum SyncStatus
{
    Synced = 1,
    Pending = 2,
    Failed = 3
}
```

### TemplateVisibility

```csharp
public enum TemplateVisibility
{
    Personal = 1,
    Team = 2,
    Organization = 3
}
```

### WorkLogSource

```csharp
public enum WorkLogSource
{
    Manual = 1,
    Git = 2,
    Timer = 3
}
```

## Domain Events

### IssueCreatedEvent

```csharp
public sealed record IssueCreatedEvent(
    Guid IssueId,
    string JiraKey,
    string Title,
    IssueType Type,
    Guid CreatedBy,
    DateTime CreatedAt
) : IDomainEvent;
```

### IssueSyncedEvent

```csharp
public sealed record IssueSyncedEvent(
    Guid IssueId,
    string JiraKey,
    SyncDirection Direction,
    DateTime SyncedAt
) : IDomainEvent;

public enum SyncDirection
{
    ToJira,
    FromJira
}
```

### TemplateCreatedEvent

```csharp
public sealed record TemplateCreatedEvent(
    Guid TemplateId,
    string Name,
    IssueType Type,
    Guid CreatedBy
) : IDomainEvent;
```

### WorkLoggedEvent

```csharp
public sealed record WorkLoggedEvent(
    Guid WorkLogId,
    string IssueKey,
    int TimeSpentSeconds,
    WorkLogSource Source,
    Guid AuthorId
) : IDomainEvent;
```

## Base Classes

### AuditableEntity

```csharp
public abstract class AuditableEntity
{
    public Guid Id { get; protected set; }
    public Guid CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    
    protected void SetCreated(Guid userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }
    
    protected void SetUpdated(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

## Usage Examples

### Creating an Issue

```csharp
// 1. Get template
var template = await _templateRepository.GetByType(IssueType.Bug, ct);

// 2. Create issue sync entity
var issue = JiraIssueSync.Create(
    jiraKey: \"PROJ-123\",
    jiraId: \"10001\",
    projectKey: \"PROJ\",
    title: \"Login bug\",
    type: IssueType.Bug,
    reporterId: currentUserId,
    jiraUrl: \"https://company.atlassian.net/browse/PROJ-123\"
);

// 3. Apply template defaults
issue.UpdateFromJira(
    title: \"Login bug\",
    description: template.Fields.DescriptionTemplate,
    status: \"To Do\",
    priority: template.Fields.DefaultPriority,
    assigneeId: null
);

// 4. Add labels from template
foreach (var label in template.Fields.DefaultLabels)
{
    issue.AddLabel(label);
}

// 5. Save
await _repository.AddAsync(issue, ct);
```

### Logging Work

```csharp
// From Git commit
var workLog = WorkLogEntry.Create(
    issueKey: JiraKey.Create(\"PROJ-123\"),
    timeSpentSeconds: 1800, // 30 minutes
    startedAt: DateTime.UtcNow,
    authorId: currentUserId,
    source: WorkLogSource.Git,
    comment: \"Implemented fix\",
    commitHash: \"abc123\"
);

await _workLogRepository.AddAsync(workLog, ct);
```

---

**Last Updated:** 2026-03-22
