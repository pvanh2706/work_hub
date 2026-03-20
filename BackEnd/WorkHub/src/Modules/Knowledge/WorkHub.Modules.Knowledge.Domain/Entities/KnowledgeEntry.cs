using WorkHub.Shared;

namespace WorkHub.Modules.Knowledge.Domain.Entities;

public class KnowledgeEntry : AuditableEntity
{
    public Guid NodeId { get; private set; }
    public string IssueTitle { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string RootCause { get; private set; } = default!;
    public string Fix { get; private set; } = default!;
    public string? FixVersion { get; private set; }
    public string? JiraIssueKey { get; private set; }
    public int HelpfulVotes { get; private set; }

    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private KnowledgeEntry() { }

    public static KnowledgeEntry Create(
        Guid nodeId,
        string issueTitle,
        string description,
        string rootCause,
        string fix,
        string? fixVersion,
        string? jiraIssueKey,
        IEnumerable<string> tags,
        Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issueTitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(rootCause);
        ArgumentException.ThrowIfNullOrWhiteSpace(fix);

        var entry = new KnowledgeEntry
        {
            NodeId = nodeId,
            IssueTitle = issueTitle.Trim(),
            Description = description.Trim(),
            RootCause = rootCause.Trim(),
            Fix = fix.Trim(),
            FixVersion = fixVersion?.Trim(),
            JiraIssueKey = jiraIssueKey?.Trim().ToUpperInvariant(),
            CreatedBy = createdBy
        };
        entry._tags.AddRange(tags.Select(t => t.Trim().ToLowerInvariant()).Distinct());
        return entry;
    }

    public void UpdateFix(string newFix, string? newVersion, Guid updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newFix);
        Fix = newFix.Trim();
        FixVersion = newVersion?.Trim();
        SetUpdated(updatedBy);
    }

    public void MarkHelpful() => HelpfulVotes++;
}
