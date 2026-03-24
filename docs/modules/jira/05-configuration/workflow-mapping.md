# Workflow Mapping Configuration

Map Jira workflow states to WorkHub statuses for seamless synchronization.

## Understanding Workflows

### Jira Workflow

Jira uses customizable workflows with multiple statuses:
```
To Do → In Progress → In Review → Testing → Done
```

### WorkHub Simplified Status

WorkHub uses simplified status for UI:
```
Open → In Progress → Done
```

**Mapping required** to translate between systems.

## Default Mapping

### Built-in Mappings

```json
{
  \"mappings\": {
    \"Open\": [\"To Do\", \"Backlog\", \"Open\", \"New\"],
    \"InProgress\": [\"In Progress\", \"In Development\", \"Working\"],
    \"Done\": [\"Done\", \"Closed\", \"Resolved\", \"Completed\"]
  }
}
```

**How it works:**
- Jira "To Do" → WorkHub "Open"
- Jira "In Progress" → WorkHub "InProgress"
- Jira "Done" → WorkHub "Done"

### Reverse Mapping

```json
{
  \"reverseMapping\": {
    \"Open\": \"To Do\",
    \"InProgress\": \"In Progress\",
    \"Done\": \"Done\"
  }
}
```

When WorkHub → Jira:
- WorkHub "Open" → Jira "To Do"
- WorkHub "InProgress" → Jira "In Progress"
- WorkHub "Done" → Jira "Done"

## Custom Mapping

### Add Custom Status

**Scenario:** Your team uses "In Review" status

**Steps:**
1. Settings → Jira → Workflow Mapping
2. Click "Add Custom Mapping"
3. Configure:
```
WorkHub Status: InProgress
Jira Statuses: ["In Review", "Code Review", "QA Review"]
```

### Example Custom Workflow

**Your Jira workflow:**
```
Backlog → Ready → In Dev → Code Review → QA → Staging → Done
```

**WorkHub mapping:**
```json
{
  \"mappings\": {
    \"Open\": [\"Backlog\", \"Ready\"],
    \"InProgress\": [\"In Dev\", \"Code Review\", \"QA\", \"Staging\"],
    \"Done\": [\"Done\"]
  },
  \"reverseMapping\": {
    \"Open\": \"Backlog\",
    \"InProgress\": \"In Dev\",
    \"Done\": \"Done\"
  }
}
```

**Result:**
- Jira "Backlog" or "Ready" → Show as "Open" in WorkHub
- Jira "In Dev/Code Review/QA/Staging" → Show as "InProgress" in WorkHub
- Jira "Done" → Show as "Done" in WorkHub

## Transition Rules

### Required Fields

Some Jira transitions require fields:

```json
{
  \"transitions\": {
    \"Done\": {
      \"requires\": [\"resolution\", \"testResult\"],
      \"validates\": {
        \"resolution\": [\"Fixed\", \"Won't Fix\", \"Duplicate\"],
        \"testResult\": \"must not be empty\"
      }
    }
  }
}
```

### Conditional Transitions

```json
{
  \"transitions\": {
    \"In Progress\": {
      \"conditions\": [
        {
          \"field\": \"assignee\",
          \"operator\": \"is not empty\"
        }
      ]
    }
  }
}
```

**Effect:**
- Cannot move to "In Progress" without assignee
- WorkHub auto-assigns to current user if empty

## Sync Behavior

### Bidirectional Sync

**Jira → WorkHub:**
```
User changes status in Jira → "To Do" to "In Progress"
↓
Webhook fires
↓
WorkHub updates: "Open" to "InProgress"
↓
UI refreshes
```

**WorkHub → Jira:**
```
User clicks "Start Work" in WorkHub
↓
WorkHub status: "Open" to "InProgress"
↓
Maps to Jira: "To Do" to "In Progress"
↓
API call to Jira
↓
Jira updates
```

### Conflict Resolution

**Scenario:** Simultaneous edits

```
10:00 - User A changes in WorkHub: Open → InProgress
10:00 - User B changes in Jira: To Do → Done
10:01 - Both sync → Conflict!
```

**Resolution Strategy (configurable):**

1. **Last-Write-Wins (default):**
```json
{
  \"conflictResolution\": \"last-write-wins\"
}
```
Result: Jira change wins (happened slightly later)

2. **Manual Resolution:**
```json
{
  \"conflictResolution\": \"manual\"
}
```
Result: Show dialog to user
```
Conflict Detected:
Your change: Open → InProgress
Jira change: To Do → Done
[Accept Mine] [Accept Theirs] [Review]
```

## Configuration UI

### Step 1: Open Workflow Settings

```
Settings → Jira → Workflow Mapping
```

### Step 2: View Current Mappings

Shows table:
```
┌────────────────┬──────────────────────────────────┐
│ WorkHub Status │ Jira Statuses                    │
├────────────────┼──────────────────────────────────┤
│ Open           │ To Do, Backlog, Open, New        │
│ InProgress     │ In Progress, In Development      │
│ Done           │ Done, Closed, Resolved           │
└────────────────┴──────────────────────────────────┘
```

