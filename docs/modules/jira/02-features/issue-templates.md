# Issue Templates - Chuẩn Hóa Và Tăng Tốc

Hệ thống template giúp chuẩn hóa cách tạo issues và giảm 80% thời gian điền thông tin.

## Tổng Quan

**Vấn đề:** Mỗi người tạo issue theo cách riêng, thiếu thông tin, không đồng nhất  
**Giải pháp:** Templates với pre-filled fields, validation, và structured format

## Template Types

### 🐛 Bug Report Template

**Khi nào dùng:** Report lỗi trong hệ thống

**Fields tự động:**
- Issue Type: Bug
- Priority: High (nếu có keywords như "critical", "blocker")
- Component: Auto-detect từ code context
- Labels: ["bug", "needs-investigation"]

**Description format:**
```markdown
## 🐛 Bug Description
[User input title]

## 📱 Environment
- **Version:** [Auto-detected from current release]
- **Platform:** [Auto-detected: Web/Mobile/Desktop]
- **Browser:** [Auto-detected if web]
- **OS:** [Auto-detected from user agent]

## 📋 Steps to Reproduce
1. [User fills]
2. 
3. 

## ✅ Expected Behavior
[User fills]

## ❌ Actual Behavior
[User fills]

## 📸 Screenshots/Logs
[Drag & drop area - auto-uploads to Jira]

## 🔍 Error Messages
[User fills - optional]

## ℹ️ Additional Context
[User fills - optional]

---
*Created via WorkHub on {timestamp}*
*Reporter: {user.name}*
```

### ✨ Feature Request Template

**Khi nào dùng:** Đề xuất tính năng mới

**Fields tự động:**
- Issue Type: Story
- Priority: Medium
- Labels: ["feature-request", "needs-estimation"]

**Description format:**
```markdown
## ✨ Feature Description
[User input title]

## 🎯 User Story
**As a** [role],  
**I want** [capability],  
**So that** [benefit].

## 📋 Acceptance Criteria
- [ ] [Criterion 1]
- [ ] [Criterion 2]
- [ ] [Criterion 3]

## 💡 Proposed Solution
[User describes approach - optional]

## 🎨 UI/UX Mockups
[Optional attachments]

## 🔗 Related Issues
[Auto-detect similar issues]

## 📦 Affected Components
[Auto-detect from context: Authentication, Payment, etc.]

## 🚀 Priority Justification
[User explains why important - optional]

---
*Created via WorkHub on {timestamp}*
```

### 🔥 Hotfix Template

**Khi nào dùng:** Urgent production fixes

**Fields tự động:**
- Issue Type: Bug
- Priority: Highest
- Labels: ["hotfix", "production"]
- Sprint: Current sprint (auto-assigned)

**Description format:**
```markdown
## 🔥 Critical Issue
[User input title]

## 💥 Production Impact
- **Users Affected:** [Number or percentage]
- **Severity:** [Business impact description]
- **Revenue Impact:** [If applicable]

## 📋 Steps to Reproduce in Production
1. 
2. 
3. 

## 🛠️ Immediate Workaround
[If available]

## 🔍 Root Cause Analysis
[To be filled during investigation]

## ✅ Proposed Fix
[Developer fills]

## 🧪 Testing Checklist
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Staging verification
- [ ] Production rollout plan

---
*HOTFIX - Created via WorkHub on {timestamp}*
*Priority: HIGHEST - Requires immediate attention*
```

### 🎫 Support Ticket Template

**Khi nào dùng:** Escalate từ customer support

**Fields tự động:**
- Issue Type: Task
- Priority: Varies (based on customer tier)
- Labels: ["customer-reported", "support"]
- Custom field: Customer Ticket ID (linked)

