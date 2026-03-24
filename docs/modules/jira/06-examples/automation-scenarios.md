# Automation Scenarios

Real-world automation examples for Jira Module.

## Scenario 1: Auto-Assign Bugs to Component Owner

**Goal:** Automatically assign bugs to the right person based on component.

### Configuration

```json
{
  \"automation\": {
    \"name\": \"Auto-assign bugs by component\",
    \"trigger\": \"issue_created\",
    \"conditions\": [
      { \"field\": \"type\", \"operator\": \"equals\", \"value\": \"Bug\" }
    ],
    \"actions\": [
      {
        \"type\": \"assign\",
        \"logic\": \"component_owner\",
        \"mappings\": {
          \"Authentication\": \"john.doe@company.com\",
          \"Payment\": \"jane.smith@company.com\",
          \"UI\": \"bob.wilson@company.com\",
          \"API\": \"alice.brown@company.com\"
        },
        \"default\": \"tech-lead@company.com\"
      },
      {
        \"type\": \"set_priority\",
        \"value\": \"High\"
      },
      {
        \"type\": \"add_label\",
        \"value\": \"auto-assigned\"
      }
    ]
  }
}
```

### How It Works

**Trigger:**
```
User creates bug: \"Login timeout error\"
Component: Authentication (auto-detected from \"login\" keyword)
↓
Automation fires
↓
Checks: Type = Bug? ✅
↓
Actions:
1. Assign to john.doe@company.com (Auth owner)
2. Set priority to High
3. Add label \"auto-assigned\"
↓
Issue ready for john.doe to work on!
```

### Benefits

✅ **No manual assignment:** Saves 30 seconds per bug  
✅ **Right person immediately:** No routing delays  
✅ **Consistent priority:** All bugs treated equally  
✅ **Audit trail:** \"auto-assigned\" label for tracking  

### Expected Impact

- **Daily bugs:** ~20 bugs/day
- **Time saved:** 20 × 30s = 10 minutes/day
- **Response time:** Improved by 2-4 hours (instant notification to right person)

---

## Scenario 2: Auto-Add to Current Sprint (High Priority)

**Goal:** High priority items automatically added to current sprint.

### Configuration

```json
{
  \"automation\": {
    \"name\": \"Auto-add high priority to sprint\",
    \"trigger\": \"issue_created\",
    \"conditions\": [
      { \"field\": \"priority\", \"operator\": \"in\", \"value\": [\"Highest\", \"High\"] },
      { \"field\": \"status\", \"operator\": \"equals\", \"value\": \"Approved\" }
    ],
    \"actions\": [
      {
        \"type\": \"add_to_sprint\",
        \"sprint\": \"current\",
        \"skip_if_full\": false
      },
      {
        \"type\": \"set_story_points\",
        \"value\": \"auto_estimate\"
      },
      {
        \"type\": \"notify\",
        \"target\": \"team\",
        \"message\": \"High priority issue {issueKey} added to current sprint\"
      }
    ]
  }
}
```

### How It Works

**Example:**
```
PM creates feature: \"Critical bug fix for payment\"\nPriority: Highest\nStatus: Approved\n↓\nAutomation checks:\n- Priority = Highest? ✅\n- Status = Approved? ✅\n↓\nActions:\n1. Add to Sprint 15 (current sprint)\n2. Auto-estimate: 5 story points (ML model)\n3. Notify team on Slack: \"PROJ-789 added to sprint\"\n↓\nTeam sees new high-priority work immediately\n```

### Benefits

✅ **Immediate visibility:** Team knows about urgent work  
✅ **No sprint planning delay:** Work starts faster  
✅ **Auto-estimation:** Baseline for planning  
✅ **Team notification:** No one misses it  

---

## Scenario 3: Auto-Label from Keywords

**Goal:** Tag issues based on keywords for better organization.

### Configuration

