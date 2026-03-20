using WorkHub.Modules.Knowledge.Application.Abstractions;
using WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;
using Elastic.Clients.Elasticsearch;

namespace WorkHub.Modules.Knowledge.Infrastructure.Search;

internal sealed class ElasticsearchIndexer : ISearchIndexer
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "knowledge-entries";

    public ElasticsearchIndexer(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexEntryAsync(
        Guid entryId,
        string issueTitle,
        string rootCause,
        string fix,
        IEnumerable<string> tags,
        CancellationToken ct = default)
    {
        await _client.IndexAsync(new
        {
            Id = entryId,
            IssueTitle = issueTitle,
            RootCause = rootCause,
            Fix = fix,
            Tags = tags.ToList()
        }, idx => idx.Index(IndexName).Id(entryId.ToString()), ct);
    }

    public async Task RemoveEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        await _client.DeleteAsync(IndexName, entryId.ToString(), ct);
    }
}

internal sealed class ElasticsearchSearchService : IKnowledgeSearchService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "knowledge-entries";

    public ElasticsearchSearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<SearchKnowledgeResult> SearchAsync(
        string keyword,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<KnowledgeEntryDto>(s => s
            .Indices(IndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(new[] { "issueTitle^3", "rootCause^2", "fix", "tags" })
                    .Query(keyword)
                    .Fuzziness(new Fuzziness("AUTO"))
                )
            ), ct);

        var items = response.Hits
            .Select(h => h.Source!)
            .ToList();

        return new SearchKnowledgeResult(
            items,
            (int)response.Total,
            page,
            pageSize);
    }
}
