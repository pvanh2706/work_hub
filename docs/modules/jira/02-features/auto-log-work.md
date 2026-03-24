# Tự Động Log Work - Theo Dõi Thời Gian Chính Xác

Tự động ghi nhận work time từ commits và work sessions, giúp PM track progress chính xác mà không cần developers thao tác thủ công.

## Tổng Quan

**Vấn đề:** Developers quên log work, time tracking không chính xác, reports sai lệch  
**Giải pháp:** Tự động log work từ Git commits và work sessions

**Tiết kiệm:** 1-2 phút/issue × 10+ issues/ngày = 10-20 phút/ngày

## Cách Hoạt Động

### 1. Git Commit Integration

**Commit message format:**
```bash
git commit -m "feat(auth): implement login #PROJ-123"
```

**Parsing logic:**
```typescript
interface CommitInfo {
  type: 'feat' | 'fix' | 'refactor' | 'test', // From conventional commits
  scope: 'auth',                                // Module name
  message: 'implement login',                   // Description
  issueKey: 'PROJ-123',                        // Extracted from #PROJ-123
  timestamp: '2026-03-22T10:30:00Z'
}
```

**Auto-log rules:**
```typescript
function estimateWorkTime(commit: CommitInfo): number {
  const baseTime = 30; // minutes default
  
  // Adjust based on commit type
  const typeMultipliers = {
    'feat': 1.5,    // Features take longer → 45 min
    'fix': 1.0,     // Bug fixes → 30 min
    'refactor': 1.2, // Refactoring → 36 min
    'test': 0.8,    // Writing tests → 24 min
    'docs': 0.5     // Documentation → 15 min
  };
  
  // Adjust based on file changes
  const filesChanged = commit.stats.filesChanged;
  const linesAdded = commit.stats.linesAdded;
  const linesDeleted = commit.stats.linesDeleted;
  
  let timeEstimate = baseTime * typeMultipliers[commit.type];
  
  // More files = more time
  if (filesChanged > 5) timeEstimate *= 1.3;
  if (filesChanged > 10) timeEstimate *= 1.5;
  
  // Large diffs = more time
  const totalChanges = linesAdded + linesDeleted;
  if (totalChanges > 100) timeEstimate *= 1.2;
  if (totalChanges > 500) timeEstimate *= 1.5;
  
  // Cap at 4 hours per commit
  return Math.min(timeEstimate, 240);
}
```

**Example:**
```bash
# Commit 1: Small bug fix
git commit -m "fix(auth): resolve login redirect #PROJ-123"
# → Auto-logs 30 minutes to PROJ-123

# Commit 2: Large feature
git commit -m "feat(payment): add Stripe integration #PROJ-456"
# Changed 15 files, +800 lines
# → Auto-logs 90 minutes to PROJ-456

# Commit 3: Multiple issues
git commit -m "fix(ui): button styles #PROJ-789 #PROJ-790"
# → Auto-logs 15 minutes to each issue (split evenly)
```

### 2. Work Session Tracking

**Start work session:**
```
User clicks "Start Work" on PROJ-123
→ Timer starts
→ Status auto-changed to "In Progress" in Jira
```

**Session tracking:**
```typescript
interface WorkSession {
  issueKey: string;
  startTime: DateTime;
  endTime: DateTime | null;
  pauses: PausePeriod[];
  manualAdjustments: Adjustment[];
  source: 'manual' | 'auto';
}

interface PausePeriod {
  startTime: DateTime;
  endTime: DateTime;
  reason: 'lunch' | 'meeting' | 'context-switch' | 'other';
}
```

**Auto-pause detection:**
```typescript
// Pause tự động khi:
- Không có activity 10 phút → Auto-pause
- User switches to other app > 5 phút → Auto-pause
- System detects idle (no keyboard/mouse) → Auto-pause
- User manually pauses → Manual pause

// Resume khi:
- User returns to app
- User clicks "Resume"
- Auto-resume on activity detected
```

**Stop work session:**
```
User clicks "Stop Work"
→ Calculate total time (excluding pauses)
→ Show confirmation: "Log 1h 25m to PROJ-123?"
→ User confirms → Auto-log to Jira
```

### 3. Manual Override

**Adjust logged time:**
```
History → Find logged entry → Edit
→ Change time or delete
→ Reason required for audit trail
```

**Manual logging:**
```
# Still available for edge cases
Issue → Log Work → Enter time manually
```

## Configuration

### Enable/Disable

```json
// User preferences
{
  "autoLogWork": {
    "enabled": true,
    "sources": {
      "gitCommits": true,      // Auto-log from commits
      "workSessions": true,    // Auto-log from sessions
      "manualOnly": false      // Disable all auto-logging
    }
  }
}
```

### Time Estimation Rules

