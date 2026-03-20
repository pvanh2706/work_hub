namespace WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;

public record SearchKnowledgeResult(
    IReadOnlyList<KnowledgeEntryDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record KnowledgeEntryDto(
    Guid Id,
    string IssueTitle,
    string RootCause,
    string Fix,
    string? FixVersion,
    string? JiraIssueKey,
    IReadOnlyList<string> Tags,
    int HelpfulVotes,
    DateTime CreatedAt);
