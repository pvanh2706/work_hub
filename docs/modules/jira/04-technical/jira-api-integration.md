# Jira API Integration

Integration with Atlassian Jira Cloud REST API v3.

## Setup

### Get API Token

**Steps:**
1. Go to https://id.atlassian.com/manage-profile/security/api-tokens
2. Click "Create API token"
3. Name it: "WorkHub Integration"
4. Copy token (save securely!)

### Configuration

```json
{
  \"Jira\": {
    \"BaseUrl\": \"https://your-domain.atlassian.net\",
    \"Email\": \"your-email@company.com\",
    \"ApiToken\": \"your-api-token-here\"
  }
}
```

## API Client Implementation

```csharp
public interface IJiraApiClient
{
    Task<Result<string>> CreateIssue(CreateIssueRequest request, CancellationToken ct);
    Task<Result<JiraIssue>> GetIssue(string issueKey, CancellationToken ct);
    Task<Result> UpdateIssue(string issueKey, UpdateIssueRequest request, CancellationToken ct);
    Task<Result<SearchResult>> SearchIssues(string jql, CancellationToken ct);
}

public class JiraApiClient : IJiraApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JiraApiClient> _logger;
    
    public JiraApiClient(HttpClient httpClient, IOptions<JiraSettings> settings, ILogger<JiraApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($\"{settings.Value.Email}:{settings.Value.ApiToken}\")
        );
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue(\"Basic\", credentials);
    }
    
    public async Task<Result<string>> CreateIssue(CreateIssueRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(\"/rest/api/3/issue\", request, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError(\"Failed to create issue: {Error}\", error);
                return Result<string>.Failure($\"Jira API error: {error}\");
            }
            
            var result = await response.Content.ReadFromJsonAsync<CreateIssueResponse>(ct);
            return Result<string>.Success(result!.Key);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, \"HTTP error creating issue\");
            return Result<string>.Failure(\"Network error\");
        }
    }
}
```

## Common Operations

### Create Issue

```csharp
POST /rest/api/3/issue

{
  \"fields\": {
    \"project\": { \"key\": \"PROJ\" },
    \"summary\": \"Login bug on mobile\",
    \"description\": {
      \"type\": \"doc\",
      \"version\": 1,
      \"content\": [
        {
          \"type\": \"paragraph\",
          \"content\": [{
            \"type\": \"text\",
            \"text\": \"Bug description here\"
          }]
        }
      ]
    },
    \"issuetype\": { \"name\": \"Bug\" },
    \"priority\": { \"name\": \"High\" }
  }
}

// Response
{
  \"id\": \"10000\",
  \"key\": \"PROJ-123\",
  \"self\": \"https://your-domain.atlassian.net/rest/api/3/issue/10000\"
}
```

### Get Issue

```csharp
GET /rest/api/3/issue/PROJ-123

// Response
{
  \"key\": \"PROJ-123\",
  \"fields\": {
    \"summary\": \"Login bug\",
    \"description\": {...},
    \"status\": { \"name\": \"In Progress\" },
    \"assignee\": { \"accountId\": \"...\", \"displayName\": \"John Doe\" },
    \"priority\": { \"name\": \"High\" }
  }
}
```

### Search Issues

```csharp
GET /rest/api/3/search?jql=project=PROJ AND status=\"In Progress\"

// Response
{
  \"startAt\": 0,
  \"maxResults\": 50,
  \"total\": 150,
  \"issues\": [
    { \"key\": \"PROJ-123\", \"fields\": {...} }
  ]
}
```

## Rate Limiting

**Jira limits:** 10,000 requests per hour

**Implementation:**
```csharp
services.AddHttpClient<IJiraApiClient, JiraApiClient>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

## Webhooks

### Register Webhook

```bash
POST /rest/api/3/webhook

{
  \"name\": \"WorkHub Sync\",
  \"url\": \"https://workhub.company.com/api/jira/webhooks\",
  \"events\": [
    \"jira:issue_created\",
    \"jira:issue_updated\",
    \"jira:issue_deleted\"
  ]
}
```

### Handle Webhook

```csharp
[HttpPost(\"webhooks\")]
public async Task<IActionResult> HandleWebhook([FromBody] JiraWebhookPayload payload)
{
    // Verify signature
    if (!VerifySignature(Request.Headers[\"X-Hub-Signature\"], payload))
        return Unauthorized();
    
    // Process event
    await _mediator.Publish(new JiraIssueUpdatedNotification(payload));
    
    return Ok();
}
```

---

**Last Updated:** 2026-03-22
