using WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;

namespace WorkHub.Modules.Knowledge.Application.Abstractions;

public interface IKnowledgeSearchService
{
    Task<SearchKnowledgeResult> SearchAsync(string keyword, int page, int pageSize, CancellationToken ct = default);
}
