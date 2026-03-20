using WorkHub.Modules.Knowledge.Application.Abstractions;
using WorkHub.Modules.Knowledge.Domain.Entities;
using WorkHub.Modules.Knowledge.Domain.Repositories;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Knowledge.Application.Commands.CreateEntry;

internal sealed class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, Result<Guid>>
{
    private readonly IKnowledgeRepository _repository;
    private readonly ISearchIndexer _searchIndexer;

    public CreateEntryCommandHandler(IKnowledgeRepository repository, ISearchIndexer searchIndexer)
    {
        _repository = repository;
        _searchIndexer = searchIndexer;
    }

    public async Task<Result<Guid>> Handle(CreateEntryCommand request, CancellationToken ct)
    {
        var node = await _repository.FindOrCreateNodeAsync(
            request.SoftwareName,
            request.ModuleName,
            request.CreatedBy,
            ct);

        var entry = KnowledgeEntry.Create(
            node.Id,
            request.IssueTitle,
            request.Description,
            request.RootCause,
            request.Fix,
            request.FixVersion,
            request.JiraIssueKey,
            request.Tags,
            request.CreatedBy);

        await _repository.AddEntryAsync(entry, ct);
        await _repository.SaveChangesAsync(ct);

        await _searchIndexer.IndexEntryAsync(
            entry.Id,
            entry.IssueTitle,
            entry.RootCause,
            entry.Fix,
            entry.Tags,
            ct);

        return Result<Guid>.Success(entry.Id);
    }
}
