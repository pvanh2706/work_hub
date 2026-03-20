using WorkHub.Modules.Jira.Domain.Entities;
using WorkHub.Modules.Jira.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Modules.Jira.Infrastructure.Repositories;

internal sealed class JiraIssueSyncRepository : IJiraIssueSyncRepository
{
    private readonly JiraDbContext _db;
    public JiraIssueSyncRepository(JiraDbContext db) => _db = db;

    public async Task<JiraIssueSync?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.IssuesSyncs.FindAsync([id], ct);

    public async Task<JiraIssueSync?> FindByJiraKeyAsync(string jiraIssueKey, CancellationToken ct = default)
        => await _db.IssuesSyncs.FirstOrDefaultAsync(x => x.JiraIssueKey == jiraIssueKey, ct);

    public async Task AddAsync(JiraIssueSync entity, CancellationToken ct = default)
        => await _db.IssuesSyncs.AddAsync(entity, ct);

    public Task UpdateAsync(JiraIssueSync entity, CancellationToken ct = default)
    {
        _db.IssuesSyncs.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(JiraIssueSync entity, CancellationToken ct = default)
    {
        _db.IssuesSyncs.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
