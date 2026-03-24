# Template Setup Guide

Create and manage issue templates for your team.

## Create Your First Template

### Step 1: Navigate to Templates

```
WorkHub → Jira Module → Templates → Create New
```

### Step 2: Choose Template Base

**Options:**
- Start from scratch
- Use built-in template (Bug Report, Feature Request, etc.)
- Import from existing Jira issue

**Recommended:** Start with built-in template and customize

### Step 3: Basic Information

Fill template details:
```
Name: Bug Report - Mobile
Type: Bug
Description: Template for mobile-specific bugs
Visibility: Team (visible to all team members)
```

### Step 4: Configure Fields

#### Required Fields

```
Title: [User enters]
Description: [Template with placeholders]
Issue Type: Bug (fixed)
Project: PROJ (fixed)
```

#### Default Values

```
Priority: High
Labels: ["bug", "mobile", "needs-investigation"]
Components: ["Mobile App"]
Assignee: Component owner (auto)
```

#### Description Template

Use markdown with placeholders:
```markdown
## 🐛 Bug Description
{description}

## 📱 Device Information
- Device: {device}
- OS Version: {os_version}
- App Version: {app_version}

## 📋 Steps to Reproduce
1. Open app
2. Navigate to [screen]
3. [Action that causes bug]

## ✅ Expected Behavior
[What should happen]

## ❌ Actual Behavior
[What actually happens]

## 📸 Screenshots
[Drag & drop screenshots here]

## 🔍 Additional Info
- Frequency: [Always/Sometimes/Rare]
- User Impact: [High/Medium/Low]
- First seen: {date}
```

### Step 5: Validation Rules

Add validation to ensure quality:
```
Title:
- Min length: 10 characters
- Max length: 200 characters
- Required: Yes

Steps to Reproduce:
- Min length: 20 characters
- Must contain: "1.", "2." (numbered steps)
- Required: Yes

Device Info:
- Auto-filled from user agent
- Required: Yes
```

### Step 6: Test Template

1. Click "Preview" button
2. Fill sample data
3. See how issue will look
4. Verify all placeholders work
5. Check validation rules

### Step 7: Save Template

```
[Save as Personal] - Only you can use
[Save for Team] - Everyone in team can use
[Save for Organization] - All projects can use
```

## Managing Templates

### View Templates

```
Jira → Templates
```

Shows:
- All available templates
- Usage statistics
- Last modified date
- Creator

### Edit Template

```
Templates → Select template → Edit
→ Make changes
→ Save

Note: Creates new version, old issues unaffected
```

### Archive Template

```
Templates → Select template → Archive
→ Template hidden from list
→ Can be restored later
→ Existing issues unchanged
```

### Template Permissions

```
Personal Templates:
- Only you can view/edit
- Not shared with team

Team Templates:
- Everyone can view/use
- Only creator + admins can edit

Organization Templates:
- All teams can use
- Only admins can edit
```

## Advanced Features

### Variable Placeholders

**System Variables:**
```
{user.name} → John Doe
{user.email} → john@company.com
{date} → 2026-03-22
{time} → 14:30:00
{version} → 2.5.1 (current app version)
```

**Auto-Detect Variables:**
```
{autoDetect.browser} → Chrome 120
{autoDetect.os} → macOS 14.2
{autoDetect.component} → Authentication (from code context)
{autoDetect.priority} → High (from keywords)
```

**Custom Variables:**
```json
{
  \"variables\": {
    \"teamSlack\": \"#mobile-team\",
    \"oncall\": \"@john-doe\",
    \"releaseVersion\": \"2.5.0\"
  }
}
```

### Conditional Fields

Show/hide fields based on conditions:
```json
{
  \"conditions\": [
    {
      \"if\": { \"type\": \"Bug\" },
      \"then\": { \"show\": [\"severity\", \"stepsToReproduce\"] }
    },
    {
      \"if\": { \"priority\": \"Highest\" },
      \"then\": { \"required\": [\"businessImpact\"] }
    }
  ]
}
```

### Auto-Assignment Rules

Assign based on conditions:
```json
{
  \"autoAssignment\": {
    \"rules\": [
      {
        \"if\": { \"component\": \"Mobile App\" },
        \"assign\": \"john.doe@company.com\"
      },
      {
        \"if\": { \"labels\": [\"security\"] },
        \"assign\": \"security-team@company.com\"
      }
    ],
    \"default\": \"unassigned\"
  }
}
```

## Best Practices

### 1. Keep Templates Simple
✅ Essential fields only
✅ Clear placeholders
❌ Don't make templates too complex

### 2. Use Clear Names
✅ "Bug Report - Mobile"
✅ "Feature Request - API"
❌ "Template 1", "New Template"

### 3. Provide Examples
✅ Show example in placeholder
✅ \"Example: User cannot login on iOS 16\"
❌ Just \"Enter description\"

### 4. Regular Reviews
✅ Review templates quarterly
✅ Archive unused templates
✅ Update based on team feedback

### 5. Version Control
✅ Document template changes
✅ Keep change history
✅ Communicate updates to team

## Example Templates

### Complete Bug Template

Already shown above - production-ready bug report template

### Feature Request Template

```markdown
# Feature Request: {title}

## 💡 Problem Statement
What problem does this solve?

## 🎯 Proposed Solution
How should we solve it?

## 📊 Business Value
- ROI estimate:
- User impact:
- Priority justification:

## 🎨 Design
[Link to mockups/wireframes]

## ✅ Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## 🚀 Implementation Notes
[Technical considerations]

---
Created: {date}
Requested by: {user.name}
```

### Hotfix Template

```markdown
# 🔥 HOTFIX: {title}

## Production Impact
- Severity: CRITICAL
- Users affected: [number/%]
- Revenue impact: [if applicable]
- Started: {time}

## Root Cause
[Initial analysis]

## Immediate Fix
[Quick solution]

## Testing Checklist
- [ ] Unit tests pass
- [ ] Smoke tests pass
- [ ] Staging verification
- [ ] Rollback plan ready

## Deployment Plan
1. Deploy to staging
2. Verify fix
3. Deploy to production
4. Monitor for 1 hour

---
⚠️ URGENT - Handle immediately
Escalate if blocked: @oncall-engineer
```

## Troubleshooting

### Template Not Saving

**Check:**
- All required fields filled
- Template name unique
- Validation rules valid
- Permissions sufficient

### Variables Not Working

**Verify:**
- Correct syntax: `{variable}` not `${variable}`
- Variable exists in system
- User has permission to access variable

### Auto-Assignment Not Working

**Debug:**
1. Check rules syntax
2. Verify user exists
3. Check component/label matches
4. Review logs for errors

---

**Last Updated:** 2026-03-22
