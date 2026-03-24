# Tạo Issue Nhanh (Quick Issue Creation)

Feature chính của Jira Module - tạo issue trong 30 giây.

## Cách Hoạt Động

### 1. User Input (Minimal)

User chỉ cần cung cấp 2 thông tin bắt buộc:
- **Title:** Tiêu đề issue
- **Description:** Mô tả ngắn (optional)

### 2. Smart Detection

Hệ thống tự động detect:
- **Issue Type** từ keywords trong title
  - "bug", "lỗi", "error" → Bug
  - "feature", "tính năng", "add" → Feature
  - "hotfix", "urgent" → Hotfix
  - "support", "customer" → Support Ticket

- **Priority** từ keywords
  - "urgent", "critical", "blocker" → Highest
  - "important", "serious" → High
  - Default → Medium

- **Component** từ context
  - Đang ở file `Auth/Login.tsx` → Component: Authentication
  - Đang ở file `Payment/Checkout.ts` → Component: Payment

### 3. Template Application

Áp template phù hợp:
```json
{
  "issueType": "Bug",
  "template": "bug-report-template",
  "fields": {
    "project": "PROJ",
    "summary": "[User input title]",
    "description": "[Template format + user description]",
    "priority": "High",
    "assignee": "[Current user]",
    "labels": ["auto-created", "workhub"],
    "components": ["Authentication"]
  }
}
```

### 4. Create on Jira

- Call Jira REST API
- Return issue key (e.g., PROJ-123)
- Store mapping trong DB

### 5. Notification

User nhận:
- Issue key
- Direct link đến Jira
- Local reference trong WorkHub

## UI Flow

### Option A: Command Palette (Fastest)

```
1. Nhấn Ctrl+J
2. Type: "Login button not working on mobile"
3. Enter
```

→ Issue created với key PROJ-123

### Option B: Quick Form

```
1. Click "Quick Issue" button
2. Fill form:
   - Title: "Login button not working on mobile"
   - Type: Bug (auto-detected)
   - Priority: High (auto-detected)
3. Click "Create"
```

### Option C: From Code (VS Code Extension)

```
1. Select code có bug
2. Right-click → "Create Jira Issue"
3. Title auto-filled từ error
4. Code snippet auto-attached
```

## Auto-Fill Logic

### Project Detection

```typescript
function detectProject(context: Context): string {
  // 1. From current workspace
  if (context.workspace.jiraProject)
    return context.workspace.jiraProject;
  
  // 2. From user's recent projects
  if (context.user.recentProjects.length > 0)
    return context.user.recentProjects[0];
  
  // 3. From team default
  return context.team.defaultProject;
}
```

### Assignee Detection

```typescript
function detectAssignee(issueType: string, component: string): string {
  // 1. Nếu là bug → assign cho reported by
  if (issueType === 'Bug')
    return currentUser.id;
  
  // 2. Component owner
  if (componentOwners[component])
    return componentOwners[component];
  
  // 3. Team lead
  return team.lead.id;
}
```

### Sprint Detection

```typescript
function detectSprint(): string | null {
  // 1. Current active sprint
  const activeSprints = await jiraApi.getActiveSprints(project);
  if (activeSprints.length > 0)
    return activeSprints[0].id;
  
  return null; // Backlog
}
```

## Templates

### Bug Report Template

```markdown
## 🐛 Bug Description
{title}

## 📱 Environment
- **Version:** {autoDetectVersion}
- **Platform:** {autoDetectPlatform}
- **Browser:** {autoDetectBrowser}

## 📋 Steps to Reproduce
1. {userInput}
2. 
3. 

## ✅ Expected Behavior
{userInput}

## ❌ Actual Behavior
{userInput}

## 📸 Screenshots/Logs
{attachments}

## ℹ️ Additional Context
{userInput}

---
*Created via WorkHub Quick Issue*
```

### Feature Request Template

```markdown
## ✨ Feature Description
{title}

## 🎯 User Story
As a {role}, I want {goal} so that {benefit}.

## 📋 Acceptance Criteria
- [ ] {criterion1}
- [ ] {criterion2}
- [ ] {criterion3}

## 💡 Proposed Solution
{userInput}

## 🔗 Related Issues
{autoDetectRelatedIssues}

## 📦 Affected Components
{autoDetectComponents}

---
*Created via WorkHub Quick Issue*
```

## Validation

Trước khi create, system validate:

```typescript
interface ValidationRules {
  title: {
    required: true,
    minLength: 10,
    maxLength: 200
  },
  project: {
    required: true,
    exists: true  // Check project tồn tại
  },
  issueType: {
    required: true,
    validValues: ['Bug', 'Feature', 'Task', 'Story', 'Epic']
  }
}
```

Nếu validation fails → show error, không create.

## Error Handling

| Error | Cause | Solution |
|-------|-------|----------|
| Jira API không available | Network, credentials | Queue request, retry sau |
| Invalid project | Project không tồn tại | Prompt user chọn project khác |
| Missing required field | Custom field mandatory | Show form để fill |
| Rate limit exceeded | Too many requests | Throttle, show warning |

## Performance

- **Average creation time:** 500-800ms
- **95th percentile:** < 1.5s
- **Timeout:** 10s → fallback to manual

## Analytics Tracked

```typescript
interface IssueCreationMetrics {
  timeToCreate: number;        // Milliseconds
  detectionAccuracy: {
    issueType: boolean;
    priority: boolean;
    component: boolean;
  };
  userEditsBeforeSubmit: number;
  templateUsed: string;
}
```

## Next Steps

- [Issue Templates](issue-templates.md) - Tùy chỉnh templates
- [Configuration](../05-configuration/template-setup.md) - Setup
- [Examples](../06-examples/create-bug-report.md) - Ví dụ thực tế

---

**Last Updated:** 2026-03-22
