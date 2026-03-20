using WorkHub.Modules.Knowledge.Domain.Enums;
using WorkHub.Shared;

namespace WorkHub.Modules.Knowledge.Domain.Entities;

public class KnowledgeNode : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public KnowledgeNodeType Type { get; private set; }
    public Guid? ParentId { get; private set; }
    public KnowledgeNode? Parent { get; private set; }

    private readonly List<KnowledgeNode> _children = [];
    private readonly List<KnowledgeEntry> _entries = [];

    public IReadOnlyCollection<KnowledgeNode> Children => _children.AsReadOnly();
    public IReadOnlyCollection<KnowledgeEntry> Entries => _entries.AsReadOnly();

    private KnowledgeNode() { }

    public static KnowledgeNode CreateSoftware(string name, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new KnowledgeNode
        {
            Name = name.Trim(),
            Type = KnowledgeNodeType.Software,
            ParentId = null,
            CreatedBy = createdBy
        };
    }

    public KnowledgeNode AddChild(string name, KnowledgeNodeType type, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var child = new KnowledgeNode
        {
            Name = name.Trim(),
            Type = type,
            ParentId = Id,
            Parent = this,
            CreatedBy = createdBy
        };
        _children.Add(child);
        return child;
    }
}
