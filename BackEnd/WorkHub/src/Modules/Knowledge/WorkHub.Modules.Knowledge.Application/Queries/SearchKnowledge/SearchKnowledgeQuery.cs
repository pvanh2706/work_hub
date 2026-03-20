using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;

public record SearchKnowledgeQuery(
    string Keyword,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<SearchKnowledgeResult>>;
