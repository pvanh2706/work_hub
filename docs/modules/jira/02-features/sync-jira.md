# Đồng Bộ Với Jira - Real-time Two-Way Sync

Đồng bộ hai chiều giữa WorkHub và Jira, đảm bảo data luôn nhất quán và up-to-date.

## Tổng Quan

**Vấn đề:** Changes trên Jira không reflect trong WorkHub và ngược lại  
**Giải pháp:** Bidirectional sync với webhooks (real-time) + polling (backup)

## Sync Architecture

```
┌─────────────┐         ┌──────────────┐         ┌────────────┐
│   WorkHub   │◄───────►│  Sync Engine │◄───────►│    Jira    │
│  Database   │         │              │         │   Cloud    │
└─────────────┘         └──────────────┘         └────────────┘
      │                        │                        │
      │                        │                        │
   Changes                  Detects                 Changes
   locally                  conflicts               remotely
      │                        │                        │
      ▼                        ▼                        ▼
  Update UI          Conflict resolution          Webhook
                          queue                    trigger
```

## Sync Directions

### 1. Jira → WorkHub (Real-time)

**Webhook setup:**
```typescript
// Jira sends webhook when changes happen
interface JiraWebhook {
  webhookEvent: 'jira:issue_updated' | 'jira:issue_created' | 'jira:issue_deleted';
  issue: {
    key: string;
    fields: {
      summary: string;
      status: { name: string };
      assignee: { accountId: string };
      priority: { name: string };
      updated: string;
      // ... other fields
    };
  };
  changelog: {
    items: Array<{
      field: string;
      fromString: string;
      toString: string;
    }>;
  };
  user: {
    accountId: string;
    displayName: string;
  };
  timestamp: number;
}
```

**Processing webhooks:**
```typescript
async function handleJiraWebhook(payload: JiraWebhook) {
  const issueKey = payload.issue.key;
  
  // 1. Find local copy
  const localIssue = await db.jiraIssues.findByKey(issueKey);
  
  // 2. Check if newer
  if (localIssue && localIssue.updatedAt > payload.issue.fields.updated) {
    // Local is newer → queue conflict resolution
    await conflictQueue.add({ issueKey, remoteData: payload });
    return;
  }
  
  // 3. Update local database
  await db.jiraIssues.upsert({
    key: issueKey,
    summary: payload.issue.fields.summary,
    status: payload.issue.fields.status.name,
    assignee: payload.issue.fields.assignee,
    priority: payload.issue.fields.priority.name,
    updatedAt: payload.issue.fields.updated,
    syncedAt: new Date(),
    syncStatus: 'synced'
  });
  
  // 4. Notify UI (via SignalR)
  await signalR.notifyIssueUpdated(issueKey);
  
  // 5. Trigger domain events
  await eventBus.publish(new IssueUpdatedFromJira(issueKey, payload.changelog));
}
```

**Events subscribed:**
- ✅ `jira:issue_created` - New issue created
- ✅ `jira:issue_updated` - Issue fields changed
- ✅ `jira:issue_deleted` - Issue deleted
- ✅ `jira:worklog_updated` - Work logged
- ✅ `jira:comment_created` - Comment added
- ⚠️ `jira:version_released` - Optional
- ⚠️ `jira:sprint_started` - Optional

### 2. WorkHub → Jira (On Change)

**Trigger sync on WorkHub changes:**
```typescript
// When user updates issue in WorkHub
async function updateIssueInWorkHub(issueKey: string, updates: IssueUpdates) {
  // 1. Update local database
  await db.jiraIssues.update(issueKey, updates);
  
  // 2. Queue sync to Jira (async)
  await syncQueue.add({
    type: 'update',
    issueKey,
    updates,
    retryCount: 0,
    queuedAt: new Date()
  });
  
  // 3. Attempt immediate sync
  try {
    await jiraApi.updateIssue(issueKey, updates);
    await db.jiraIssues.updateSyncStatus(issueKey, 'synced');
  } catch (error) {
    // Will retry from queue
    await db.jiraIssues.updateSyncStatus(issueKey, 'pending');
  }
  
  // 4. Return to user immediately (optimistic UI)
  return { success: true, syncStatus: 'pending' };
}
```

**Sync queue processing:**
```typescript
// Background worker processes queue
async function processSyncQueue() {
  while (true) {
    const jobs = await syncQueue.getNext(10); // Batch of 10
    
    for (const job of jobs) {
      try {
        await syncToJira(job);
        await syncQueue.complete(job.id);
      } catch (error) {
        if (job.retryCount < 3) {
          // Retry with exponential backoff
          await syncQueue.retry(job.id, {
            delay: Math.pow(2, job.retryCount) * 1000 // 1s, 2s, 4s
          });
        } else {
          // Max retries → mark as failed
          await syncQueue.fail(job.id, error.message);
          await notifyUser(job.userId, 'Sync failed for ' + job.issueKey);
        }
      }
    }
    
    await sleep(5000); // Check queue every 5 seconds
  }
}
```

