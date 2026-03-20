using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.ValueObjects;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Queries.GetIssue;

internal sealed class GetIssueQueryHandler : IRequestHandler<GetIssueQuery, Result<JiraIssueDetail>>
{
    private readonly IJiraClient _jiraClient;

    public GetIssueQueryHandler(IJiraClient jiraClient) => _jiraClient = jiraClient;

    public async Task<Result<JiraIssueDetail>> Handle(GetIssueQuery request, CancellationToken ct)
    {
        try
        {
            var issue = await _jiraClient.GetIssueAsync(request.JiraIssueKey, ct);
            return Result<JiraIssueDetail>.Success(issue);
        }
        catch (Exception ex)
        {
            return Result<JiraIssueDetail>.Failure($"Jira API error: {ex.Message}");
        }
    }
}
