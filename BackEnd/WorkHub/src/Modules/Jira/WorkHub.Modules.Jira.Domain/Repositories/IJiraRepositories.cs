using WorkHub.Modules.Jira.Domain.Entities;
using WorkHub.Shared.Abstractions;

namespace WorkHub.Modules.Jira.Domain.Repositories;

public interface IJiraIssueSyncRepository : IRepository<JiraIssueSync>
{
    Task<JiraIssueSync?> FindByJiraKeyAsync(string jiraIssueKey, CancellationToken ct = default);
    new Task AddAsync(JiraIssueSync entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IIssueTemplateRepository : IRepository<IssueTemplate>
{
    Task<IReadOnlyList<IssueTemplate>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<IssueTemplate?> GetDefaultAsync(Guid organizationId, string issueTypeName, CancellationToken ct = default);
    new Task AddAsync(IssueTemplate entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