### 3. Polling (Backup Sync)

**For reliability when webhooks miss:**
```typescript
// Runs every 15 minutes
async function pollJiraForChanges() {
  // 1. Get recently updated issues
  const recentUpdates = await jiraApi.search({
    jql: `updated >= -30m AND project = PROJ`,
    fields: ['key', 'summary', 'status', 'updated']
  });
  
  // 2. Compare with local database
  for (const jiraIssue of recentUpdates.issues) {
    const localIssue = await db.jiraIssues.findByKey(jiraIssue.key);
    
    if (!localIssue || jiraIssue.fields.updated > localIssue.updatedAt) {
      // Jira is newer → sync down
      await syncFromJira(jiraIssue);
    }
  }
  
  // 3. Check for pending WorkHub changes
  const pendingChanges = await db.jiraIssues.findBySyncStatus('pending');
  
  for (const issue of pendingChanges) {
    // Retry syncing to Jira
    await syncQueue.add({
      type: 'update',
      issueKey: issue.key,
      updates: issue.pendingUpdates
    });
  }
}
```

**Polling frequency:**
- Default: Every 15 minutes
- Configurable: 5 mins to 1 hour
- On-demand: Manual "Sync Now" button

## Sync Scope

### What Data Syncs

| Field | Jira → WorkHub | WorkHub → Jira | Notes |
|-------|----------------|----------------|-------|
| **Summary** | ✅ Yes | ✅ Yes | Issue title |
| **Description** | ✅ Yes | ✅ Yes | Full text |
| **Status** | ✅ Yes | ✅ Yes | With workflow validation |
| **Assignee** | ✅ Yes | ✅ Yes | User mapping required |
| **Priority** | ✅ Yes | ✅ Yes | High/Medium/Low |
| **Labels** | ✅ Yes | ✅ Yes | Array of strings |
| **Components** | ✅ Yes | ✅ Yes | Must exist in Jira |
| **Sprint** | ✅ Yes | ✅ Yes | If using sprints |
| **Story Points** | ✅ Yes | ✅ Yes | Numeric field |
| **Comments** | ✅ Yes | ✅ Yes | Full history |
| **Work Logs** | ✅ Yes | ✅ Yes | Time entries |
| **Attachments** | ✅ Yes | ⚠️ Partial | Upload only, not edit |
| **Issue Links** | ✅ Yes | ❌ No | Read-only |
| **Fix Versions** | ✅ Yes | ⚠️ Partial | Must exist in Jira |
| **Custom Fields** | ⚠️ Optional | ⚠️ Optional | Configurable |

**Custom field mapping:**
```json
{
  "customFieldMappings": {
    "customfield_10001": "storyPoints",     // Jira field ID → WorkHub field
    "customfield_10002": "epicLink",
    "customfield_10003": "customerTicket"
  }
}
```

### What Doesn't Sync

❌ **Issue history** - Too large, query on-demand  
❌ **Workflow transitions** - Different workflows in each system  
❌ **Watchers** - Different user bases  
❌ **Votes** - WorkHub doesn't have voting  
❌ **SLA data** - Jira-specific  

## Conflict Resolution

### When Conflicts Occur

**Scenario 1: Simultaneous edits**
```
10:00 - User A edits in WorkHub: Summary = "Fix login bug"
10:01 - User B edits in Jira: Summary = "Fix auth bug"
10:02 - Both sync → Conflict!
```

**Scenario 2: Network delay**
```
WorkHub offline → User edits locally
WorkHub reconnects → Local changes conflict with remote changes
```

### Resolution Strategies

#### 1. Last-Write-Wins (Default)

```typescript
function resolveConflict_LastWriteWins(local: Issue, remote: Issue): Issue {
  // Compare timestamps
  if (remote.updatedAt > local.updatedAt) {
    // Remote is newer → accept remote changes
    return remote;
  } else {
    // Local is newer → push local changes to remote
    syncToJira(local);
    return local;
  }
}
```

**Pros:** Simple, automatic  
**Cons:** Can lose changes

#### 2. Field-Level Merge

```typescript
function resolveConflict_FieldMerge(local: Issue, remote: Issue): Issue {
  const resolved = { ...local };
  
  // For each field, pick newer value
  if (remote.fields.summary.updatedAt > local.fields.summary.updatedAt) {
    resolved.summary = remote.summary;
  }
  
  if (remote.fields.status.updatedAt > local.fields.status.updatedAt) {
    resolved.status = remote.status;
  }
  
  // Arrays: merge uniquely
  resolved.labels = uniqueMerge(local.labels, remote.labels);
  
  return resolved;
}
```

