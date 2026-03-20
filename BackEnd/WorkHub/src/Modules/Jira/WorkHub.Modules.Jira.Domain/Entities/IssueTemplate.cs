using WorkHub.Shared;

namespace WorkHub.Modules.Jira.Domain.Entities;

/// <summary>
/// Template để tạo issue nhanh. Lưu trữ nội bộ, không sync với Jira.
/// </summary>
public class IssueTemplate : AuditableEntity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = default!;
    public string IssueTypeName { get; private set; } = default!;
    public string SummaryPrefix { get; private set; } = default!;
    public string DefaultDescription { get; private set; } = default!;
    public string? DefaultLabels { get; private set; }
    public string? DefaultPriorityId { get; private set; }
    public bool IsDefault { get; private set; }

    private IssueTemplate() { }

    public static IssueTemplate Create(
        Guid organizationId,
        string name,
        string issueTypeName,
        string summaryPrefix,
        string defaultDescription,
        Guid createdBy,
        string? defaultLabels = null,
        string? defaultPriorityId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(issueTypeName);

        return new IssueTemplate
        {
            OrganizationId = organizationId,
            Name = name.Trim(),
            IssueTypeName = issueTypeName.Trim(),
            SummaryPrefix = summaryPrefix.Trim(),
            DefaultDescription = defaultDescription.Trim(),
            DefaultLabels = defaultLabels,
            DefaultPriorityId = defaultPriorityId,
            IsDefault = false,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string summaryPrefix, string defaultDescription)
    {
        Name = name.Trim();
        SummaryPrefix = summaryPrefix.Trim();
        DefaultDescription = defaultDescription.Trim();
    }

    public void SetAsDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;
}
