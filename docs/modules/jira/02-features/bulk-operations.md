# Thao Tác Hàng Loạt (Bulk Operations)

Xử lý nhiều issues cùng lúc, tiết kiệm hàng giờ công việc thủ công.

## Tổng Quan

**Vấn đề:** Cập nhật từng issue một rất tốn thời gian  
**Giải pháp:** Bulk operations cho create, update, assign, log work

**Tiết kiệm:** Từ 30 giây/issue → 5 giây/issue với batches

## Use Cases

### Khi Nào Dùng Bulk Operations

✅ **Sprint planning:** Assign 20+ issues vào sprint mới  
✅ **Team rebalancing:** Reassign issues khi team member nghỉ  
✅ **Status updates:** Move completed issues to "Done"  
✅ **Mass import:** Import issues từ CSV/Excel  
✅ **Label management:** Add/remove labels theo pattern  
✅ **Priority adjustment:** Update priorities sau meeting  
✅ **Work logging:** Log time cho multiple issues  

## Bulk Create

### From CSV Import

**CSV format:**
```csv
Summary,Type,Priority,Assignee,Description,Labels
"Fix login bug",Bug,High,john@company.com,"Login not working on mobile","bug,mobile"
"Add dark mode",Feature,Medium,jane@company.com,"Users want dark theme","feature,ui"
"Update documentation",Task,Low,admin@company.com,"Docs outdated","docs,maintenance"
```

**Import flow:**
```
1. Jira → Bulk Operations → Import from CSV
2. Upload CSV file
3. Map columns to Jira fields:
   Summary → Summary
   Type → Issue Type
   Priority → Priority
   Assignee → Assignee (email)
   Description → Description
   Labels → Labels (comma-separated)
4. Preview (shows first 5 rows)
5. Validate (checks all rows)
6. Import (creates issues in batches)
```

**Validation:**
```typescript
interface ImportValidation {
  totalRows: number;
  validRows: number;
  invalidRows: number;
  errors: Array<{
    row: number;
    field: string;
    error: string;
    value: string;
  }>;
}

// Common errors:
// - Invalid email (assignee not found)
// - Invalid issue type
// - Missing required fields
// - Invalid priority value
```

**Example validation result:**
```
Validation Results:
├── Total Rows: 100
├── Valid: 95 ✅
└── Errors: 5 ❌
    ├── Row 12: Invalid assignee email "nonexistent@company.com"
    ├── Row 45: Missing required field "Summary"
    ├── Row 67: Invalid issue type "Bugg" (typo)
    ├── Row 78: Priority "Ultra High" not valid
    └── Row 89: Description too long (max 32,000 chars)

Options:
[Fix Errors and Re-upload] [Skip Error Rows] [Cancel]
```

### From Template Batch

**Scenario:** Create multiple similar issues from template

```
1. Select Template: "Bug Report"
2. Enter count: 10
3. Customize each:
   - Issue 1: "Fix button on homepage"
   - Issue 2: "Fix dropdown on settings"
   - Issue 3: "Fix table on dashboard"
   - ...
4. Batch Create → 10 issues created in 5 seconds
```

**Quick batch creation:**
```typescript
interface BatchCreateRequest {
  templateId: string;
  issues: Array<{
    summary: string;
    description?: string;
    customFields?: Record<string, any>;
  }>;
}

// API call
POST /api/jira/issues/batch-create
{
  "templateId": "bug-template-001",
  "issues": [
    { "summary": "Fix button on homepage" },
    { "summary": "Fix dropdown on settings" },
    { "summary": "Fix table on dashboard" }
  ]
}

// Response
{
  "created": 3,
  "failed": 0,
  "issues": [
    { "key": "PROJ-123", "summary": "Fix button on homepage" },
    { "key": "PROJ-124", "summary": "Fix dropdown on settings" },
    { "key": "PROJ-125", "summary": "Fix table on dashboard" }
  ]
}
```