**Description format:**
```markdown
## 🎫 Customer Issue
[User input title]

## 👤 Customer Information
- **Customer:** {customer.name}
- **Tier:** {customer.tier} (Affects priority)
- **Support Ticket:** #{ticket.id}
- **Reporter:** {support.agent}

## 📧 Customer Description
[Copy-paste from support ticket]

## 🔄 Reproduction Status
- [ ] Successfully reproduced internally
- [ ] Waiting for more information
- [ ] Cannot reproduce

## 📸 Attachments from Customer
[Screenshots, logs from support ticket]

## 🕐 SLA
- **Response Time:** [Based on customer tier]
- **Resolution Time:** [Based on severity]

## 💬 Customer Communication Log
[Track updates sent to customer]

---
*Customer Escalation - Handle with priority*
*Ticket Link: {ticket.url}*
```

### ♻️ Refactor Template

**Khi nào dùng:** Technical debt, code improvements

**Fields tự động:**
- Issue Type: Task
- Priority: Low
- Labels: ["refactor", "tech-debt"]
- Component: Auto-detect

**Description format:**
```markdown
## ♻️ Refactoring Task
[User input title]

## 🎯 Goal
[What needs to be improved]

## 🤔 Current Problems
- [Problem 1: e.g., "Duplicated code in 5 controllers"]
- [Problem 2: e.g., "No unit tests"]
- [Problem 3: e.g., "Complex methods > 100 lines"]

## ✅ Expected Outcome
- [ ] [Outcome 1: e.g., "Extract to shared service"]
- [ ] [Outcome 2: e.g., "Add unit tests coverage > 80%"]
- [ ] [Outcome 3: e.g., "Split into smaller methods"]

## 📁 Affected Files
- `{file1.path}`
- `{file2.path}`

## 🚨 Breaking Changes?
- [ ] Yes - requires migration
- [ ] No - backward compatible

## 📏 Definition of Done
- [ ] Code reviewed
- [ ] Tests added
- [ ] Documentation updated
- [ ] No performance regression

---
*Technical Improvement - Can be done in slack time*
```

### 📝 Technical Debt Template

**Khi nào dùng:** Document known issues to fix later

**Fields tự động:**
- Issue Type: Task
- Priority: Low
- Labels: ["tech-debt", "needs-planning"]

**Description format:**
```markdown
## 📝 Technical Debt Item
[User input title]

## 🏚️ Debt Description
[Explain what's wrong or needs improvement]

## 📊 Impact
- **Performance:** [Performance impact if any]
- **Maintainability:** [How hard to maintain]
- **Security:** [Security concerns if any]
- **Scalability:** [Scalability issues if any]

## 💰 Cost of Delay
[What happens if we don't fix this]

## 🛠️ Proposed Solution
[High-level approach]

## ⏱️ Estimated Effort
[Story points or time estimate]

## 🗓️ Suggested Timeline
[When should this be addressed]

---
*Tracked for future sprint planning*
```

## Template Structure

### Required Fields

| Field | Required | Validation |
|-------|----------|------------|
| Title | ✅ Yes | 10-200 characters |
| Description | ✅ Yes | Follows template format |
| Issue Type | ✅ Yes | Bug/Story/Task/Epic |
| Priority | ⚠️ Auto-set | Can be overridden |
| Project | ✅ Yes | Valid Jira project key |

### Optional Fields

| Field | Default | Notes |
|-------|---------|-------|
| Assignee | Current user | Can be auto-assigned by rules |
| Sprint | Current active | Or backlog |
| Story Points | null | Estimated later |
| Labels | Template-specific | Auto-added from template |
| Components | Auto-detected | Can be overridden |

## Creating Templates

### Step 1: Navigate to Templates

```
Jira Module → Templates → Create New Template
```

### Step 2: Choose Base Type

- Start from existing template (recommended)
- Start from scratch
- Import from Jira

### Step 3: Configure Fields

**Template Builder UI:**
```
┌─────────────────────────────────────┐
│ Template Name: Bug Report           │
│ Template Type: Bug                  │
│ Visibility: Team-wide               │
├─────────────────────────────────────┤
│ Fields Configuration:               │
│                                     │
│ ☑ Title (Required)                 │
│   Validation: Min 10 chars          │
│                                     │
│ ☑ Description (Required)           │
│   Format: Markdown template         │
│   [Edit Template Format...]         │
│                                     │
│ ☑ Priority (Auto-set)              │
│   Default: High                     │
│   Rules: [Configure Rules...]       │
│                                     │
│ ☐ Assignee (Optional)              │
│   Default: Component owner          │
│                                     │
│ ☑ Labels (Auto-add)                │
│   Tags: bug, needs-investigation    │
│                                     │
│ [+ Add Custom Field]                │
└─────────────────────────────────────┘
```