```json
{
  "timeEstimation": {
    "base": {
      "feat": 45,      // minutes
      "fix": 30,
      "refactor": 36,
      "test": 24,
      "docs": 15
    },
    "multipliers": {
      "filesChanged": {
        "threshold": 5,
        "multiplier": 1.3
      },
      "linesChanged": {
        "threshold": 100,
        "multiplier": 1.2
      }
    },
    "max": 240,        // Cap at 4 hours
    "min": 10          // Minimum 10 minutes
  }
}
```

### Time Rounding

```json
{
  "rounding": {
    "enabled": true,
    "interval": 15,    // Round to nearest 15 minutes
    "direction": "up"  // Always round up
  }
}
```

**Example:**
- Actual: 23 minutes → Logged: 30 minutes
- Actual: 47 minutes → Logged: 45 minutes
- Actual: 8 minutes → Logged: 15 minutes (minimum)

### Default Time Estimates

```json
{
  "defaults": {
    "commit": 30,           // Default per commit
    "smallFix": 15,         // Quick fixes
    "mediumTask": 60,       // Typical tasks
    "largeFeature": 120     // Major features
  }
}
```

## Accuracy

### How System Estimates Time

**Data sources:**
1. **Commit metadata:**
   - Files changed
   - Lines added/deleted
   - Commit message type
   
2. **Historical data:**
   - Similar issues in past
   - Team averages
   - Individual velocity

3. **Machine learning (optional):**
   - Learn from corrections
   - Improve estimates over time
   - Per-team calibration

**Accuracy improvement:**
```typescript
interface AccuracyTracking {
  estimated: number;      // Auto-estimated time
  actual: number;         // Manual correction (if any)
  accuracy: number;       // Percentage accuracy
  
  // Learn from corrections
  feedback: {
    wasAccurate: boolean;
    userAdjustment: number;
    adjustmentReason: string;
  }
}

// Over time, system learns:
// - Your coding speed
// - Task complexity patterns
// - Module-specific time needs
```

### Validation

**Sanity checks przed auto-log:**
```typescript
function validateLogEntry(entry: WorkLogEntry): ValidationResult {
  // Check 1: Time reasonable?
  if (entry.timeSpent > 8 * 60) // > 8 hours
    return invalid("Time too long for single entry");
  
  // Check 2: Multiple logs collision?
  if (hasOverlappingLogs(entry))
    return invalid("Overlapping time entries");
  
  // Check 3: Issue still open?
  if (issue.status === 'Closed')
    return warning("Logging time to closed issue");
  
  return valid();
}
```

## Manual Override

### When to Override

**Common scenarios:**
- Pair programming (2 people, log time for each)
- Meetings about issue (log meeting time)
- Research/investigation (no commit, but work done)
- Commit bundled multiple issues (split time differently)

### How to Adjust

**UI flow:**
```
1. Navigate to: Jira → Work Logs
2. Find entry: Filter by date/issue
3. Click Edit: Shows current log
4. Adjust time: Enter correct time
5. Add reason: "Included 30m meeting"
6. Save: Updates Jira + keeps audit trail
```

**Audit trail:**
```typescript
interface WorkLogAudit {
  id: string;
  issueKey: string;
  originalTime: number;    // Auto-logged: 45 min
  adjustedTime: number;    // Manual: 75 min
  adjustment: number;      // Difference: +30 min
  reason: string;          // "Included design review meeting"
  adjustedBy: string;      // User ID
  adjustedAt: DateTime;    // Timestamp
}
```

### Bulk Adjustments

**Scenario:** Forgot to log work for a week

**Solution:**
```
Jira → Bulk Log Work
→ Select issues: PROJ-123, PROJ-124, PROJ-125
→ Enter times: 2h, 3h, 1.5h
→ Date: 2026-03-15
→ Reason: "Retroactive logging for last week"
→ Submit → Logs to all issues
```

## Reports

### Personal Time Report

**View your logged time:**
```
Reports → My Work Log
→ Filter by:
  - Date range
  - Project
  - Issue type
→ Shows:
  - Total time logged
  - By issue
  - By day/week/month
  - Auto vs manual split
```

**Export options:**
- Excel
- CSV
- PDF
- Send to email

### Team Time Report

**For PMs/managers:**
```
Reports → Team Work Log
→ Select team members
→ Date range
→ Shows:
  - Total team time
  - Per person breakdown
  - Per project breakdown
  - Velocity trends
```

**Insights:**
```typescript
interface TeamInsights {
  totalLogged: number;
  averagePerPerson: number;
  mostProductiveDays: DayOfWeek[];
  peakHours: Hour[];
  
  accuracy: {
    autoVsManual: {
      auto: 75%,      // 75% auto-logged
      manual: 25%     // 25% manual adjustments
    },
    estimateAccuracy: 85%  // 85% accurate estimates
  }
}
```

### Issue Time Breakdown

**For specific issue:**
```
PROJ-123 → Work Log tab
→ Shows:
  - Total time: 8h 30m
  - Breakdown:
    - Auto-logged from commits: 6h 15m
    - Manual work sessions: 2h 15m
  - Contributors: John (5h), Jane (3.5h)
  - Timeline: Graph showing when work happened
```

