using WorkHub.Shared.Abstractions;

namespace WorkHub.Modules.Knowledge.Domain.Events;

public record KnowledgeEntryCreatedEvent(
    Guid EntryId,
    string IssueTitle,
    string SoftwareName
) : DomainEvent;
