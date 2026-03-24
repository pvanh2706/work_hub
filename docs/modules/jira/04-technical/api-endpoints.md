# Jira Module - API Endpoints

Complete REST API reference for Jira Module.

## Base URL

```
Production: https://workhub.company.com/api/jira
Development: https://localhost:7001/api/jira
```

## Authentication

All endpoints require JWT Bearer token:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Rate Limiting

- **Limit:** 100 requests per minute per user
- **Burst:** 20 requests in 10 seconds
- **Headers:**
  ```http
  X-RateLimit-Limit: 100
  X-RateLimit-Remaining: 85
  X-RateLimit-Reset: 1679472000
  ```

## Templates

### Create Template

```http
POST /templates
Content-Type: application/json

{
  \"name\": \"Bug Report\",
  \"type\": \"Bug\",
  \"description\": \"Standard bug report template\",
  \"fields\": {
    \"priority\": \"High\",
    \"labels\": [\"bug\", \"needs-investigation\"],
    \"descriptionTemplate\": \"## Bug\\n{description}\\n...\"
  }
}
```

**Response (201 Created):**
```json
{
  \"id\": \"3fa85f64-5717-4562-b3fc-2c963f66afa6\",
  \"name\": \"Bug Report\",
  \"type\": \"Bug\",
  \"fields\": { ... },
  \"createdAt\": \"2026-03-22T10:00:00Z\",
  \"createdBy\": \"john.doe@company.com\"
}
```

### Get Templates

```http
GET /templates?type=Bug&page=1&pageSize=20
```

**Response (200 OK):**
```json
{
  \"items\": [
    {
      \"id\": \"3fa85f64...\",
      \"name\": \"Bug Report\",
      \"type\": \"Bug\",
      \"usageCount\": 145
    }
  ],
  \"totalCount\": 5,
  \"page\": 1,
  \"pageSize\": 20
}
```

### Update Template

```http
PUT /templates/{id}
{
  \"name\": \"Updated Bug Report\",
  \"fields\": { ... }
}
```

**Response (200 OK):** Updated template

### Delete Template

```http
DELETE /templates/{id}
```

**Response (204 No Content)**

## Issues

### Quick Create Issue

```http
POST /issues/quick-create
Content-Type: application/json

{
  \"title\": \"Login button not working on mobile\",
  \"description\": \"Button appears frozen\",
  \"type\": \"Bug\"
}
```

**Response (201 Created):**
```json
{
  \"id\": \"7c9e6679-7425-40de-944b-e07fc1f90ae7\",
  \"jiraKey\": \"PROJ-123\",
  \"title\": \"Login button not working on mobile\",
  \"type\": \"Bug\",
  \"priority\": \"High\",
  \"status\": \"To Do\",
  \"url\": \"https://your-domain.atlassian.net/browse/PROJ-123\",
  \"createdAt\": \"2026-03-22T10:00:00Z\"
}
```

### Get Issue

```http
GET /issues/{id}
```

**Response (200 OK):**
```json
{
  \"id\": \"7c9e6679-7425-40de-944b-e07fc1f90ae7\",
  \"jiraKey\": \"PROJ-123\",
  \"title\": \"Login button not working on mobile\",
  \"description\": \"Full description...\",
  \"type\": \"Bug\",
  \"priority\": \"High\",
  \"status\": \"In Progress\",
  \"assignee\": {
    \"id\": \"user-id\",
    \"name\": \"John Doe\",
    \"email\": \"john@company.com\"
  },
  \"reporter\": { ... },
  \"createdAt\": \"2026-03-22T10:00:00Z\",
  \"updatedAt\": \"2026-03-22T14:30:00Z\",
  \"syncedAt\": \"2026-03-22T14:30:10Z\",
  \"syncStatus\": \"synced\"
}
```

### Search Issues

```http
GET /issues/search?q=login&type=Bug&status=Open&page=1&pageSize=20
```

**Response (200 OK):**
```json
{
  \"items\": [
    {
      \"id\": \"...\",
      \"jiraKey\": \"PROJ-123\",
      \"title\": \"Login button not working\",
      \"type\": \"Bug\",
      \"priority\": \"High\",
      \"status\": \"Open\",
      \"assignee\": \"John Doe\"
    }
  ],
  \"totalCount\": 45,
  \"page\": 1,
  \"pageSize\": 20
}
```

