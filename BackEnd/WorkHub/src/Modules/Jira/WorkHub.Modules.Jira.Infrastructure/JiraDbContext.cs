using WorkHub.Modules.Jira.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Modules.Jira.Infrastructure;

internal sealed class JiraDbContext : DbContext
{
    public JiraDbContext(DbContextOptions<JiraDbContext> options) : base(options) { }

    public DbSet<JiraIssueSync> IssuesSyncs => Set<JiraIssueSync>();
    public DbSet<IssueTemplate> IssueTemplates => Set<IssueTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jira");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JiraDbContext).Assembly);
    }
}
