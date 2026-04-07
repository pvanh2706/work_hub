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
        // ── Bước 1: Tạo sync record với trạng thái PendingCreate và lưu vào DB ──────
        //
        // Mục đích: Ghi nhận "ý định tạo issue" trước khi thực sự gọi Jira API.
        // Tại sao phải làm trước? Vì nếu gọi API xong mới lưu DB mà bị crash
        // giữa chừng, chúng ta sẽ không biết issue đã được tạo bên Jira hay chưa —
        // dẫn đến dữ liệu mất đồng bộ (orphan issue trên Jira mà DB không hay biết).
        //
        // Trạng thái PendingCreate = "đã ghi nhận yêu cầu, chưa xác nhận Jira đã tạo".
        // Đây là pattern "write-ahead log" / Outbox: ghi nội bộ trước, rồi mới thực thi.
        var sync = JiraIssueSync.CreatePending(
            request.OrganizationId,
            request.UserId,
            request.ProjectKey,
            request.Summary,
            request.IssueType,
            request.Priority);

        await _syncRepository.AddAsync(sync, ct);
        await _syncRepository.SaveChangesAsync(ct); // Commit PendingCreate vào DB ngay

        // ── Bước 2: Gọi Jira API để thực sự tạo issue ───────────────────────────────
        //
        // Tách thành try/catch vì đây là I/O ra hệ thống ngoài (external API),
        // hoàn toàn có thể fail vì: network timeout, Jira down, token hết hạn,
        // dữ liệu không hợp lệ theo rule của Jira, v.v.
        // Chúng ta cần bắt lỗi để xử lý gracefully thay vì để exception lan ra ngoài.
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

            // ── Bước 3: Jira tạo thành công → cập nhật sync record → Synced ─────────
            //
            // Lưu lại JiraIssueId và JiraIssueKey mà Jira trả về (vd: "10001", "PROJ-42").
            // Chuyển trạng thái sang Synced = "DB và Jira đã đồng bộ".
            // Từ đây về sau, mọi thao tác edit/transition đều dùng JiraIssueKey này.
            sync.MarkSynced(created.Id, created.Key);
            await _syncRepository.SaveChangesAsync(ct);

            return Result<CreateIssueResult>.Success(
                new CreateIssueResult(sync.Id, created.Id, created.Key));
        }
        catch (Exception ex)
        {
            // ── Bước 4: Jira API lỗi → đánh dấu Failed, trả về Result.Failure ───────
            //
            // Tại sao không throw lại exception?
            // Vì controller dùng pattern Result<T> — lỗi nghiệp vụ được biểu diễn
            // qua giá trị trả về, không phải exception. Điều này giúp:
            //   - Controller quyết định HTTP status code một cách rõ ràng (400 vs 500)
            //   - Tránh lộ stack trace ra ngoài
            //   - Caller không cần try/catch
            //
            // Sync record vẫn được lưu với trạng thái Failed + lý do lỗi.
            // Điều này cho phép: audit trail, retry sau, hoặc alert monitoring.
            sync.MarkFailed(ex.Message);
            await _syncRepository.SaveChangesAsync(ct);
            return Result<CreateIssueResult>.Failure($"Jira API error: {ex.Message}");
        }
    }
}