**Pros:** Preserves more changes  
**Cons:** More complex

#### 3. Manual Resolution (Critical Issues)

```typescript
function resolveConflict_Manual(local: Issue, remote: Issue): ConflictDialog {
  // Show UI dialog to user
  return {
    type: 'conflict',
    issueKey: local.key,
    conflicts: [
      {
        field: 'summary',
        localValue: local.summary,
        remoteValue: remote.summary,
        localUpdatedBy: local.updatedBy,
        remoteUpdatedBy: remote.updatedBy,
        localUpdatedAt: local.updatedAt,
        remoteUpdatedAt: remote.updatedAt
      }
    ],
    actions: ['accept-local', 'accept-remote', 'merge-manual']
  };
}
```

**UI for manual resolution:**
```
┌─────────────────────────────────────────┐
│ Conflict Detected: PROJ-123            │
├─────────────────────────────────────────┤
│ Field: Summary                          │
│                                         │
│ Your Change (WorkHub):                  │
│ "Fix login bug on mobile"               │
│ By: You, 2 minutes ago                  │
│                                         │
│ Remote Change (Jira):                   │
│ "Fix authentication issue"              │
│ By: John Doe, 1 minute ago              │
│                                         │
│ [Accept Mine] [Accept Theirs] [Merge]  │
└─────────────────────────────────────────┘
```

### Configuration

```json
{
  "conflictResolution": {
    "strategy": "last-write-wins",  // or "field-merge" or "manual"
    "autoResolveAfter": 300,        // Auto-resolve after 5 minutes
    "notifyOnConflict": true,       // Notify user
    "criticalFields": [             // Always require manual resolution
      "status",
      "assignee"
    ]
  }
}
```

## Offline Handling

### Queue Changes When Offline

```typescript
interface OfflineChange {
  id: string;
  type: 'create' | 'update' | 'delete';
  issueKey: string;
  changes: any;
  timestamp: DateTime;
  synced: boolean;
  failureCount: number;
}

// User makes changes while offline
async function handleOfflineChange(change: OfflineChange) {
  // 1. Apply locally immediately
  await db.applyChange(change);
  
  // 2. Queue for sync
  await offlineQueue.add(change);
  
  // 3. Show indicator
  ui.showIndicator('📶 Offline - Changes will sync when online');
}
```

### Sync When Online

```typescript
// Detect when back online
window.addEventListener('online', async () => {
  ui.showIndicator('🔄 Syncing offline changes...');
  
  // Get all queued changes
  const queue = await offlineQueue.getAll();
  
  // Sync in order
  for (const change of queue) {
    try {
      await syncToJira(change);
      await offlineQueue.remove(change.id);
    } catch (error) {
      // Mark as failed, will retry
      await offlineQueue.markFailed(change.id);
    }
  }
  
  if (queue.length > 0) {
    ui.showNotification(`✅ Synced ${queue.length} changes`);
  }
});
```

### Offline Indicators

```typescript
// Visual feedback
interface SyncStatus {
  online: boolean;
  queuedChanges: number;
  lastSyncedAt: DateTime;
  syncInProgress: boolean;
}

// UI displays:
// ✅ Online - Last synced 2 minutes ago
// 📶 Offline - 3 changes queued
// 🔄 Syncing... 2 of 3 changes
// ❌ Sync failed - Retry in 30 seconds
```

## Monitoring

### Sync Status Dashboard

**For admins:**
```
Jira Sync Dashboard
├── Status: ✅ Healthy
├── Last Sync: 30 seconds ago
├── Webhook Status: ✅ Receiving
├── Queue Length: 0 pending
├── Failed Syncs: 2 (view details)
└── Sync Rate: 150 issues/hour
```

**Health checks:**
```typescript
interface SyncHealth {
  webhookStatus: 'healthy' | 'degraded' | 'down';
  apiStatus: 'healthy' | 'rate-limited' | 'down';
  queueLength: number;
  failureRate: number;        // Percentage
  avgSyncTime: number;        // Milliseconds
  lastSuccessfulSync: DateTime;
}

// Alert when:
- Webhook not received in 30 minutes
- API errors > 10% of requests
- Queue length > 100 items
- Sync time > 5 seconds average
```

### Sync Logs

**For debugging:**
```typescript
interface SyncLog {
  id: string;
  timestamp: DateTime;
  direction: 'to-jira' | 'from-jira';
  issueKey: string;
  operation: 'create' | 'update' | 'delete';
  fields: string[];            // Which fields changed
  success: boolean;
  error: string | null;
  duration: number;            // Milliseconds
  retryCount: number;
}

// View logs:
Admin → Jira → Sync Logs
→ Filter by: Issue, Date, Status
→ Export for analysis
```

### Metrics