### Update Issue

```http
PATCH /issues/{id}
Content-Type: application/json

{
  \"title\": \"Updated title\",
  \"description\": \"Updated description\",
  \"priority\": \"Highest\",
  \"assignee\": \"jane.doe@company.com\"
}
```

**Response (200 OK):** Updated issue

### Bulk Create

```http
POST /issues/bulk-create
{
  \"templateId\": \"3fa85f64-5717-4562-b3fc-2c963f66afa6\",
  \"issues\": [
    { \"title\": \"Fix bug 1\", \"description\": \"Description 1\" },
    { \"title\": \"Fix bug 2\", \"description\": \"Description 2\" }
  ]
}
```

**Response (201 Created):**
```json
{
  \"created\": 2,
  \"failed\": 0,
  \"issues\": [
    { \"key\": \"PROJ-124\", \"title\": \"Fix bug 1\" },
    { \"key\": \"PROJ-125\", \"title\": \"Fix bug 2\" }
  ]
}
```

### Bulk Update

```http
POST /issues/bulk-update
{
  \"filter\": {
    \"status\": \"To Do\",
    \"priority\": \"Medium\"
  },
  \"updates\": {
    \"priority\": \"High\",
    \"labels\": { \"add\": [\"urgent\"] }
  }
}
```

**Response (200 OK):**
```json
{
  \"updated\": 15,
  \"failed\": 0,
  \"duration\": \"2.5s\"
}
```

## Work Logs

### Log Work

```http
POST /issues/{id}/worklogs
{
  \"timeSpentSeconds\": 3600,
  \"started\": \"2026-03-22T09:00:00Z\",
  \"comment\": \"Implemented login fix\"
}
```

**Response (201 Created):**
```json
{
  \"id\": \"worklog-id\",
  \"issueKey\": \"PROJ-123\",
  \"timeSpentSeconds\": 3600,
  \"timeSpent\": \"1h\",
  \"author\": \"john.doe@company.com\",
  \"started\": \"2026-03-22T09:00:00Z\",
  \"comment\": \"Implemented login fix\"
}
```

### Get Work Logs

```http
GET /issues/{id}/worklogs
```

**Response (200 OK):**
```json
{
  \"worklogs\": [
    {
      \"id\": \"worklog-1\",
      \"timeSpent\": \"2h\",
      \"author\": \"John Doe\",
      \"started\": \"2026-03-22T09:00:00Z\",
      \"comment\": \"Investigation\"
    }
  ],
  \"totalTimeSpent\": \"5h 30m\"
}
```

### Bulk Log Work

```http
POST /worklogs/bulk-create
{
  \"entries\": [
    {
      \"issueKey\": \"PROJ-123\",
      \"timeSpentSeconds\": 7200,
      \"started\": \"2026-03-22T09:00:00Z\",
      \"comment\": \"Development\"
    },
    {
      \"issueKey\": \"PROJ-124\",
      \"timeSpentSeconds\": 3600,
      \"started\": \"2026-03-22T11:00:00Z\",
      \"comment\": \"Testing\"
    }
  ]
}
```

## Sync

### Get Sync Status

```http
GET /sync/status
```

**Response (200 OK):**
```json
{
  \"status\": \"healthy\",
  \"lastSync\": \"2026-03-22T14:30:00Z\",
  \"webhookStatus\": \"active\",
  \"queueLength\": 0,
  \"failedSyncs\": 0,
  \"syncRate\": \"150 issues/hour\"
}
```

### Force Sync

```http
POST /sync/force
{
  \"project\": \"PROJ\",
  \"sinceDays\": 7
}
```

**Response (202 Accepted):**
```json
{
  \"jobId\": \"sync-job-123\",
  \"status\": \"queued\",
  \"estimatedDuration\": \"5 minutes\"
}
```

### Sync Job Status

```http
GET /sync/jobs/{jobId}
```

**Response (200 OK):**
```json
{
  \"jobId\": \"sync-job-123\",
  \"status\": \"in-progress\",
  \"progress\": 65,
  \"processedIssues\": 130,
  \"totalIssues\": 200,
  \"startedAt\": \"2026-03-22T14:35:00Z\"
}
```

## Webhooks

### Register Webhook (Internal)