### Step 3: Edit Mapping

```
1. Click "Edit" on row
2. Modify Jira statuses (comma-separated)
3. Click "Save"
4. Test: Create issue and change status
```

### Step 4: Set Reverse Mapping

```
When WorkHub changes status to "InProgress":
→ Change Jira to: [Select: In Progress ▼]

Options from your Jira workflow:
- In Progress
- In Development
- Working On It
```

## Advanced Configuration

### Status Categories

Jira groups statuses into categories:
```json
{
  \"categories\": {
    \"todo\": [\"To Do\", \"Backlog\", \"Open\"],
    \"inprogress\": [\"In Progress\", \"In Review\"],
    \"done\": [\"Done\", \"Closed\"]
  }
}
```

**Auto-map by category:**
```json
{
  \"useCategories\": true,
  \"categoryMapping\": {
    \"todo\": \"Open\",
    \"inprogress\": \"InProgress\",
    \"done\": \"Done\"
  }
}
```

### Multi-Step Transitions

Some workflows require multiple steps:
```
To Do → cannot go directly to Done
To Do → In Progress → Done (valid)
```

**Configure:**
```json
{
  \"multiStepTransitions\": {
    \"To Do\": {
      \"to\": \"Done\",
      \"via\": [\"In Progress\"]
    }
  }
}
```

**Effect:**
- User clicks "Mark Done" on "To Do" issue
- WorkHub automatically:
  1. Transition to "In Progress"
  2. Then transition to "Done"

## Testing Mappings

### Test Workflow

1. **Create test issue:**
```
Title: "Test workflow mapping"
Status: To Do
```

2. **Change in WorkHub:**
```
WorkHub: Open → InProgress
Expected Jira: To Do → In Progress
```

3. **Verify:**
```
✅ Jira status updated?
✅ WorkHub shows correct status?
✅ No errors in sync logs?
```

4. **Change in Jira:**
```
Jira: In Progress → Done
Expected WorkHub: InProgress → Done
```

5. **Verify:**
```
✅ WorkHub status updated?
✅ Change synced within 30 seconds?
```

6. **Clean up:**
```
Delete test issue
```

## Troubleshooting

### Status Not Syncing

**Check:**
1. Mapping configured correctly
2. Status exists in Jira
3. User has permission to transition
4. Workflow allows transition
5. Required fields filled

**Debug:**
```
Settings → Jira → Sync Logs
→ Filter by issue key
→ Look for errors
```

### Wrong Status in WorkHub

**Cause:** Mapping incorrect

**Fix:**
1. Check mapping table
2. Verify Jira status name (case-sensitive!)
3. Update mapping
4. Force re-sync issue

### Transition Blocked

**Common causes:**
```
❌ "Cannot transition: Resolution required"
→ Add resolution before marking Done

❌ "Cannot transition: Invalid transition"
→ Workflow doesn't allow direct transition
→ Use multi-step transitions

❌ "Cannot transition: Permission denied"
→ User lacks Jira permissions
→ Contact Jira admin
```

## Best Practices

### 1. Keep Mappings Simple
✅ Group similar statuses
✅ Use 3-5 WorkHub statuses max
❌ Don't map every Jira status separately

### 2. Use Categories
✅ Map by category when possible
✅ Simpler configuration
✅ More maintainable

### 3. Document Custom Workflows
✅ Document why custom mapping exists
✅ Share with team
✅ Review when workflow changes

### 4. Test After Changes
✅ Test workflow after editing mappings
✅ Test all transitions
✅ Verify bidirectional sync

### 5. Monitor Sync Logs
✅ Check logs weekly
✅ Fix mapping errors promptly
✅ Keep audit trail

## Example Configurations

### Simple 3-Step Workflow

```json
{
  \"mappings\": {
    \"Open\": [\"To Do\"],
    \"InProgress\": [\"In Progress\"],
    \"Done\": [\"Done\"]
  }
}
```

### Complex Development Workflow

```json
{
  \"mappings\": {
    \"Open\": [\"Backlog\", \"Ready for Dev\"],
    \"InProgress\": [
      \"In Development\",
      \"Code Review\",
      \"QA Testing\",
      \"Staging\"
    ],
    \"Done\": [\"Released\", \"Closed\"]
  },
  \"reverseMapping\": {
    \"Open\": \"Backlog\",
    \"InProgress\": \"In Development\",
    \"Done\": \"Released\"
  }
}
```

### Support Ticket Workflow

```json
{
  \"mappings\": {
    \"Open\": [\"New\", \"Waiting for Customer\"],
    \"InProgress\": [
      \"Investigating\",
      \"Escalated\",
      \"Waiting for Engineering\"
    ],
    \"Done\": [\"Resolved\", \"Closed - Won't Fix\"]
  }
}
```

---

**Last Updated:** 2026-03-22
