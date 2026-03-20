using WorkHub.Modules.Knowledge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkHub.Modules.Knowledge.Infrastructure.Persistence.Configurations;

internal sealed class KnowledgeNodeConfiguration : IEntityTypeConfiguration<KnowledgeNode>
{
    public void Configure(EntityTypeBuilder<KnowledgeNode> builder)
    {
        builder.ToTable("knowledge_nodes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).IsRequired();

        builder.HasMany(x => x.Children)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class KnowledgeEntryConfiguration : IEntityTypeConfiguration<KnowledgeEntry>
{
    public void Configure(EntityTypeBuilder<KnowledgeEntry> builder)
    {
        builder.ToTable("knowledge_entries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IssueTitle).HasMaxLength(500).IsRequired();
        builder.Property(x => x.RootCause).IsRequired();
        builder.Property(x => x.Fix).IsRequired();
        builder.Property(x => x.FixVersion).HasMaxLength(50);
        builder.Property(x => x.JiraIssueKey).HasMaxLength(50);

        // Store tags as JSON array
        builder.Property<List<string>>("_tags")
            .HasColumnName("tags")
            .HasColumnType("jsonb");
    }
}
