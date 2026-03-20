using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.Repositories;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.EditIssue;

internal sealed class EditIssueCommandHandler : IRequestHandler<EditIssueCommand, Result>
{
    private readonly IJiraClient _jiraClient;
    private readonly IJiraIssueSyncRepository _syncRepository;

    public EditIssueCommandHandler(
        IJiraClient jiraClient,
        IJiraIssueSyncRepository syncRepository)
    {
        _jiraClient = jiraClient;
        _syncRepository = syncRepository;
    }

    public async Task<Result> Handle(EditIssueCommand request, CancellationToken ct)
    {
        try
        {
            await _jiraClient.EditIssueAsync(
                request.JiraIssueKey,
                new EditJiraIssueRequest(
                    request.Summary,
                    request.Description,
                    request.PriorityId,
                    request.AssigneeAccountId,
                    request.LabelsToAdd,
                    request.LabelsToRemove),
                ct);

            // Cập nhật sync record nếu có
            var sync = await _syncRepository.FindByJiraKeyAsync(request.JiraIssueKey, ct);
            if (sync is not null)
            {
                sync.MarkPendingUpdate();
                sync.SetUpdated(request.UserId);
                await _syncRepository.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Jira API error: {ex.Message}");
        }
    }
}
