using WorkHub.Modules.Knowledge.Application.Abstractions;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;

internal sealed class SearchKnowledgeQueryHandler
    : IRequestHandler<SearchKnowledgeQuery, Result<SearchKnowledgeResult>>
{
    private readonly IKnowledgeSearchService _searchService;

    public SearchKnowledgeQueryHandler(IKnowledgeSearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<Result<SearchKnowledgeResult>> Handle(
        SearchKnowledgeQuery request,
        CancellationToken ct)
    {
        var result = await _searchService.SearchAsync(request.Keyword, request.Page, request.PageSize, ct);
        return Result<SearchKnowledgeResult>.Success(result);
    }
}