### Step 4: Configure Validation Rules

**Example validation:**
```typescript
{
  "title": {
    "required": true,
    "minLength": 10,
    "maxLength": 200,
    "pattern": "^[A-Z].*" // Must start with capital
  },
  "stepsToReproduce": {
    "required": true,
    "minLength": 20,
    "mustContain": ["1.", "2."] // Must have numbered steps
  },
  "environment": {
    "required": true,
    "autoFill": true // Auto-detect
  }
}
```

### Step 5: Test Template

**Preview mode:**
- Fill sample data
- See how issue will look
- Validate all rules work

### Step 6: Save & Publish

**Options:**
- 👤 **Personal:** Only you can use
- 👥 **Team:** Everyone in team
- 🌍 **Organization:** All projects

## Managing Templates

### Editing Templates

```
Templates → Select template → Edit
```

**Versioning:**
- Changes create new version
- Old issues remain unchanged
- New issues use latest version

### Deleting Templates

**Safety checks:**
- ⚠️ Warning if template has existing issues
- Option to migrate to another template
- Soft delete (can be restored)

### Sharing Templates

**Permissions:**
- **View:** Can see and use template
- **Edit:** Can modify template
- **Admin:** Can delete, change permissions

**Sharing flow:**
```
Template → Share → Add users/teams → Set permissions
```

## Variable Placeholders

### User Variables

| Variable | Output | Example |
|----------|--------|---------|
| `{user.name}` | Current user's name | "John Doe" |
| `{user.email}` | Current user's email | "john@company.com" |
| `{user.id}` | User ID | "johndoe" |

### DateTime Variables

| Variable | Output | Example |
|----------|--------|---------|
| `{date}` | Current date | "2026-03-22" |
| `{time}` | Current time | "14:30:00" |
| `{timestamp}` | ISO timestamp | "2026-03-22T14:30:00Z" |

### System Variables

| Variable | Output | Example |
|----------|--------|---------|
| `{version}` | App version | "1.5.2" |
| `{environment}` | Environment | "Production" |
| `{platform}` | Platform | "Web/Mobile/Desktop" |

### Auto-Detect Variables

| Variable | Output | Example |
|----------|--------|---------|
| `{autoDetect.browser}` | Browser info | "Chrome 120" |
| `{autoDetect.os}` | Operating system | "Windows 11" |
| `{autoDetect.component}` | Component from code | "Authentication" |
| `{autoDetect.priority}` | Priority from keywords | "High" |

### Custom Variables

**Define your own:**
```json
{
  "variables": {
    "team": "Backend Team",
    "slackChannel": "#backend-alerts",
    "oncallEngineer": "@johndoe"
  }
}
```

**Use in template:**
```markdown
## Team
Owned by: {team}
Slack: {slackChannel}
On-call: {oncallEngineer}
```

## Example Templates

### Complete Bug Template (Production-Ready)

```markdown
# Bug Report: {title}

## 🐛 Summary
{description}

## 📱 Environment
- **Version:** {version}
- **Platform:** {autoDetect.platform}
- **Browser:** {autoDetect.browser}
- **OS:** {autoDetect.os}
- **User Agent:** {autoDetect.userAgent}

## 📋 Steps to Reproduce
1. [Step 1]
2. [Step 2]
3. [Step 3]

## ✅ Expected Result
[What should happen]

## ❌ Actual Result
[What actually happens]

## 📸 Evidence
### Screenshots
[Drag & drop screenshots here]

### Error Logs
```
[Paste error logs if available]
```

### Console Errors
```
[Browser console errors if applicable]
```

## 🔍 Additional Information
- **Frequency:** [Always/Sometimes/Rare]
- **User Impact:** [Number of users affected]
- **Workaround:** [If any available]

## 📊 Related Information
- **Related Issues:** [Auto-linked similar issues]
- **Component:** {autoDetect.component}
- **Assignee:** {componentOwner}

---
**Reported by:** {user.name} ({user.email})  
**Reported at:** {timestamp}  
**Via:** WorkHub Jira Module  
**Severity:** {autoDetect.severity}
```

