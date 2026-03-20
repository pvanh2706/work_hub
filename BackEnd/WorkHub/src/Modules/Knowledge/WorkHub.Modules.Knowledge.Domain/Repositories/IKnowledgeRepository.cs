using WorkHub.Modules.Knowledge.Domain.Entities;
using WorkHub.Modules.Knowledge.Domain.Enums;
using WorkHub.Shared.Abstractions;

namespace WorkHub.Modules.Knowledge.Domain.Repositories;

public interface IKnowledgeRepository : IRepository<KnowledgeEntry>
{
    Task<KnowledgeNode?> FindNodeAsync(string softwareName, string? moduleName, CancellationToken ct = default);
    Task<KnowledgeNode> FindOrCreateNodeAsync(string softwareName, string moduleName, Guid createdBy, CancellationToken ct = default);
    Task AddEntryAsync(KnowledgeEntry entry, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeNode>> GetTreeAsync(Guid organizationId, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeEntry>> GetEntriesByNodeAsync(Guid nodeId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
