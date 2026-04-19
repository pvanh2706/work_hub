using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.Entities;
using WorkHub.Modules.Jira.Domain.Repositories;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.CreateIssueAndTransitionToDone;

internal sealed class CreateIssueAndTransitionToDoneCommandHandler
    : IRequestHandler<CreateIssueAndTransitionToDoneCommand, Result<CreateIssueAndTransitionToDoneResult>>
{
    private readonly IJiraClient _jiraClient;
    private readonly IJiraIssueSyncRepository _syncRepository;

    public CreateIssueAndTransitionToDoneCommandHandler(
        IJiraClient jiraClient,
        IJiraIssueSyncRepository syncRepository)
    {
        _jiraClient = jiraClient;
        _syncRepository = syncRepository;
    }

    public async Task<Result<CreateIssueAndTransitionToDoneResult>> Handle(
        CreateIssueAndTransitionToDoneCommand request,
        CancellationToken ct)
    {
        // Bước 1: Ghi nhận ý định trước khi gọi Jira (write-ahead, tránh orphan issue)
        var sync = JiraIssueSync.CreatePending(
            request.OrganizationId,
            request.UserId,
            request.ProjectKey,
            request.Summary,
            request.IssueType,
            request.Priority);

        await _syncRepository.AddAsync(sync, ct);
        await _syncRepository.SaveChangesAsync(ct);

        try
        {
            // Bước 2: Tạo issue và chuyển ngay sang Done trong một luồng
            var created = await _jiraClient.CreateIssueAndTransitionToDoneAsync(
                new CreateJiraIssueRequest(
                    request.ProjectKey,
                    request.Summary,
                    request.Description,
                    request.IssueTypeId,
                    request.PriorityId,
                    request.AssigneeAccountId,
                    request.Labels,
                    request.WorklogTimeSpent,
                    request.WorklogComment,
                    request.WorklogStarted),
                ct);

            // Bước 3: Đánh dấu Synced
            sync.MarkSynced(created.Id, created.Key);
            await _syncRepository.SaveChangesAsync(ct);

            return Result<CreateIssueAndTransitionToDoneResult>.Success(
                new CreateIssueAndTransitionToDoneResult(sync.Id, created.Id, created.Key));
        }
        catch (Exception ex)
        {
            sync.MarkFailed(ex.Message);
            await _syncRepository.SaveChangesAsync(ct);
            return Result<CreateIssueAndTransitionToDoneResult>.Failure($"Jira API error: {ex.Message}");
        }
    }
}