### Complete Feature Template (Production-Ready)

```markdown
# Feature Request: {title}

## ✨ Overview
{description}

## 🎯 User Story
**As a** [user role],  
**I want** [capability],  
**So that** [business value].

## 💡 Business Value
- **Problem it solves:** [Pain point]
- **Expected benefit:** [Metrics if possible]
- **Target users:** [Who benefits]
- **Priority justification:** [Why now]

## 📋 Acceptance Criteria
**Given** [context]  
**When** [action]  
**Then** [expected outcome]

- [ ] AC1: [Specific, testable criterion]
- [ ] AC2: [Specific, testable criterion]
- [ ] AC3: [Specific, testable criterion]

## 🎨 Design & UX
### Mockups
[Attach design files or link to Figma]

### User Flow
1. User navigates to [location]
2. User clicks [action]
3. System displays [result]

## 🛠️ Technical Considerations
- **Affected modules:** {autoDetect.components}
- **API changes:** [Yes/No - describe if yes]
- **Database changes:** [Yes/No - describe if yes]
- **Breaking changes:** [Yes/No - migration needed?]

## 📦 Dependencies
- [ ] Depends on PROJ-XXX
- [ ] Requires infrastructure change
- [ ] Needs third-party integration

## 🧪 Testing Strategy
- [ ] Unit tests
- [ ] Integration tests
- [ ] E2E tests
- [ ] Performance tests
- [ ] Security review

## 📏 Definition of Done
- [ ] Feature implemented per AC
- [ ] Tests written and passing
- [ ] Code reviewed
- [ ] Documentation updated
- [ ] QA approved
- [ ] Stakeholder demo completed

## 🚀 Rollout Plan
- **Phase 1:** [e.g., Beta users]
- **Phase 2:** [e.g., 50% rollout]
- **Phase 3:** [e.g., Full release]

---
**Requested by:** {user.name}  
**Priority:** {autoDetect.priority}  
**Team:** {team}  
**Created:** {timestamp}
```

## Best Practices

### 1. Keep Templates Concise
❌ **Bad:** 20+ fields, overwhelming  
✅ **Good:** Essential fields only, rest optional

### 2. Use Smart Defaults
❌ **Bad:** Empty fields users must fill  
✅ **Good:** Pre-filled with auto-detected values

### 3. Provide Examples
❌ **Bad:** "Enter description"  
✅ **Good:** "Example: User cannot login on mobile Safari"

### 4. Validate Early
❌ **Bad:** Validation errors after submit  
✅ **Good:** Real-time validation as user types

### 5. Version Control
✅ Keep history of template changes  
✅ Document why changes were made  
✅ Allow rollback if needed

### 6. Team Standards
✅ Agree on naming conventions  
✅ Regular template reviews  
✅ Archive unused templates

## Troubleshooting

### Template Not Showing

**Cause:** Permission issue or archived  
**Solution:**
1. Check template visibility (personal vs team)
2. Verify template is active
3. Check your role permissions

### Auto-Fill Not Working

**Cause:** Missing context or configuration  
**Solution:**
1. Verify auto-detect settings enabled
2. Check required data is available
3. Review variable mappings

### Validation Failing

**Cause:** Strict rules or missing fields  
**Solution:**
1. Read validation error messages
2. Fill all required fields
3. Check field format (e.g., date format)

## Next Steps

- [Quick Issue Creation](quick-issue-creation.md) - Use templates effectively
- [Template Setup Guide](../05-configuration/template-setup.md) - Configure templates
- [Examples](../06-examples/) - Real-world template usage

---

**Last Updated:** 2026-03-22
