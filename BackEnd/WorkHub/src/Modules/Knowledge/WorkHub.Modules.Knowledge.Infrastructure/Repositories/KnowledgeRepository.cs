using WorkHub.Modules.Knowledge.Domain.Entities;
using WorkHub.Modules.Knowledge.Domain.Enums;
using WorkHub.Modules.Knowledge.Domain.Repositories;
using WorkHub.Modules.Knowledge.Infrastructure.Persistence;
using WorkHub.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace WorkHub.Modules.Knowledge.Infrastructure.Repositories;

internal sealed class KnowledgeRepository : IKnowledgeRepository
{
    private readonly KnowledgeDbContext _context;

    public KnowledgeRepository(KnowledgeDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Entries.FindAsync([id], ct);

    public async Task AddAsync(KnowledgeEntry entity, CancellationToken ct = default)
        => await _context.Entries.AddAsync(entity, ct);

    public Task UpdateAsync(KnowledgeEntry entity, CancellationToken ct = default)
    {
        _context.Entries.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(KnowledgeEntry entity, CancellationToken ct = default)
    {
        _context.Entries.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<KnowledgeNode?> FindNodeAsync(
        string softwareName,
        string? moduleName,
        CancellationToken ct = default)
    {
        return await _context.Nodes
            .FirstOrDefaultAsync(n =>
                n.Name == softwareName &&
                n.Type == KnowledgeNodeType.Software, ct);
    }

    public async Task<KnowledgeNode> FindOrCreateNodeAsync(
        string softwareName,
        string moduleName,
        Guid createdBy,
        CancellationToken ct = default)
    {
        var software = await _context.Nodes
            .Include(n => n.Children)
            .FirstOrDefaultAsync(n =>
                n.Name == softwareName &&
                n.Type == KnowledgeNodeType.Software, ct);

        if (software is null)
        {
            software = KnowledgeNode.CreateSoftware(softwareName, createdBy);
            await _context.Nodes.AddAsync(software, ct);
        }

        var module = software.Children
            .FirstOrDefault(n => n.Name == moduleName && n.Type == KnowledgeNodeType.Module);

        if (module is null)
        {
            module = software.AddChild(moduleName, KnowledgeNodeType.Module, createdBy);
        }

        return module;
    }

    public async Task AddEntryAsync(KnowledgeEntry entry, CancellationToken ct = default)
        => await _context.Entries.AddAsync(entry, ct);

    public async Task<IReadOnlyList<KnowledgeNode>> GetTreeAsync(
        Guid organizationId,
        CancellationToken ct = default)
    {
        return await _context.Nodes
            .Include(n => n.Children)
            .Where(n => n.Type == KnowledgeNodeType.Software)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<KnowledgeEntry>> GetEntriesByNodeAsync(
        Guid nodeId,
        CancellationToken ct = default)
    {
        return await _context.Entries
            .Where(e => e.NodeId == nodeId)
            .OrderByDescending(e => e.HelpfulVotes)
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
