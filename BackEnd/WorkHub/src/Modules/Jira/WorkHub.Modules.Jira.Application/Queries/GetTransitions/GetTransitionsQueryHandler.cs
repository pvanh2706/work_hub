using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.ValueObjects;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Queries.GetTransitions;

internal sealed class GetTransitionsQueryHandler
    : IRequestHandler<GetTransitionsQuery, Result<IReadOnlyList<JiraTransition>>>
{
    private readonly IJiraClient _jiraClient;

    public GetTransitionsQueryHandler(IJiraClient jiraClient) => _jiraClient = jiraClient;

    public async Task<Result<IReadOnlyList<JiraTransition>>> Handle(
        GetTransitionsQuery request,
        CancellationToken ct)
    {
        try
        {
            var transitions = await _jiraClient.GetTransitionsAsync(request.JiraIssueKey, ct);
            return Result<IReadOnlyList<JiraTransition>>.Success(transitions);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<JiraTransition>>.Failure($"Jira API error: {ex.Message}");
        }
    }
}