```json
{
  \"automation\": {
    \"name\": \"Auto-label from keywords\",
    \"trigger\": \"issue_created\",
    \"actions\": [
      {
        \"type\": \"keyword_labeling\",
        \"rules\": [
          {
            \"keywords\": [\"mobile\", \"ios\", \"android\", \"app\"],
            \"label\": \"mobile\"
          },
          {
            \"keywords\": [\"security\", \"vulnerability\", \"CVE\", \"exploit\"],
            \"label\": \"security\",
            \"priority_override\": \"Highest\",
            \"notify\": \"security-team@company.com\"
          },
          {
            \"keywords\": [\"customer\", \"client\", \"support ticket\"],
            \"label\": \"customer-reported\"
          },
          {
            \"keywords\": [\"performance\", \"slow\", \"timeout\", \"loading\"],
            \"label\": \"performance\"
          },
          {
            \"keywords\": [\"ui\", \"button\", \"layout\", \"design\"],
            \"label\": \"ui\"
          }
        ]
      }
    ]
  }
}
```

### How It Works

**Example:**
```\nTitle: \"Mobile app login security vulnerability\"\nDescription: \"Customer reported...\"\n↓\nKeyword detection:\n- \"mobile\" → add label \"mobile\" ✅\n- \"security\" → add label \"security\" + priority \"Highest\" + notify security team ✅\n- \"customer\" → add label \"customer-reported\" ✅\n↓\nResult:\nLabels: [\"mobile\", \"security\", \"customer-reported\"]\nPriority: Highest (upgraded)\nNotified: security-team@company.com\n```

### Benefits

✅ **Consistent tagging:** No human error  
✅ **Searchable:** Easy to find related issues  
✅ **Smart routing:** Security team alerted automatically  
✅ **Priority adjustment:** Critical issues escalated  

---

## Scenario 4: Conditional Template Selection

**Goal:** Apply different templates based on issue context.

### Configuration