## Bulk Assign

### Assign by Filter

**Scenario:** Assign all bugs to QA team lead

```
1. Jira → Issues → Filter:
   - Type: Bug
   - Status: Ready for QA
   - Assignee: Unassigned
   
2. Shows: 15 issues matching
   PROJ-100: Login validation bug
   PROJ-101: Payment form bug
   PROJ-102: Dashboard display bug
   ...

3. Bulk Actions → Assign To → Select: Jane Doe (QA Lead)

4. Confirm: "Assign 15 issues to Jane Doe?"
   [Yes] [No]

5. Processing:
   ✅ PROJ-100 assigned
   ✅ PROJ-101 assigned
   ✅ PROJ-102 assigned
   ...
   Done: 15 issues assigned in 3 seconds
```

### Team Distribution

**Scenario:** Balance workload across team

```typescript
interface TeamDistribution {
  strategy: 'round-robin' | 'by-capacity' | 'by-skill';
  team: Array<{
    userId: string;
    name: string;
    currentWorkload: number;  // Current assigned issues
    capacity: number;          // Max issues
    skills: string[];          // For skill-based assignment
  }>;
}

// Example: Round-robin distribution
function distributeRoundRobin(issues: Issue[], team: User[]): Assignment[] {
  const assignments: Assignment[] = [];
  let currentIndex = 0;
  
  for (const issue of issues) {
    const assignee = team[currentIndex];
    assignments.push({ issue: issue.key, assignee: assignee.id });
    currentIndex = (currentIndex + 1) % team.length;
  }
  
  return assignments;
}

// Example: By capacity
function distributeByCapacity(issues: Issue[], team: User[]): Assignment[] {
  // Sort by available capacity (ascending)
  const sorted = team.sort((a, b) => 
    (a.capacity - a.currentWorkload) - (b.capacity - b.currentWorkload)
  );
  
  // Assign to least loaded first
  const assignments: Assignment[] = [];
  for (const issue of issues) {
    const assignee = sorted[0]; // Least loaded
    assignments.push({ issue: issue.key, assignee: assignee.id });
    assignee.currentWorkload++;
    sorted.sort((a, b) => 
      (a.capacity - a.currentWorkload) - (b.capacity - b.currentWorkload)
    );
  }
  
  return assignments;
}
```

**UI for team distribution:**
```
┌─────────────────────────────────────────────────┐
│ Distribute 30 Issues Across Team               │
├─────────────────────────────────────────────────┤
│ Strategy: ⦿ By Capacity  ○ Round Robin         │
│                                                 │
│ Team Members:                                   │
│ ☑ John Doe    (5/10 issues)  ███░░░░░░░        │
│ ☑ Jane Smith  (8/10 issues)  ████████░░        │
│ ☑ Bob Wilson  (2/10 issues)  ██░░░░░░░░        │
│ ☐ Alice Brown (10/10 issues) ██████████  FULL  │
│                                                 │
│ Preview:                                        │
│ → John will get 10 issues (15 total)           │
│ → Jane will get 8 issues (16 total)            │
│ → Bob will get 12 issues (14 total)            │
│                                                 │
│ [Cancel] [Distribute]                           │
└─────────────────────────────────────────────────┘
```

## Bulk Update

### Update Multiple Fields

**Scenario:** Update all issues in sprint

```
1. Filter: Sprint = "Sprint 15"
2. Select: 25 issues
3. Bulk Edit:
   - Set Priority: High
   - Add Label: sprint-15
   - Set Story Points: 3
   - Update Epic Link: PROJ-500
4. Preview changes
5. Apply → Updates 25 issues
```

**API:**
```typescript
POST /api/jira/issues/bulk-update
{
  "filter": {
    "sprint": "Sprint 15",
    "status": "To Do"
  },
  "updates": {
    "priority": "High",
    "labels": { "add": ["sprint-15"] },
    "storyPoints": 3,
    "epicLink": "PROJ-500"
  }
}

// Response
{
  "updated": 25,
  "failed": 0,
  "duration": "2.5s"
}
```

