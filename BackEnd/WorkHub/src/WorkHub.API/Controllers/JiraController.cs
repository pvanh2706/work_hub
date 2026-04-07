using WorkHub.Modules.Jira.Application.Commands.CreateIssue;
using WorkHub.Modules.Jira.Application.Commands.EditIssue;
using WorkHub.Modules.Jira.Application.Commands.TransitionIssue;
using WorkHub.Modules.Jira.Application.Queries.GetIssue;
using WorkHub.Modules.Jira.Application.Queries.GetTransitions;
using WorkHub.Modules.Jira.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkHub.API.Controllers;

[ApiController]
[Route("api/jira")]
[Authorize]
public class JiraController : ControllerBase
{
    private readonly ISender _sender;
    /// <summary>
    /// ISender là interface của thư viện MediatR, dùng để gửi requests qua mediator pipeline.
    /// Trong JiraController, nó được inject qua constructor và dùng để dispatch commands/queries.
    /// 
    /// Cách hoạt động:
    /// Thay vì controller gọi trực tiếp service, _sender.Send(command) sẽ tìm IRequestHandler 
    /// tương ứng và thực thi nó. Ví dụ: _sender.Send(new CreateIssueCommand(...)) 
    /// → tự động tìm và chạy CreateIssueCommandHandler.
    /// 
    /// Lợi ích: Controller không cần biết implementation cụ thể, chỉ cần gửi request — 
    /// giúp tách biệt tầng API khỏi Application layer (Clean Architecture / CQRS pattern).
    /// </summary>
    /// <param name="sender">ISender instance từ MediatR để gửi các command và query</param>
    public JiraController(ISender sender) => _sender = sender;

    /// <summary>Tạo Jira issue mới và lưu trạng thái sync</summary>
    [HttpPost("issues")]
    public async Task<IActionResult> CreateIssue(
        [FromBody] CreateIssueRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var command = new CreateIssueCommand(
            request.OrganizationId,
            userId,
            request.ProjectKey,
            request.Summary,
            request.Description,
            request.IssueType,
            request.Priority,
            request.IssueTypeId,
            request.PriorityId,
            request.AssigneeAccountId,
            request.Labels ?? []);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Cập nhật Jira issue</summary>
    [HttpPut("issues/{issueKey}")]
    public async Task<IActionResult> EditIssue(
        string issueKey,
        [FromBody] EditIssueRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var command = new EditIssueCommand(
            issueKey,
            userId,
            request.Summary,
            request.Description,
            request.PriorityId,
            request.AssigneeAccountId,
            request.LabelsToAdd,
            request.LabelsToRemove);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Lấy chi tiết Jira issue</summary>
    [HttpGet("issues/{issueKey}")]
    public async Task<IActionResult> GetIssue(string issueKey, CancellationToken ct)
    {
        var result = await _sender.Send(new GetIssueQuery(issueKey), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    /// <summary>Lấy danh sách transitions của issue</summary>
    [HttpGet("issues/{issueKey}/transitions")]
    public async Task<IActionResult> GetTransitions(string issueKey, CancellationToken ct)
    {
        var result = await _sender.Send(new GetTransitionsQuery(issueKey), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Chuyển trạng thái Jira issue</summary>
    [HttpPost("issues/{issueKey}/transitions")]
    public async Task<IActionResult> TransitionIssue(
        string issueKey,
        [FromBody] TransitionIssueRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var command = new TransitionIssueCommand(issueKey, request.TransitionId, userId, request.Comment);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }
}

// ── Request DTOs ─────────────────────────────────────────────────────────────

public record CreateIssueRequest(
    Guid OrganizationId,
    string ProjectKey,
    string Summary,
    string Description,
    IssueType IssueType,
    IssuePriority Priority,
    string IssueTypeId,
    string PriorityId,
    string? AssigneeAccountId,
    List<string>? Labels);

public record EditIssueRequest(
    string? Summary,
    string? Description,
    string? PriorityId,
    string? AssigneeAccountId,
    List<string>? LabelsToAdd,
    List<string>? LabelsToRemove);

public record TransitionIssueRequest(string TransitionId, string? Comment);
