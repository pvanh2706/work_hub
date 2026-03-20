using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.TransitionIssue;

internal sealed class TransitionIssueCommandHandler : IRequestHandler<TransitionIssueCommand, Result>
{
    private readonly IJiraClient _jiraClient;

    public TransitionIssueCommandHandler(IJiraClient jiraClient)
    {
        _jiraClient = jiraClient;
    }

    public async Task<Result> Handle(TransitionIssueCommand request, CancellationToken ct)
    {
        try
        {
            await _jiraClient.TransitionIssueAsync(
                request.JiraIssueKey,
                request.TransitionId,
                request.Comment,
                ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Jira transition error: {ex.Message}");
        }
    }
}
