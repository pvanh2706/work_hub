# Hướng Dẫn Cho Project Managers

Quản lý team, templates, và theo dõi progress hiệu quả với Jira Module.

## Quick Start

### Lần Đầu Setup (5 Phút)

**1. Kết nối Jira (2 phút):**
```
Settings → Integrations → Jira
→ Connect with admin account
→ Grant permissions for team
```

**2. Setup default project (1 phút):**
```
Jira → Settings → Default Project: PROJ
→ All team issues created here by default
```

**3. Create first template (2 phút):**
```
Templates → New → Bug Report
→ Configure fields
→ Save as Team Template
```

## Core Responsibilities

### 1. Template Management

**Create Team Templates:**

**Bug Template:**
```markdown
## Bug Description
{title}

## Priority Justification
- Customer impact: [Select: High/Medium/Low]
- Affected users: [Number]
- Business value: [Describe]

## Steps to Reproduce
1. 
2. 
3. 

## Expected vs Actual
- Expected: 
- Actual: 

## Acceptance Criteria for Fix
- [ ] Bug no longer reproducible
- [ ] Tests added
- [ ] Documentation updated
```

**Feature Template:**
```markdown
## Feature Overview
{title}

## Business Value
- Problem it solves:
- Expected ROI:
- Target users:

## User Story
As a [role]
I want [capability]
So that [benefit]

## Acceptance Criteria
- [ ] AC1:
- [ ] AC2:
- [ ] AC3:

## Out of Scope
- 
-
```

**Manage Templates:**
```
Templates → Team Templates
→ View usage stats
→ Edit/Archive unused
→ Share across projects
```

### 2. Team Configuration

**Component Owners:**
```
Settings → Components
→ Authentication: John Doe (auto-assign bugs)
→ Payment: Jane Smith
→ UI/UX: Bob Wilson
→ API: Alice Brown
```

**Auto-Assignment Rules:**
```json
{
  "rules": [
    {
      "if": { "type": "Bug", "component": "Authentication" },
      "then": { "assign": "johndoe", "priority": "High" }
    },
    {
      "if": { "type": "Feature", "labels": ["customer-request"] },
      "then": { "assign": "pm-team", "addToSprint": "current" }
    }
  ]
}
```

**Sprint Defaults:**
```
Settings → Sprints
→ Default duration: 2 weeks
→ Auto-add high priority: Yes
→ Sprint goal template: "Sprint {number}: {focus-area}"
```

### 3. Reporting & Metrics

**Team Velocity Dashboard:**
```
Reports → Team Velocity
→ Shows:
  - Completed story points per sprint
  - Trend line
  - Projected capacity
  - Burndown chart
```

**Example metrics:**
```
Sprint 15 (Current):
├── Planned: 50 points
├── Completed: 35 points (70%)
├── In Progress: 10 points
├── Remaining: 5 points
└── Projected finish: On time

Team Velocity (Last 6 sprints):
Average: 45 points/sprint
Trend: +5% (improving)
```

**Time Tracking Report:**
```
Reports → Time Tracking
→ Filter: Last 30 days, Team: Dev Team

Total logged: 640 hours
By person:
- John Doe: 160h (5 issues)
- Jane Smith: 155h (8 issues)
- Bob Wilson: 165h (12 issues)
- Alice Brown: 160h (6 issues)

By project:
- PROJ: 400h (62%)
- PROJ2: 240h (38%)
```

**Issue Distribution:**
```
Reports → Issue Distribution

By Type:
- Bugs: 45 (30%)
- Features: 60 (40%)
- Tasks: 45 (30%)

By Priority:
- Highest: 15 (10%)
- High: 45 (30%)
- Medium: 75 (50%)
- Low: 15 (10%)

By Status:
- To Do: 30 (20%)
- In Progress: 45 (30%)
- In Review: 30 (20%)
- Done: 45 (30%)
```

### 4. Priority Management

**Bulk Priority Updates:**
```
Filter: Type = Feature AND Sprint = "Sprint 15"
→ Select all (25 issues)
→ Bulk Update Priority: High
→ Reason: "Customer deadline moved up"
→ Apply
```

**Priority Rules:**
```
Define automatic priority:
- Customer-reported bugs → High
- Security issues → Highest
- Technical debt → Low
- Features with ROI > $10k → High
```

