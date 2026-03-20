using WorkHub.Modules.Jira.Domain.Entities;
using WorkHub.Modules.Jira.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Modules.Jira.Infrastructure.Repositories;

internal sealed class IssueTemplateRepository : IIssueTemplateRepository
{
    private readonly JiraDbContext _db;
    public IssueTemplateRepository(JiraDbContext db) => _db = db;

    public async Task<IssueTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.IssueTemplates.FindAsync([id], ct);

    public async Task<IReadOnlyList<IssueTemplate>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default)
        => await _db.IssueTemplates.Where(x => x.OrganizationId == organizationId).ToListAsync(ct);

    public async Task<IssueTemplate?> GetDefaultAsync(Guid organizationId, string issueTypeName, CancellationToken ct = default)
        => await _db.IssueTemplates
            .Where(x => x.OrganizationId == organizationId && x.IsDefault)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(IssueTemplate entity, CancellationToken ct = default)
        => await _db.IssueTemplates.AddAsync(entity, ct);

    public Task UpdateAsync(IssueTemplate entity, CancellationToken ct = default)
    {
        _db.IssueTemplates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IssueTemplate entity, CancellationToken ct = default)
    {
        _db.IssueTemplates.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