### Status Transitions

**Scenario:** Move all completed issues to "Done"

```
1. Filter:
   - Status: "In Review"
   - Resolution: Fixed
   - Updated: Last 7 days
   
2. Shows: 12 issues

3. Bulk Transition → "Done"

4. Validation:
   - Check workflow allows transition
   - Check required fields filled
   - Preview transition

5. Execute:
   Transitioning 12 issues to Done...
   ✅ PROJ-100 → Done
   ✅ PROJ-101 → Done
   ⚠️  PROJ-102 → Failed (resolution required)
   ...
   
   Summary: 11 successful, 1 failed
```

**Workflow validation:**
```typescript
async function validateBulkTransition(
  issues: string[],
  toStatus: string
): Promise<ValidationResult> {
  const results: ValidationResult[] = [];
  
  for (const issueKey of issues) {
    const issue = await jiraApi.getIssue(issueKey);
    const transitions = await jiraApi.getTransitions(issueKey);
    
    // Check if transition available
    const canTransition = transitions.some(t => t.to.name === toStatus);
    
    if (!canTransition) {
      results.push({
        issueKey,
        valid: false,
        error: `Cannot transition from ${issue.status} to ${toStatus}`
      });
      continue;
    }
    
    // Check required fields
    const transition = transitions.find(t => t.to.name === toStatus);
    const missingFields = checkRequiredFields(issue, transition);
    
    if (missingFields.length > 0) {
      results.push({
        issueKey,
        valid: false,
        error: `Missing fields: ${missingFields.join(', ')}`
      });
      continue;
    }
    
    results.push({ issueKey, valid: true });
  }
  
  return {
    totalIssues: issues.length,
    valid: results.filter(r => r.valid).length,
    invalid: results.filter(r => !r.valid).length,
    details: results
  };
}
```

### Label Management

**Add labels to multiple issues:**
```
Select issues → Bulk Actions → Add Labels
→ Enter labels: "urgent", "customer-reported"
→ Apply to 15 issues
```

**Remove labels:**
```
Select issues → Bulk Actions → Remove Labels
→ Select labels to remove: [x] old-sprint [x] deprecated
→ Apply
```

**Replace labels:**
```
Find: "sprint-14"
Replace with: "sprint-15"
→ Updates all matching issues
```

## Bulk Log Work

### Scenario 1: Log Same Time to Multiple Issues

**Use case:** Team meeting discussed 5 issues, log 30 mins each

```
1. Select 5 issues:
   PROJ-100, PROJ-101, PROJ-102, PROJ-103, PROJ-104

2. Bulk Log Work:
   Time spent: 30 minutes
   Date: Today
   Comment: "Team meeting discussion"

3. Apply → 30 minutes logged to each issue (150 min total)
```

### Scenario 2: Log Different Times

**Use case:** Retroactive logging for last week

```
CSV format:
IssueKey,TimeSpent,Date,Comment
PROJ-100,2h,2026-03-15,"Feature development"
PROJ-101,3h,2026-03-15,"Bug investigation"
PROJ-102,1h,2026-03-16,"Code review"
PROJ-103,4h,2026-03-17,"Implementation"

Import → Logs work for each issue
```

**API:**
```typescript
POST /api/jira/worklogs/bulk-create
{
  "entries": [
    {
      "issueKey": "PROJ-100",
      "timeSpentSeconds": 7200,  // 2 hours
      "started": "2026-03-15T09:00:00.000Z",
      "comment": "Feature development"
    },
    {
      "issueKey": "PROJ-101",
      "timeSpentSeconds": 10800,  // 3 hours
      "started": "2026-03-15T10:00:00.000Z",
      "comment": "Bug investigation"
    }
  ]
}
```

## Performance

### Batching Strategy

