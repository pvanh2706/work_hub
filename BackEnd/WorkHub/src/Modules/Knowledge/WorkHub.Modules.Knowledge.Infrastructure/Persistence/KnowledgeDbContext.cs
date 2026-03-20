using WorkHub.Modules.Knowledge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WorkHub.Modules.Knowledge.Infrastructure.Persistence;

internal sealed class KnowledgeDbContext : DbContext
{
    public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options) : base(options) { }

    public DbSet<KnowledgeNode> Nodes => Set<KnowledgeNode>();
    public DbSet<KnowledgeEntry> Entries => Set<KnowledgeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("knowledge");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