```http
POST /webhooks
X-Hub-Signature: sha256=...
Content-Type: application/json

{
  \"webhookEvent\": \"jira:issue_updated\",
  \"issue\": {
    \"key\": \"PROJ-123\",
    \"fields\": { ... }
  },
  \"changelog\": { ... }
}
```

**Response (200 OK):**
```json
{
  \"received\": true,
  \"processed\": true
}
```

## Error Responses

### 400 Bad Request

```json
{
  \"type\": \"ValidationError\",
  \"title\": \"Validation failed\",
  \"status\": 400,
  \"errors\": {
    \"title\": [\"Title is required\"],
    \"type\": [\"Invalid issue type\"]
  }
}
```

### 401 Unauthorized

```json
{
  \"type\": \"AuthenticationError\",
  \"title\": \"Authentication required\",
  \"status\": 401,
  \"detail\": \"JWT token is missing or invalid\"
}
```

### 403 Forbidden

```json
{
  \"type\": \"AuthorizationError\",
  \"title\": \"Insufficient permissions\",
  \"status\": 403,
  \"detail\": \"You don't have permission to create issues in this project\"
}
```

### 404 Not Found

```json
{
  \"type\": \"NotFoundError\",
  \"title\": \"Resource not found\",
  \"status\": 404,
  \"detail\": \"Issue with ID '...' not found\"
}
```

### 409 Conflict

```json
{
  \"type\": \"ConflictError\",
  \"title\": \"Resource conflict\",
  \"status\": 409,
  \"detail\": \"Issue has been modified by another user\",
  \"conflictDetails\": {
    \"localVersion\": 5,
    \"remoteVersion\": 6
  }
}
```

### 429 Too Many Requests

```json
{
  \"type\": \"RateLimitError\",
  \"title\": \"Rate limit exceeded\",
  \"status\": 429,
  \"detail\": \"Maximum 100 requests per minute exceeded\",
  \"retryAfter\": 30
}
```

### 500 Internal Server Error

```json
{
  \"type\": \"InternalError\",
  \"title\": \"Internal server error\",
  \"status\": 500,
  \"detail\": \"An unexpected error occurred\",
  \"traceId\": \"00-4bf92f3577b34da6a3ce929d0e0e4736-00\"
}
```

## Pagination

All list endpoints support pagination:

**Request:**
```http
GET /issues?page=2&pageSize=20
```

**Response:**
```json
{
  \"items\": [...],
  \"totalCount\": 250,
  \"page\": 2,
  \"pageSize\": 20,
  \"totalPages\": 13,
  \"hasNextPage\": true,
  \"hasPreviousPage\": true
}
```

## Filtering & Sorting

**Filters:**
```http
GET /issues?type=Bug&priority=High&status=Open&assignee=john.doe
```

**Sorting:**
```http
GET /issues?sortBy=createdAt&sortOrder=desc
```

**Combined:**
```http
GET /issues?type=Bug&status=Open&sortBy=priority&sortOrder=desc&page=1&pageSize=20
```

## OpenAPI/Swagger

Interactive API documentation available at:
```
https://workhub.company.com/swagger/index.html
```

## Code Examples

### C# (.NET)

```csharp
var client = new HttpClient { BaseAddress = new Uri(\"https://workhub.company.com\") };
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(\"Bearer\", token);

var request = new
{
    title = \"Login bug\",
    description = \"Button not working\",
    type = \"Bug\"
};

var response = await client.PostAsJsonAsync(\"/api/jira/issues/quick-create\", request);
var result = await response.Content.ReadFromJsonAsync<IssueResponse>();
```

### TypeScript

```typescript
const response = await fetch('https://workhub.company.com/api/jira/issues/quick-create', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    title: 'Login bug',
    description: 'Button not working',
    type: 'Bug'
  })
});

const issue = await response.json();
console.log(`Created issue: ${issue.jiraKey}`);
```

### Python

```python
import requests

headers = {
    'Authorization': f'Bearer {token}',
    'Content-Type': 'application/json'
}

data = {
    'title': 'Login bug',
    'description': 'Button not working',
    'type': 'Bug'
}

response = requests.post(
    'https://workhub.company.com/api/jira/issues/quick-create',
    headers=headers,
    json=data
)

issue = response.json()
print(f\"Created issue: {issue['jiraKey']}\")
```

---

**Last Updated:** 2026-03-22