**Small batches for reliability:**
```typescript
async function bulkUpdate(issues: string[], updates: any) {
  const BATCH_SIZE = 10;
  const batches = chunk(issues, BATCH_SIZE); // Split into batches of 10
  
  for (const batch of batches) {
    await Promise.all(
      batch.map(issueKey => jiraApi.updateIssue(issueKey, updates))
    );
    
    // Progress update
    emitProgress({
      processed: batches.indexOf(batch) * BATCH_SIZE,
      total: issues.length
    });
    
    // Small delay to avoid rate limits
    await sleep(100);
  }
}
```

**Progress indication:**
```
Processing 100 issues...
[████████████████░░░░░░░░░░░░] 60% (60/100)
Estimated time remaining: 15 seconds
```

### Rate Limiting

**Respect Jira API limits:**
```typescript
const rateLimiter = new Bottleneck({
  maxConcurrent: 5,    // Max 5 parallel requests
  minTime: 100         // Minimum 100ms between requests
});

async function bulkOperation(issues: string[], operation: (key: string) => Promise<void>) {
  await Promise.all(
    issues.map(issueKey =>
      rateLimiter.schedule(() => operation(issueKey))
    )
  );
}
```

### Timeout Handling

```typescript
async function bulkUpdateWithTimeout(issues: string[], updates: any, timeoutMs: number = 30000) {
  const results = {
    succeeded: [] as string[],
    failed: [] as { issueKey: string; error: string }[],
    timedOut: [] as string[]
  };
  
  for (const issueKey of issues) {
    try {
      await Promise.race([
        jiraApi.updateIssue(issueKey, updates),
        timeout(timeoutMs)
      ]);
      results.succeeded.push(issueKey);
    } catch (error) {
      if (error.name === 'TimeoutError') {
        results.timedOut.push(issueKey);
      } else {
        results.failed.push({ issueKey, error: error.message });
      }
    }
  }
  
  return results;
}
```

## Partial Failures

### Handling Errors

**Scenario:** Bulk update 100 issues, 5 fail

```
Bulk Update Results:
├── Successful: 95 ✅
├── Failed: 5 ❌
│   ├── PROJ-123: Permission denied (need admin access)
│   ├── PROJ-456: Validation failed (invalid priority)
│   ├── PROJ-789: Issue locked (in transition)
│   ├── PROJ-100: Rate limit exceeded (retry later)
│   └── PROJ-200: Network timeout
└── Actions:
    [Retry Failed] [Export Failed] [Ignore]
```

**Retry logic:**
```typescript
async function retryFailed(failed: string[], updates: any, maxRetries: number = 3) {
  const stillFailing: string[] = [];
  
  for (const issueKey of failed) {
    let success = false;
    
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        await jiraApi.updateIssue(issueKey, updates);
        success = true;
        break;
      } catch (error) {
        if (attempt < maxRetries) {
          // Exponential backoff
          await sleep(Math.pow(2, attempt) * 1000);
        }
      }
    }
    
    if (!success) {
      stillFailing.push(issueKey);
    }
  }
  
  return stillFailing;
}
```

### Rollback on Critical Failure

**Option:** Rollback all changes if any fails

```typescript
async function bulkUpdateWithRollback(issues: string[], updates: any) {
  // Save original state
  const originalStates = await Promise.all(
    issues.map(key => jiraApi.getIssue(key))
  );
  
  try {
    // Try bulk update
    await bulkUpdate(issues, updates);
  } catch (error) {
    // Rollback on failure
    await Promise.all(
      originalStates.map(original =>
        jiraApi.updateIssue(original.key, original.fields)
      )
    );
    
    throw new Error('Bulk update failed, changes rolled back');
  }
}
```

## Best Practices

### 1. Preview Before Execute
✅ Always preview changes on first 5-10 issues  
❌ Don't blindly bulk update hundreds of issues

