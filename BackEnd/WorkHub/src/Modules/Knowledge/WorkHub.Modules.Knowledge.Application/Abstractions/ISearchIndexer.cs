namespace WorkHub.Modules.Knowledge.Application.Abstractions;

public interface ISearchIndexer
{
    Task IndexEntryAsync(Guid entryId, string issueTitle, string rootCause, string fix, IEnumerable<string> tags, CancellationToken ct = default);
    Task RemoveEntryAsync(Guid entryId, CancellationToken ct = default);
}
