using WorkHub.Modules.Knowledge.Application.Commands.CreateEntry;
using WorkHub.Modules.Knowledge.Application.Queries.SearchKnowledge;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkHub.API.Controllers;

[ApiController]
[Route("api/knowledge")]
[Authorize]
public class KnowledgeController : ControllerBase
{
    private readonly ISender _sender;

    public KnowledgeController(ISender sender) => _sender = sender;

    /// <summary>Tạo knowledge entry mới</summary>
    [HttpPost]
    public async Task<IActionResult> CreateEntry(
        [FromBody] CreateEntryRequest request,
        CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var command = new CreateEntryCommand(
            request.SoftwareName,
            request.ModuleName,
            request.IssueTitle,
            request.Description,
            request.RootCause,
            request.Fix,
            request.FixVersion,
            request.JiraIssueKey,
            request.Tags,
            userId);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Search), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Tìm kiếm knowledge entries</summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Search keyword is required." });

        var result = await _sender.Send(new SearchKnowledgeQuery(q, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : StatusCode(500, new { error = result.Error });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst("sub") ?? User.FindFirst("userId");
        return claim is not null && Guid.TryParse(claim.Value, out var id)
            ? id
            : throw new UnauthorizedAccessException("User identity not found.");
    }
}

public record CreateEntryRequest(
    string SoftwareName,
    string ModuleName,
    string IssueTitle,
    string Description,
    string RootCause,
    string Fix,
    string? FixVersion,
    string? JiraIssueKey,
    List<string> Tags);
