using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkHub.Modules.Jira.Infrastructure.ExternalServices;

internal sealed class JiraApiClient : IJiraClient
{
    private readonly HttpClient _http;
    private readonly ILogger<JiraApiClient> _log;

    public JiraApiClient(HttpClient http, ILogger<JiraApiClient> log)
    {
        _http = http;
        _log = log;
    }

    // ── Create ────────────────────────────────────────────────────────────────
    public async Task<JiraCreatedIssue> CreateIssueAsync(CreateJiraIssueRequest req, CancellationToken ct = default)
    {
        var body = new
        {
            fields = new
            {
                project = new { key = req.ProjectKey },
                summary = req.Summary,
                description = new
                {
                    type = "doc",
                    version = 1,
                    content = new[]
                    {
                        new
                        {
                            type = "paragraph",
                            content = new[] { new { type = "text", text = req.Description } }
                        }
                    }
                },
                issuetype = new { id = req.IssueTypeId },
                priority = new { id = req.PriorityId },
                assignee = req.AssigneeAccountId is not null
                    ? new { accountId = req.AssigneeAccountId }
                    : null,
                labels = req.Labels
            }
        };

        var response = await _http.PostAsJsonAsync("rest/api/2/issue", body, ct);
        await EnsureSuccessAsync(response, ct);

        var result = await response.Content.ReadFromJsonAsync<JiraCreatedIssueResponse>(
            _jsonOptions, ct);

        return new JiraCreatedIssue(result!.Id, result.Key, result.Self);
    }

    // ── Edit ──────────────────────────────────────────────────────────────────
    public async Task EditIssueAsync(string issueKey, EditJiraIssueRequest req, CancellationToken ct = default)
    {
        var fields = new Dictionary<string, object?>();
        if (req.Summary is not null) fields["summary"] = req.Summary;
        if (req.Description is not null)
            fields["description"] = new
            {
                type = "doc",
                version = 1,
                content = new[]
                {
                    new
                    {
                        type = "paragraph",
                        content = new[] { new { type = "text", text = req.Description } }
                    }
                }
            };
        if (req.PriorityId is not null) fields["priority"] = new { id = req.PriorityId };
        if (req.AssigneeAccountId is not null)
            fields["assignee"] = new { accountId = req.AssigneeAccountId };

        object body;
        if (req.LabelsToAdd?.Count > 0 || req.LabelsToRemove?.Count > 0)
        {
            var labelOps = new List<object>();
            foreach (var l in req.LabelsToAdd ?? []) labelOps.Add(new { add = l });
            foreach (var l in req.LabelsToRemove ?? []) labelOps.Add(new { remove = l });
            body = new { fields, update = new { labels = labelOps } };
        }
        else
        {
            body = new { fields };
        }

        var response = await _http.PutAsJsonAsync($"rest/api/2/issue/{issueKey}", body, ct);
        await EnsureSuccessAsync(response, ct);
    }

    // ── Get Issue ─────────────────────────────────────────────────────────────
    public async Task<JiraIssueDetail> GetIssueAsync(string issueKey, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(
            $"rest/api/2/issue/{issueKey}?fields=summary,description,status,priority,assignee,issuetype,timetracking,worklog",
            ct);
        await EnsureSuccessAsync(response, ct);

        var doc = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions, ct);
        var fields = doc.GetProperty("fields");

        return new JiraIssueDetail(
            Id: doc.GetProperty("id").GetString()!,
            Key: doc.GetProperty("key").GetString()!,
            Summary: GetString(fields, "summary") ?? string.Empty,
            Description: GetString(fields, "description"),
            StatusName: fields.GetProperty("status").GetProperty("name").GetString()!,
            StatusCategory: fields.GetProperty("status").GetProperty("statusCategory").GetProperty("name").GetString()!,
            AssigneeAccountId: GetNestedString(fields, "assignee", "accountId"),
            AssigneeDisplayName: GetNestedString(fields, "assignee", "displayName"),
            PriorityName: GetString(fields.GetProperty("priority"), "name") ?? "Medium",
            IssueTypeName: fields.GetProperty("issuetype").GetProperty("name").GetString()!,
            OriginalEstimate: GetNestedString(fields, "timetracking", "originalEstimate"),
            TimeSpent: GetNestedString(fields, "timetracking", "timeSpent"),
            Worklogs: ParseWorklogs(fields));
    }

    // ── Get Transitions ───────────────────────────────────────────────────────
    public async Task<IReadOnlyList<JiraTransition>> GetTransitionsAsync(string issueKey, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"rest/api/2/issue/{issueKey}/transitions", ct);
        await EnsureSuccessAsync(response, ct);

        var doc = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions, ct);
        var transitions = doc.GetProperty("transitions").EnumerateArray().Select(t =>
            new JiraTransition(
                Id: t.GetProperty("id").GetString()!,
                Name: t.GetProperty("name").GetString()!,
                ToStatusName: t.GetProperty("to").GetProperty("name").GetString()!,
                ToStatusCategory: t.GetProperty("to").GetProperty("statusCategory").GetProperty("name").GetString()!))
            .ToList();

        return transitions;
    }

    // ── Transition ────────────────────────────────────────────────────────────
    public async Task TransitionIssueAsync(string issueKey, string transitionId, string? comment = null, CancellationToken ct = default)
    {
        object body = comment is not null
            ? new
            {
                transition = new { id = transitionId },
                update = new
                {
                    comment = new[] { new { add = new { body = comment } } }
                }
            }
            : new { transition = new { id = transitionId } };

        var response = await _http.PostAsJsonAsync($"rest/api/2/issue/{issueKey}/transitions", body, ct);
        await EnsureSuccessAsync(response, ct);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Jira API returned {(int)response.StatusCode}: {content}",
                null,
                response.StatusCode);
        }
    }

    private static string? GetString(JsonElement elem, string prop)
        => elem.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string? GetNestedString(JsonElement root, string prop1, string prop2)
        => root.TryGetProperty(prop1, out var v1) && v1.ValueKind == JsonValueKind.Object
            ? GetString(v1, prop2)
            : null;

    private static IReadOnlyList<JiraWorklogEntry> ParseWorklogs(JsonElement fields)
    {
        if (!fields.TryGetProperty("worklog", out var wl) ||
            !wl.TryGetProperty("worklogs", out var entries))
            return [];

        return entries.EnumerateArray().Select(w => new JiraWorklogEntry(
            Id: w.TryGetProperty("id", out var wid) ? wid.GetString()! : string.Empty,
            AuthorAccountId: GetNestedString(w, "author", "accountId") ?? string.Empty,
            AuthorDisplayName: GetNestedString(w, "author", "displayName") ?? "Unknown",
            TimeSpent: GetString(w, "timeSpent") ?? string.Empty,
            TimeSpentSeconds: w.TryGetProperty("timeSpentSeconds", out var ts) ? ts.GetInt32() : 0,
            Started: w.GetProperty("started").GetDateTime())).ToList();
    }

    private sealed record JiraCreatedIssueResponse(string Id, string Key, string Self);
}
