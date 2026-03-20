using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.Entities;
using WorkHub.Modules.Jira.Domain.Repositories;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Commands.CreateIssue;

internal sealed class CreateIssueCommandHandler
    : IRequestHandler<CreateIssueCommand, Result<CreateIssueResult>>
{
    private readonly IJiraClient _jiraClient;
    private readonly IJiraIssueSyncRepository _syncRepository;

    public CreateIssueCommandHandler(
        IJiraClient jiraClient,
        IJiraIssueSyncRepository syncRepository)
    {
        _jiraClient = jiraClient;
        _syncRepository = syncRepository;
    }

    public async Task<Result<CreateIssueResult>> Handle(
        CreateIssueCommand request,
        CancellationToken ct)
    {
        // 1. Lưu record sync với trạng thái PendingCreate
        var sync = JiraIssueSync.CreatePending(
            request.OrganizationId,
            request.UserId,
            request.ProjectKey,
            request.Summary,
            request.IssueType,
            request.Priority);

        await _syncRepository.AddAsync(sync, ct);
        await _syncRepository.SaveChangesAsync(ct);

        // 2. Gọi Jira API
        try
        {
            var created = await _jiraClient.CreateIssueAsync(
                new CreateJiraIssueRequest(
                    request.ProjectKey,
                    request.Summary,
                    request.Description,
                    request.IssueTypeId,
                    request.PriorityId,
                    request.AssigneeAccountId,
                    request.Labels),
                ct);

            // 3. Cập nhật sync record → Synced
            sync.MarkSynced(created.Id, created.Key);
            await _syncRepository.SaveChangesAsync(ct);

            return Result<CreateIssueResult>.Success(
                new CreateIssueResult(sync.Id, created.Id, created.Key));
        }
        catch (Exception ex)
        {
            // 4. Đánh dấu failed, không throw — caller nhận Result.Failure
            sync.MarkFailed(ex.Message);
            await _syncRepository.SaveChangesAsync(ct);
            return Result<CreateIssueResult>.Failure($"Jira API error: {ex.Message}");
        }
    }
}