### 2. Use Filters Carefully
✅ Verify filter returns expected issues  
❌ Don't assume filter is correct

### 3. Start Small
✅ Test on 10 issues first  
❌ Don't start with 1000 issues

### 4. Backup Critical Data
✅ Export before bulk delete  
❌ Don't delete without backup

### 5. Monitor Progress
✅ Watch for errors during execution  
❌ Don't walk away during bulk operations

### 6. Handle Failures Gracefully
✅ Retry failed operations  
❌ Don't ignore failures

## Limits

**System limits:**
- **Max batch size:** 100 issues per request
- **Max CSV rows:** 10,000 rows
- **API rate limit:** 100 requests/minute
- **Timeout:** 30 seconds per operation
- **Max file size:** 10 MB for CSV

**Recommendations:**
- For > 100 issues: Split into multiple batches
- For > 1000 issues: Schedule overnight job
- For very large operations: Contact admin

## Security

**Permission checks:**
```typescript
async function checkBulkPermissions(userId: string, issues: string[], operation: string) {
  // Check user has permission for ALL issues
  const permissions = await Promise.all(
    issues.map(key => jiraApi.checkPermission(userId, key, operation))
  );
  
  const denied = permissions.filter(p => !p.hasPermission);
  
  if (denied.length > 0) {
    throw new Error(
      `No permission for ${denied.length} issues: ${denied.map(d => d.issueKey).join(', ')}`
    );
  }
}
```

**Audit trail:**
```typescript
interface BulkOperationAudit {
  id: string;
  performedBy: string;
  timestamp: DateTime;
  operation: 'create' | 'update' | 'delete' | 'assign' | 'transition';
  issueCount: number;
  issueKeys: string[];
  changes: Record<string, any>;
  success: boolean;
  errors: string[];
}

// All bulk operations logged for audit
```

## Examples

### Example 1: Sprint Planning

```
Task: Assign 30 backlog issues to Sprint 15

1. Filter: Status = "Backlog" AND Priority = "High"
2. Select: All 30 issues
3. Bulk Update:
   - Sprint: Sprint 15
   - Status transition: "To Do"
4. Bulk Assign: Round-robin to dev team (5 members)
5. Execute

Result: 30 issues ready for sprint in < 10 seconds
```

### Example 2: End of Sprint Cleanup

```
Task: Move all completed issues to Done, log remaining time

1. Filter: Sprint = "Sprint 14" AND Status = "In Review"
2. Validate: All have resolution
3. Bulk Transition: "Done"
4. Bulk Log Work: 
   - Automatically calculate remaining estimate
   - Log to complete time tracking
5. Execute

Result: Sprint 14 closed out properly
```

### Example 3: Bug Triage

```
Task: Triage 50 new bugs from support

1. Import from CSV (support tickets)
2. All created as Type = Bug, Status = "New"
3. AI Auto-classification:
   - By component (based on description)
   - By priority (based on keywords)
4. Bulk Assign: Component owners
5. Bulk Add Labels: "support-reported", "needs-investigation"

Result: 50 bugs triaged and assigned in < 2 minutes
```

## Troubleshooting

### Bulk Operation Stuck

**Symptoms:** Progress bar frozen

**Solutions:**
1. Check network connection
2. Check Jira API status
3. Cancel and retry with smaller batch
4. Check browser console for errors

### Some Issues Not Updated

**Check:**
- Permissions for those issues
- Workflow allows transition
- Required fields filled
- Issue not locked

### Performance Slow

**Optimization:**
- Reduce batch size
- Check API rate limits
- Schedule for off-peak hours
- Contact admin to increase quotas

## Next Steps

- [Filters Guide](../03-user-guides/advanced-filters.md) - Create effective filters
- [CSV Import Template](../06-examples/csv-import-template.md) - Download template
- [API Reference](../04-technical/api-endpoints.md#bulk-operations) - Technical details

---

**Last Updated:** 2026-03-22