## Integration with Jira

### Jira Work Log Format

**API call:**
```typescript
// WorkHub sends to Jira
POST /rest/api/3/issue/{issueKey}/worklog
{
  "timeSpentSeconds": 3600,  // 1 hour
  "started": "2026-03-22T10:00:00.000+0000",
  "comment": "Auto-logged from commit: feat(auth): implement login",
  "author": {
    "accountId": "5b10a2844c20165700ede21g"
  },
  "visibility": {
    "type": "group",
    "value": "jira-developers"
  }
}
```

**Jira displays:**
```
Work Log:
┌──────────────────────────────────────┐
│ 1h logged by John Doe                │
│ On Mar 22, 2026 at 10:00 AM         │
│                                      │
│ Comment:                             │
│ Auto-logged from commit:             │
│ feat(auth): implement login          │
│                                      │
│ Via: WorkHub Auto-Logging ⚡         │
└──────────────────────────────────────┘
```

### Sync Status

**Real-time sync indicator:**
```
✅ Synced - Last logged 2 minutes ago
⏳ Syncing... - Sending to Jira
❌ Sync failed - Retry in 5 minutes
🔄 Queued - Will sync when online
```

### Offline Support

**When network unavailable:**
```typescript
interface QueuedWorkLog {
  id: string;
  issueKey: string;
  timeSpent: number;
  startedAt: DateTime;
  comment: string;
  status: 'queued' | 'syncing' | 'synced' | 'failed';
  retryCount: number;
  queuedAt: DateTime;
}

// When connection restored:
- Sync queued entries in order
- Show progress: "Syncing 5 of 12 entries..."
- Report failures for manual review
```

## Best Practices

### 1. Commit Often
✅ Small, frequent commits → More accurate time tracking  
❌ Large, infrequent commits → Less accurate estimates

### 2. Use Conventional Commits
✅ `feat(auth): add login` → Clear type, better estimates  
❌ `WIP` or `fix stuff` → Hard to estimate

### 3. Reference Issues in Commits
✅ `fix: button style #PROJ-123` → Auto-linked  
❌ `fix: button style` → No link, no auto-log

### 4. Start Work Sessions for Long Tasks
✅ Start timer for 2+ hour tasks → Accurate time  
❌ Manual log later → Forget actual time

### 5. Review Logs Weekly
✅ Check and adjust logs at end of week  
❌ Let inaccuracies accumulate

### 6. Provide Feedback on Estimates
✅ Adjust wrong estimates → System learns  
❌ Ignore inaccurate logs → System stays dumb

## Troubleshooting

### Work Not Auto-Logged

**Possible causes:**
1. **Issue key not in commit message**
   - Fix: Use `#PROJ-123` format
   
2. **Auto-logging disabled**
   - Fix: Settings → Jira → Enable "Auto-log from commits"
   
3. **No permission to log work**
   - Fix: Check Jira permissions or ask admin

4. **Sync failed**
   - Fix: Check network, retry manually

### Duplicate Logs

**Possible causes:**
- Manual log + auto-log for same commit
- Timer still running when commit made

**Solution:**
```
Work Log → Find duplicate → Delete one
System learns to avoid this pattern in future
```

### Time Seems Wrong

**Too high:**
- Large commit might be overestimated
- Solution: Adjust manually, provide feedback

**Too low:**
- Simple commit might be underestimated
- Solution: Adjust manually, system learns

### Cannot Edit Log Entry

**Possible causes:**
- Entry locked (after time period)
- No permission (edit others' logs)

**Solution:**
- Ask PM/admin to unlock
- Request permission change

## Privacy Considerations

**What's tracked:**
✅ Work time per issue  
✅ Commit metadata (files, lines changed)  
✅ Work session durations  

**What's NOT tracked:**
❌ Actual code content  
❌ Keyboard/mouse activity details  
❌ Screen captures  
❌ Personal browsing  

**Control:**
- Pause tracking anytime
- Delete your own logs
- Export your data
- Opt-out of auto-logging

## Metrics

**Typical team results:**

| Metric | Before WorkHub | After WorkHub | Improvement |
|--------|---------------|--------------|-------------|
| Time tracking compliance | 30-40% | 90-95% | +150% |
| Logging time overhead | 10-20 min/day | 0-2 min/day | -90% |
| Estimate accuracy | 60-70% | 80-90% | +30% |
| PM insights quality | Low (missing data) | High (complete data) | Significant |

## Next Steps

- [Git Integration Setup](../05-configuration/git-integration.md) - Configure commit parsing
- [Work Sessions Guide](../03-user-guides/for-developers.md#work-sessions) - Use timer effectively
- [Reports](../03-user-guides/for-project-managers.md#reports) - View time reports

---

**Last Updated:** 2026-03-22