**Priority Matrix:**
```
High Impact + Urgent → Do First (Highest)
High Impact + Not Urgent → Schedule (High)
Low Impact + Urgent → Delegate (Medium)
Low Impact + Not Urgent → Defer (Low)
```

### 5. Sprint Planning

**Sprint Setup:**
```
1. Create Sprint 16
   Name: "Sprint 16: Payment Integration"
   Start: 2026-03-25
   End: 2026-04-08
   Goal: "Complete Stripe integration and fix critical bugs"

2. Add Issues (Auto-assign by velocity)
   → Drag 50 story points to Sprint 16
   → System suggests based on:
     - Team velocity: 45 pts/sprint
     - Buffer: 10% (+5 pts)
     - Carries: 15 pts
   
3. Review and Adjust
   → Check team capacity
   → Account for holidays/PTO
   → Finalize

4. Kickoff
   → Send sprint summary to team
   → Set automated reminders
```

**Mid-Sprint Adjustments:**
```
Sprint 15 → Burndown shows behind
→ Options:
  [ ] Move low-priority items to next sprint
  [ ] Request overtime (not recommended)
  [ ] Extend sprint (with stakeholder approval)
  [ ] Accept reduced scope
```

### 6. Stakeholder Communication

**Export Reports:**
```
Reports → Any Report → Export
→ Format: PDF, Excel, CSV
→ Schedule: Weekly on Monday 9am
→ Recipients: stakeholders@company.com
```

**Automated Updates:**
```
Settings → Notifications → Stakeholder Updates
→ Weekly summary: Fridays 5pm
  - Sprint progress
  - Completed features
  - Blockers/risks
  - Next week plan
```

**Custom Dashboard for Executives:**
```
Dashboard → New → Executive View
→ Widgets:
  - Sprint burndown
  - Feature completion %
  - Bug trend (last 3 months)
  - Team capacity
  - Release readiness
```

## Workflows

### Weekly Routine

**Monday Morning (Sprint Start or Mid-Sprint):**
```
1. Review weekend Jira activity (5 min)
2. Check sprint board health (10 min)
3. Prioritize new issues (15 min)
4. Send team daily standup reminder (2 min)
```

**Wednesday (Mid-Sprint Check):**
```
1. Review burndown chart (5 min)
2. Identify blockers (10 min)
3. Adjust priorities if needed (10 min)
4. Update stakeholders on progress (15 min)
```

**Friday (Week Close):**
```
1. Review completed work (10 min)
2. Generate weekly report (5 min)
3. Plan next week priorities (15 min)
4. Archive completed issues (5 min)
```

### Sprint Planning Meeting (2 hours)

**Preparation (before meeting):**
```
1. Review backlog (30 min)
2. Groom top 20 issues (60 min)
   - Add descriptions
   - Define acceptance criteria
   - Estimate story points
3. Identify dependencies (15 min)
```

**During meeting:**
```
1. Review last sprint (15 min)
   - What went well
   - What to improve
   - Action items

2. Set sprint goal (10 min)
   - Based on objectives
   - Team input

3. Select issues (60 min)
   - Team pulls from backlog
   - Discuss each issue
   - Estimate collaboratively
   
4. Commit to sprint (5 min)
   - Final review
   - Team agreement

5. Setup in WorkHub (10 min)
   - Create sprint
   - Assign issues
   - Set dates
```

### Release Planning

**Create Release:**
```
Jira → Releases → New Release
Name: "v2.5.0 - Payment Integration"
Date: 2026-04-30
Target: Production

Add Features:
- PROJ-500: Stripe integration
- PROJ-501: Apple Pay support
- PROJ-502: Payment history UI

Add Bugs (Critical only):
- PROJ-600: Checkout crash
- PROJ-601: Tax calculation error

Review:
- Feature complete: 80% ✅
- Bug count: 5 remaining ⚠️
- Test coverage: 85% ✅
- Documentation: In progress ⚠️

Action: Hold release until bugs < 3
```

## Best Practices

### 1. Template Governance
✅ Create templates for common scenarios  
✅ Review and update quarterly  
✅ Get team feedback  
✅ Archive unused templates  