**Track sync performance:**
```
Last 24 Hours:
├── Total Syncs: 1,247
├── Successful: 1,235 (99.0%)
├── Failed: 12 (1.0%)
├── Avg Sync Time: 450ms
├── P95 Sync Time: 1.2s
└── Data Transferred: 125 MB
```

## Webhook Setup

### Register Webhook in Jira

**Step 1: Create webhook in Jira**
```
1. Go to Jira Settings → System → Webhooks
2. Click "Create a WebHook"
3. Name: "WorkHub Sync"
4. Status: Enabled
5. URL: https://workhub.company.com/api/jira/webhooks
6. Events: Select all issue events
7. Save
```

**Step 2: Configure in WorkHub**
```json
{
  "jiraWebhooks": {
    "enabled": true,
    "endpoint": "/api/jira/webhooks",
    "secret": "webhook-secret-key-for-verification",
    "events": [
      "issue_created",
      "issue_updated",
      "issue_deleted",
      "worklog_updated",
      "comment_created"
    ]
  }
}
```

**Step 3: Verify webhook**
```bash
# Test webhook reception
curl -X POST https://workhub.company.com/api/jira/webhooks/test \
  -H "Content-Type: application/json" \
  -d '{"test": true}'

# Expected: 200 OK + confirmation message
```

### Security

**Webhook verification:**
```typescript
// Verify webhook is from Jira
function verifyWebhookSignature(payload: string, signature: string): boolean {
  const expectedSignature = crypto
    .createHmac('sha256', config.webhookSecret)
    .update(payload)
    .digest('hex');
  
  return crypto.timingSafeEqual(
    Buffer.from(signature),
    Buffer.from(expectedSignature)
  );
}

// Reject if signature doesn't match
if (!verifyWebhookSignature(request.body, request.headers['x-hub-signature'])) {
  return response.status(401).send('Invalid signature');
}
```

## Performance

### Optimization Strategies

**1. Batch operations:**
```typescript
// Instead of updating one by one
for (const issue of issues) {
  await updateInDatabase(issue); // Slow!
}

// Batch insert/update
await updateManyInDatabase(issues); // Fast!
```

**2. Selective sync:**
```typescript
// Only sync fields that changed
interface PartialUpdate {
  issueKey: string;
  changedFields: {
    [fieldName: string]: any;
  };
}

// Don't fetch entire issue if only status changed
```

**3. Caching:**
```typescript
// Cache user mappings, project configs, etc.
const cache = new Redis();

async function getUserMapping(jiraAccountId: string): string {
  const cached = await cache.get(`user:${jiraAccountId}`);
  if (cached) return cached;
  
  const userId = await db.users.findByJiraId(jiraAccountId);
  await cache.set(`user:${jiraAccountId}`, userId, 'EX', 3600); // 1 hour
  return userId;
}
```

**4. Rate limiting:**
```typescript
// Respect Jira API limits
const rateLimiter = new RateLimiter({
  points: 100,        // 100 requests
  duration: 60        // Per minute
});

await rateLimiter.consume(1); // Wait if limit exceeded
await jiraApi.updateIssue(...);
```

## Best Practices

### 1. Monitor Sync Health
✅ Check dashboard weekly  
✅ Investigate failures promptly  
✅ Keep webhook active

### 2. Handle Conflicts Gracefully
✅ Configure resolution strategy for team  
✅ Educate users on conflict scenarios  
✅ Review conflict logs

### 3. Test Sync Scenarios
✅ Test offline → online transitions  
✅ Test simultaneous edits  
✅ Test bulk operations

### 4. Keep Data Clean
✅ Regular sync audits  
✅ Delete stale issues  
✅ Archive old data

## Troubleshooting

### Webhook Not Receiving

**Check:**
1. Webhook registered and enabled in Jira
2. URL correct and accessible from Jira
3. Firewall not blocking Jira IPs
4. SSL certificate valid

**Test:**
```bash
# Simulate webhook from Jira
curl -X POST https://workhub.company.com/api/jira/webhooks \
  -H "Content-Type: application/json" \
  -H "X-Hub-Signature: test-signature" \
  -d @webhook-payload.json
```

### Sync Constantly Failing

**Causes:**
- API credentials invalid
- Rate limit exceeded
- Network issues
- Data validation errors

**Solutions:**
1. Check API token still valid
2. Reduce sync frequency
3. Check network connectivity
4. Review error logs for details

### Data Out of Sync

**Fix:**
```
Admin → Jira → Force Full Sync
→ Select project
→ Start sync

# Re-syncs all issues from Jira
```

## Next Steps

- [Webhook Configuration](../05-configuration/webhook-setup.md) - Setup webhooks
- [Conflict Resolution Guide](../03-user-guides/handling-conflicts.md) - Resolve conflicts
- [API Integration](../04-technical/jira-api-integration.md) - Technical details

---

**Last Updated:** 2026-03-22