```json
{
  \"automation\": {
    \"name\": \"Smart template selection\",
    \"trigger\": \"before_create\",
    \"logic\": \"template_selection\",
    \"rules\": [
      {
        \"if\": { \"keywords\": [\"customer\", \"support\"] },
        \"template\": \"support-escalation\"
      },
      {
        \"if\": { \"keywords\": [\"security\", \"vulnerability\"] },
        \"template\": \"security-incident\",\n        \"additional\": {\n          \"priority\": \"Highest\",\n          \"assignee\": \"security-team@company.com\",\n          \"sla\": \"4 hours\"\n        }\n      },\n      {\n        \"if\": { \"keywords\": [\"urgent\", \"critical\", \"blocker\"] },\n        \"template\": \"hotfix\"\n      },\n      {\n        \"if\": { \"type\": \"Bug\" },\n        \"template\": \"bug-report\"\n      },\n      {\n        \"if\": { \"type\": \"Feature\" },\n        \"template\": \"feature-request\"\n      }\n    ],\n    \"default\": \"basic-task\"\n  }\n}\n```\n\n### How It Works\n\n**Example 1: Customer escalation**\n```\nUser types: \"Customer reporting payment failure\"\n↓\nDetects keyword: \"customer\"\n↓\nApplies: \"support-escalation\" template\n↓\nTemplate includes:\n- Customer info fields\n- Ticket link field\n- SLA timer\n- Support team notification\n```\n\n**Example 2: Security incident**\n```\nUser types: \"Security vulnerability in API\"\n↓\nDetects keyword: \"security\"\n↓\nApplies: \"security-incident\" template\n↓\nAutomatic actions:\n- Priority: Highest\n- Assignee: security-team@company.com\n- SLA: 4 hours\n- Notification: CTO + Security team\n- Label: \"security-incident\"\n```\n\n### Benefits\n\n✅ **Right template automatically:** User doesn't choose  \n✅ **Faster creation:** Less thinking required  \n✅ **Complete information:** Templates ensure nothing missing  \n✅ **Proper escalation:** Critical issues get attention  \n\n---\n\n## Scenario 5: Auto-Close Stale Issues\n\n**Goal:** Close issues inactive for 30 days with no response.\n\n### Configuration\n\n```json\n{\n  \"automation\": {\n    \"name\": \"Close stale issues\",\n    \"trigger\": \"scheduled\",\n    \"schedule\": \"daily\",\n    \"conditions\": [\n      { \"field\": \"status\", \"operator\": \"in\", \"value\": [\"Waiting for Customer\", \"Needs Info\"] },\n      { \"field\": \"updated\", \"operator\": \"older_than\", \"value\": \"30 days\" },\n      { \"field\": \"comment_count\", \"operator\": \"equals\", \"value\": 0, \"since\": \"30 days\" }\n    ],\n    \"actions\": [\n      {\n        \"type\": \"add_comment\",\n        \"text\": \"Auto-closing due to no response for 30 days. Please reopen if still relevant.\"\n      },\n      {\n        \"type\": \"transition\",\n        \"to\": \"Closed\",\n        \"resolution\": \"Incomplete\"\n      },\n      {\n        \"type\": \"add_label\",\n        \"value\": \"auto-closed\"\n      }\n    ]\n  }\n}\n```\n\n### How It Works\n\n**Daily check:**\n```\n1. Find issues:\n   - Status: \"Waiting for Customer\" OR \"Needs Info\"\n   - Last updated: > 30 days ago\n   - No comments in last 30 days\n\n2. For each matching issue:\n   - Add comment explaining closure\n   - Change status to \"Closed\"\n   - Set resolution to \"Incomplete\"\n   - Add label \"auto-closed\"\n   - Send notification to reporter\n```\n\n**Example:**\n```\nPROJ-123 - \"Need screenshot to reproduce\"\nStatus: Waiting for Customer\nLast updated: 2026-02-20 (31 days ago)\nComments: 0 since request\n↓\nAuto-closed on 2026-03-23\nComment: \"Auto-closing due to no response for 30 days...\"\nLabel: \"auto-closed\"\nResolution: \"Incomplete\"\n```\n\n### Benefits\n\n✅ **Clean backlog:** Old issues don't clutter  \n✅ **Accurate metrics:** Only active work counted  \n✅ **Respectful:** Comment explains why  \n✅ **Reversible:** User can reopen if needed  \n\n**Impact:** Typical team sees 10-20 stale issues closed weekly\n\n---\n\n## Scenario 6: Auto-Create Sub-Tasks for Features\n\n**Goal:** Break down features into standard sub-tasks.\n\n### Configuration\n\n```json\n{\n  \"automation\": {\n    \"name\": \"Create feature sub-tasks\",\n    \"trigger\": \"issue_created\",\n    \"conditions\": [\n      { \"field\": \"type\", \"operator\": \"equals\", \"value\": \"Feature\" },\n      { \"field\": \"story_points\", \"operator\": \"greater_than\", \"value\": 5 }\n    ],\n    \"actions\": [\n      {\n        \"type\": \"create_subtasks\",\n        \"subtasks\": [\n          {\n            \"summary\": \"Design: {parent.title}\",\n            \"type\": \"Sub-task\",\n            \"assignee\": \"design-team@company.com\",\n            \"estimate\": \"2h\"\n          },\n          {\n            \"summary\": \"Backend: {parent.title}\",\n            \"type\": \"Sub-task\",\n            \"assignee\": \"backend-team@company.com\",\n            \"estimate\": \"1d\"\n          },\n          {\n            \"summary\": \"Frontend: {parent.title}\",\n            \"type\": \"Sub-task\",\n            \"assignee\": \"frontend-team@company.com\",\n            \"estimate\": \"1d\"\n          },\n          {\n            \"summary\": \"Testing: {parent.title}\",\n            \"type\": \"Sub-task\",\n            \"assignee\": \"qa-team@company.com\",\n            \"estimate\": \"4h\"\n          },\n          {\n            \"summary\": \"Documentation: {parent.title}\",\n            \"type\": \"Sub-task\",\n            \"assignee\": \"tech-writer@company.com\",\n            \"estimate\": \"2h\"\n          }\n        ]\n      }\n    ]\n  }\n}\n```\n\n### How It Works\n\n**Example:**\n```\nFeature created: \"Add dark mode\"\nStory points: 8 (large feature)\n↓\nAutomation creates 5 sub-tasks:\n\n1. PROJ-790: \"Design: Add dark mode\"\n   Assigned: design-team@company.com\n   Estimate: 2h\n\n2. PROJ-791: \"Backend: Add dark mode\"\n   Assigned: backend-team@company.com\n   Estimate: 1d\n\n3. PROJ-792: \"Frontend: Add dark mode\"\n   Assigned: frontend-team@company.com\n   Estimate: 1d\n\n4. PROJ-793: \"Testing: Add dark mode\"\n   Assigned: qa-team@company.com\n   Estimate: 4h\n\n5. PROJ-794: \"Documentation: Add dark mode\"\n   Assigned: tech-writer@company.com\n   Estimate: 2h\n↓\nEach team notified of their sub-task\n```\n\n### Benefits\n\n✅ **Standardized breakdown:** Consistent approach  \n✅ **Parallel work:** Teams work simultaneously  \n✅ **Clear ownership:** Each team knows their part  \n✅ **Time savings:** No manual sub-task creation  \n\n---\n\n## Scenario 7: Link Related Issues by AI\n\n**Goal:** Automatically link similar or related issues.\n\n### Configuration\n\n```json\n{\n  \"automation\": {\n    \"name\": \"AI-powered issue linking\",\n    \"trigger\": \"issue_created\",\n    \"actions\": [\n      {\n        \"type\": \"find_similar\",\n        \"method\": \"ai_embedding\",\n        \"threshold\": 0.8,\n        \"max_results\": 5,\n        \"link_type\": \"relates to\"\n      },\n      {\n        \"type\": \"find_duplicates\",\n        \"threshold\": 0.95,\n        \"action\": \"comment_and_link\",\n        \"comment\": \"⚠️ Possible duplicate of {linked_issue}. Please verify.\"\n      }\n    ]\n  }\n}\n```\n\n### How It Works\n\n**Example:**\n```\nNew bug: \"Payment fails on Safari browser\"\n↓\nAI analyzes:\n- Title vectors\n- Description similarity\n- Component\n- Labels\n↓\nFinds similar issues:\n- PROJ-100: \"Payment timeout on Safari\" (95% match)\n- PROJ-200: \"Safari payment bug\" (92% match)\n- PROJ-300: \"Checkout fails Safari\" (88% match)\n↓\nActions:\n1. High match (95%) → Comment: \"⚠️ Possible duplicate\"\n2. Medium matches → Link as \"relates to\"\n3. Notify reporter: \"Similar issues found\"\n```\n\n### Benefits\n\n✅ **Prevent duplicates:** Find before creating  \n✅ **Knowledge reuse:** See past solutions  \n✅ **Context:** Related issues provide insights  \n✅ **Faster resolution:** Learn from similar cases  \n\n---\n\n## Implementing Automations\n\n### Step 1: Define Automation\n\n```\nSettings → Jira → Automations → Create New\n```\n\n### Step 2: Choose Trigger\n\n- Issue created\n- Issue updated\n- Status changed\n- Scheduled (daily/weekly)\n- Custom event\n\n### Step 3: Set Conditions\n\n```json\n{\n  \"conditions\": [\n    { \"field\": \"type\", \"operator\": \"equals\", \"value\": \"Bug\" },\n    { \"field\": \"priority\", \"operator\": \"in\", \"value\": [\"High\", \"Highest\"] }\n  ],\n  \"logic\": \"AND\"\n}\n```\n\n### Step 4: Configure Actions\n\nSelect from:\n- Assign issue\n- Change status\n- Add/remove labels\n- Set fields\n- Create sub-tasks\n- Send notification\n- Run custom script\n\n### Step 5: Test\n\n```\n1. Enable \"Test Mode\"\n2. Create test issue matching conditions\n3. Verify automation triggered\n4. Check actions executed correctly\n5. Review logs\n```\n\n### Step 6: Enable & Monitor\n\n```\n1. Enable automation\n2. Monitor execution logs\n3. Track success rate\n4. Adjust rules as needed\n5. Document for team\n```\n\n## Best Practices\n\n### 1. Start Simple\n✅ One automation at a time  \n✅ Test thoroughly  \n❌ Don't create 10 automations on day 1  \n\n### 2. Document Rules\n✅ Clear name describing what it does  \n✅ Comment why automation exists  \n✅ Share with team  \n\n### 3. Monitor & Iterate\n✅ Check logs weekly  \n✅ Adjust based on feedback  \n✅ Remove unused automations  \n\n### 4. Respect Users\n✅ Don't override manual actions  \n✅ Provide escape hatch  \n✅ Explain automation in comment  \n\n### 5. Performance\n✅ Limit actions per automation  \n✅ Avoid infinite loops  \n✅ Use conditions to filter  \n\n---\n\n**Last Updated:** 2026-03-22