### 2. Clear Acceptance Criteria
✅ Every feature has AC  
✅ AC is testable  
✅ AC agreed before sprint  
❌ Don't add AC mid-sprint  

### 3. Realistic Planning
✅ Use team velocity  
✅ Buffer for unknowns (10-15%)  
✅ Account for PTO  
❌ Don't overcommit  

### 4. Regular Communication
✅ Daily updates to team  
✅ Weekly updates to stakeholders  
✅ Transparent about risks  
❌ Don't hide problems  

### 5. Data-Driven Decisions
✅ Use metrics for planning  
✅ Track trends  
✅ Adjust based on actuals  
❌ Don't rely on gut feel alone  

### 6. Continuous Improvement
✅ Retrospectives after sprints  
✅ Track action items  
✅ Implement changes  
✅ Measure impact  

## Advanced Features

### Custom Workflows

**Define workflow states:**
```json
{
  "workflow": "Feature Development",
  "states": [
    { "name": "Backlog", "type": "todo" },
    { "name": "Refined", "type": "todo" },
    { "name": "Ready for Dev", "type": "todo" },
    { "name": "In Progress", "type": "inprogress" },
    { "name": "Code Review", "type": "inprogress" },
    { "name": "QA Testing", "type": "inprogress" },
    { "name": "Done", "type": "done" }
  ],
  "transitions": [
    { "from": "Backlog", "to": "Refined", "requires": ["description", "AC"] },
    { "from": "Refined", "to": "Ready for Dev", "requires": ["estimate"] },
    { "from": "Ready for Dev", "to": "In Progress", "auto": "assignToCurrentUser" }
  ]
}
```

### Automation Rules

**Example rules:**
```
Rule 1: Auto-escalate blocker bugs
- IF: Priority = Highest AND Status = Blocked
- THEN: Notify PM + CTO, Add label "escalated"

Rule 2: Auto-move to Done
- IF: Status = "In Review" AND Resolution = Fixed AND Approvals ≥ 2
- THEN: Transition to Done, Notify QA

Rule 3: Sprint auto-close
- IF: Sprint end date reached AND Sprint not closed
- THEN: Move incomplete to next sprint, Close sprint, Create report
```

### Issue Hierarchies

**Epic → Stories → Tasks:**
```
EPIC: Payment System Revamp (PROJ-1000)
├── STORY: Stripe Integration (PROJ-1001)
│   ├── TASK: API setup (PROJ-1010)
│   ├── TASK: Webhook handling (PROJ-1011)
│   └── TASK: UI integration (PROJ-1012)
├── STORY: Apple Pay Support (PROJ-1002)
│   ├── TASK: Apple Pay SDK (PROJ-1020)
│   └── TASK: Testing (PROJ-1021)
└── STORY: Payment History (PROJ-1003)
    └── TASK: UI component (PROJ-1030)
```

**Track epic progress:**
```
PROJ-1000: Payment System Revamp
Progress: 65% (13/20 story points)
├── In Progress: 35%
├── Done: 65%
└── To Do: 0%

Estimated completion: 2026-04-15
Risk: On track ✅
```

## Troubleshooting

### Team Not Using Templates

**Symptoms:** Issues created manually, missing info

**Solutions:**
1. Make templates easier to access
2. Set templates as required for certain types
3. Training session on template benefits
4. Show time savings data

### Reports Show Low Velocity

**Investigate:**
1. Check time tracking compliance
2. Review sprint scope (too ambitious?)
3. Identify blockers in retrospectives
4. Analyze issue distribution (too many bugs?)

**Actions:**
1. Improve estimation process
2. Reduce scope
3. Remove blockers
4. Balance feature vs bug work

### Issues Not Moving

**Symptoms:** Issues stuck in "In Progress" for weeks

**Check:**
1. Blockers tagged properly?
2. Developer overloaded?
3. Requirements unclear?
4. Dependencies waiting?

**Solutions:**
1. Daily standup to surface blockers
2. Rebalance work
3. Clarify requirements
4. Escalate dependencies

## Next Steps

- [Template Setup Guide](../05-configuration/template-setup.md)
- [Reports Configuration](../05-configuration/reports-setup.md)
- [Automation Examples](../06-examples/automation-scenarios.md)

---

**Last Updated:** 2026-03-22
