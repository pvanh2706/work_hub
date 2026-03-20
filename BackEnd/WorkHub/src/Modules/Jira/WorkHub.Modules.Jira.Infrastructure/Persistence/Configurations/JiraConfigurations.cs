using WorkHub.Modules.Jira.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkHub.Modules.Jira.Infrastructure.Persistence.Configurations;

internal sealed class JiraIssueSyncConfiguration : IEntityTypeConfiguration<JiraIssueSync>
{
    public void Configure(EntityTypeBuilder<JiraIssueSync> builder)
    {
        builder.ToTable("issue_syncs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.JiraProjectKey).IsRequired().HasMaxLength(50);
        builder.Property(x => x.JiraIssueId).HasMaxLength(100);
        builder.Property(x => x.JiraIssueKey).HasMaxLength(50);
        builder.Property(x => x.Summary).IsRequired().HasMaxLength(400);
        builder.Property(x => x.IssueType)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(x => x.SyncStatus)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(x => x.LastSyncError).HasMaxLength(1000);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}

internal sealed class IssueTemplateConfiguration : IEntityTypeConfiguration<IssueTemplate>
{
    public void Configure(EntityTypeBuilder<IssueTemplate> builder)
    {
        builder.ToTable("issue_templates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.IssueTypeName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SummaryPrefix).HasMaxLength(200);
        builder.Property(x => x.DefaultDescription).HasMaxLength(5000);
        builder.Property(x => x.DefaultLabels).HasMaxLength(500);
        builder.Property(x => x.DefaultPriorityId).HasMaxLength(50);
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
