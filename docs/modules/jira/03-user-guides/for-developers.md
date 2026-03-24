# Hướng Dẫn Cho Developers

Sử dụng Jira Module hiệu quả dalam workflow phát triển hàng ngày.

## Quick Start (3 Phút)

### Bước 1: Kết Nối Jira (30 giây)

```
1. Mở WorkHub
2. Settings → Integrations → Jira
3. Click "Connect Account"
4. Enter:
   - Your email
   - API token (from Atlassian)
5. Test Connection → ✅ Success
```

### Bước 2: First Issue (1 phút)

```
1. Press Ctrl+J (anywhere in app)
2. Type: "Fix login bug on mobile"
3. Press Enter
4. Done! Issue PROJ-123 created
```

### Bước 3: Auto-Log Work (1 phút)

```bash
# Commit with issue key
git commit -m "fix(auth): resolve login redirect #PROJ-123"
# → Automatically logs 30 minutes to PROJ-123
```

**Xong! Bạn đã sẵn sàng.** 🎉

## Daily Workflows

### Create Issue From Code

**Scenario:** Phát hiện bug khi đang code

**Method 1: Quick Command (Fastest)**
```
1. Press Ctrl+J
2. Type title: "Null pointer exception in UserService"
3. Press Enter
```

**Method 2: From Code Selection (VS Code)**
```
1. Select problematic code
2. Right-click → "Create Jira Issue"
3. Title auto-filled from error
4. Code snippet auto-attached
5. Click Create
```

**Method 3: From Exception Log**
```
1. Copy exception stacktrace
2. Ctrl+J → Paste
3. System extracts:
   - Error message → Title
   - Stacktrace → Description
   - File/line → Component detection
4. Issue created with full context
```

### Link Code to Issues

**In commit messages:**
```bash
# Single issue
git commit -m "feat(auth): implement OAuth2 login #PROJ-456"

# Multiple issues
git commit -m "fix: resolve bugs #PROJ-100 #PROJ-101"

# With type and scope
git commit -m "refactor(payment): simplify checkout flow #PROJ-789"
```

**Auto-linking benefits:**
- ✅ Work time logged automatically
- ✅ Commit linked to issue in Jira
- ✅ Build pipeline can reference issues
- ✅ Code review shows related issues

### Branch Naming

**Convention:**
```bash
# Create branch with issue key
git checkout -b feature/PROJ-123-oauth-login

# WorkHub detects and links automatically
# When you push commits on this branch → auto-linked to PROJ-123
```

**Benefits:**
- Auto-link all commits on branch
- PR title auto-filled
- Easy to track which branch for which issue

### Work Sessions

**Start work on issue:**
```
1. Open issue PROJ-123
2. Click "Start Work"
   → Timer starts
   → Status changes to "In Progress" in Jira
   → Your availability updated in Slack/Teams
```

**While working:**
- Timer runs in background
- Auto-pause when idle 10+ minutes
- Resume automatically when you return

**Stop work:**
```
1. Click "Stop Work"
2. Confirms: "Log 2h 15m to PROJ-123?"
3. Click Yes → Time logged to Jira
```

**Manual pause:**
```
Pause button → Select reason:
- Lunch break
- Meeting
- Context switch to other task
- Other

Resume when ready
```

## IDE Integration

### VS Code Extension

**Installation:**
```
1. VS Code Extensions
2. Search "WorkHub Jira"
3. Install
4. Reload VS Code
```

**Features:**

**1. Quick Create from Code**
```
Select code → Right-click → "Create Jira Issue"
→ Code attached as snippet
```

**2. Inline Issue References**
```typescript
// TODO: PROJ-123 - Implement retry logic
// BUG: PROJ-456 - Fix null pointer

// Hover over PROJ-123 → Shows issue details
// Click → Opens in Jira
```

**3. Status Bar**
```
Shows currently active issue:
[⏱️ PROJ-123: 0:45:32] [⏸️ Pause] [⏹️ Stop]
```

**4. Issue Browser**
```
Sidebar → WorkHub icon
→ Shows your assigned issues
→ Filter by status
→ Start work directly
```

**5. Git Integration**
```
Commit panel → Detects issue from branch
→ Auto-adds #PROJ-123 to commit message
```

### IntelliJ/WebStorm Plugin

**Similar features:**
- Quick issue creation
- Inline issue references
- Work time tracking
- Git integration

**Download:** JetBrains Marketplace → "WorkHub"

## Keyboard Shortcuts

**Global (Works Everywhere):**
| Shortcut | Action |
|----------|--------|
| `Ctrl+J` | Quick create issue |
| `Ctrl+Shift+J` | Search issues |
| `Ctrl+Alt+J` | Show my issues |
| `Ctrl+K, Ctrl+J` | Open Jira in browser |

**Issue View:**
| Shortcut | Action |
|----------|--------|
| `Ctrl+Enter` | Save changes |
| `Esc` | Close without saving |
| `Ctrl+L` | Log work |
| `Ctrl+M` | Add comment |
| `Ctrl+S` | Start work |

**Bulk Operations:**
| Shortcut | Action |
|----------|--------|
| `Ctrl+A` | Select all filtered |
| `Ctrl+Shift+A` | Assign selected |
| `Delete` | Delete selected (with confirmation) |

**Navigation:**
| Shortcut | Action |
|----------|--------|
| `Ctrl+↑/↓` | Next/prev issue |
| `Ctrl+1-9` | Jump to tab |

**Custom shortcuts:**
```
Settings → Keyboard Shortcuts → Customize
```

## Git Integration Deep Dive

### Commit Message Format

**Best practice:**
```bash
<type>(<scope>): <subject> #ISSUE-KEY

# Examples:
git commit -m "feat(auth): add OAuth2 login #PROJ-123"
git commit -m "fix(api): resolve timeout error #PROJ-456"
git commit -m "refactor(ui): simplify dashboard #PROJ-789"
git commit -m "test(payment): add checkout tests #PROJ-100"
```

**Types:**
- `feat`: New feature → Logs ~45 minutes
- `fix`: Bug fix → Logs ~30 minutes
- `refactor`: Code refactoring → Logs ~36 minutes
- `test`: Adding tests → Logs ~24 minutes
- `docs`: Documentation → Logs ~15 minutes
- `chore`: Maintenance → Logs ~20 minutes

**Scope examples:**
- `auth`: Authentication module
- `payment`: Payment processing
- `ui`: User interface
- `api`: Backend API
- `db`: Database

### Advanced Git Features

**Multiple issues in one commit:**
```bash
git commit -m "fix: resolve related bugs #PROJ-100 #PROJ-101 #PROJ-102"
# → Logs 10 minutes to each issue (30 min total split 3 ways)
```

**Skip auto-logging:**
```bash
git commit -m "chore: update dependencies [skip-worklog]"
# → Won't log time to Jira
```

**Custom time:**
```bash
git commit -m "feat(payment): add Stripe integration #PROJ-456 [time:2h]"
# → Logs exactly 2 hours instead of auto-estimate
```

### Git Hooks Integration

**pre-commit hook:**
```bash
#!/bin/bash
# .git/hooks/pre-commit

# Ensure branch has issue key
BRANCH=$(git symbolic-ref --short HEAD)
if [[ ! $BRANCH =~ PROJ-[0-9]+ ]]; then
  echo "❌ Branch name must include issue key (e.g., feature/PROJ-123-description)"
  exit 1
fi

# Ensure commit message references issue
COMMIT_MSG=$(cat .git/COMMIT_EDITMSG)
if [[ ! $COMMIT_MSG =~ #PROJ-[0-9]+ ]]; then
  # Auto-add issue key from branch
  ISSUE_KEY=$(echo $BRANCH | grep -oP 'PROJ-\d+')
  echo " #$ISSUE_KEY" >> .git/COMMIT_EDITMSG
fi
```

**commit-msg hook:**
```bash
#!/bin/bash
# .git/hooks/commit-msg

# Validate conventional commit format
COMMIT_MSG=$(cat $1)
if [[ ! $COMMIT_MSG =~ ^(feat|fix|docs|style|refactor|test|chore)(\(.+\))?: .+ #PROJ-[0-9]+$ ]]; then
  echo "❌ Commit message must follow format:"
  echo "   <type>(<scope>): <subject> #PROJ-XXX"
  exit 1
fi
```

## Templates & Automation

### Personal Templates

**Create your own templates:**
```
1. Jira → Templates → New Template
2. Name: "My Standard Bug"
3. Configure:
   - Type: Bug
   - Priority: High (default)
   - Assignee: Me
   - Component: Detection from code
   - Labels: ["needs-investigation"]
4. Description template:
   ## Bug
   {description}
   
   ## Environment
   - Version: {version}
   - Found in: {component}
   
   ## Code Reference
   {codeSnippet}
5. Save as Personal Template
```

**Use template:**
```
Ctrl+J → Type: /bug [title]
→ Uses your "My Standard Bug" template
```

### Quick Actions

**Define custom actions:**
```json
{
  "quickActions": {
    "assignToMe": {
      "name": "Assign to Me + Start Work",
      "steps": [
        { "action": "assign", "to": "currentUser" },
        { "action": "transition", "to": "In Progress" },
        { "action": "startTimer" }
      ],
      "shortcut": "Ctrl+Shift+M"
    },
    "readyForReview": {
      "name": "Ready for Code Review",
      "steps": [
        { "action": "stopTimer" },
        { "action": "transition", "to": "In Review" },
        { "action": "addComment", "text": "Ready for review @team-leads" },
        { "action": "createPR", "linkToIssue": true }
      ],
      "shortcut": "Ctrl+Shift+R"
    }
  }
}
```

## Best Practices

### 1. Commit Early, Commit Often
✅ **Good:** Small commits with clear messages
```bash
git commit -m "feat(auth): add User entity #PROJ-123"
git commit -m "feat(auth): add UserRepository #PROJ-123"  
git commit -m "feat(auth): implement login logic #PROJ-123"
```

❌ **Bad:** One huge commit at end of day
```bash
git commit -m "WIP #PROJ-123"  # Lost granularity
```

### 2. Use Templates
✅ Create templates for common issue types  
✅ Reduces thinking overhead  
✅ Ensures consistency

### 3. Reference Issues Everywhere
✅ Commit messages: `#PROJ-123`  
✅ PR titles: `[PROJ-123] Add OAuth login`  
✅ Code comments: `// TODO: PROJ-123 - refactor this`

### 4. Start Work Sessions
✅ Start timer when beginning work  
✅ Don't forget to stop when done  
✅ Review logged time weekly

### 5. Keep Issues Updated
✅ Add comments as you progress  
✅ Update status regularly  
✅ Link PRs and commits

### 6. Clean Up
✅ Close completed issues  
✅ Remove stale branches  
✅ Archive old sprints

## Common Workflows

### Bug Fix Workflow

```
1. QA reports bug in Jira
   → PROJ-456: "Login fails on mobile"

2. You're assigned
   → Notification in WorkHub

3. Start work
   → Click "Start Work"
   → Timer begins

4. Create branch
   → git checkout -b bugfix/PROJ-456-login-mobile

5. Investigate & fix
   → Debug code
   → Write fix
   → Add tests

6. Commit
   → git commit -m "fix(auth): resolve mobile login redirect #PROJ-456"
   → Automatically logs investigation time

7. Create PR
   → PR auto-linked to PROJ-456
   → Title: "[PROJ-456] Fix login on mobile"

8. Mark ready for review
   → Stop timer
   → Transition to "In Review"
   → Tag reviewers

9. After approval
   → Merge PR
   → Transition to "Done"
   → Auto-notify QA for verification
```

### Feature Development Workflow

```
1. PM creates feature in backlog
   → PROJ-789: "Add dark mode"

2. Sprint planning
   → Issue assigned to you
   → Estimated: 8 story points

3. Start work
   → Create branch: feature/PROJ-789-dark-mode
   → Start timer

4. Break down into tasks
   → Create sub-tasks in Jira:
     - PROJ-790: UI component dark styles
     - PROJ-791: Add theme switcher
     - PROJ-792: Persist user preference

5. Implement incrementally
   → Commit to each sub-task
   → Regular pushes to GitHub

6. Testing
   → Manual testing
   → Automated tests
   → Update PROJ-789 with test results

7. Demo to PM
   → Schedule demo
   → Log demo time to PROJ-789
   → Get approval

8. Deploy
   → Merge to main
   → Deploy to production
   → Mark feature as "Done"
```

## Troubleshooting

### Issue Not Auto-Created

**Symptoms:** Press Ctrl+J, type title, nothing happens

**Solutions:**
1. Check Jira connection: Settings → Jira → Test
2. Check permissions: Can you create issues in Jira?
3. Check default project configured
4. Check browser console for errors

### Work Time Not Logged

**Symptoms:** Git commit made but no work logged

**Check:**
1. Commit message has `#PROJ-123` format
2. Auto-logging enabled: Settings → Jira → Auto-log work
3. Issue exists in Jira
4. You have permission to log work
5. Check sync status

**Manual fix:**
```
Jira → Issue PROJ-123 → Log Work
→ Enter time manually
```

### IDE Extension Not Working

**VS Code:**
1. Check extension installed and enabled
2. Reload VS Code
3. Check WorkHub connection in extension settings
4. View extension output: Output → WorkHub Jira

**IntelliJ:**
1. File → Settings → Plugins → WorkHub (enabled?)
2. Invalidate Caches and Restart
3. Check plugin log: Help → Show Log

### Shortcuts Not Working

**Check:**
1. Extension/plugin installed
2. No conflicting shortcuts
3. Customize if needed: Settings → Keyboard Shortcuts

## Tips & Tricks

### 1. Smart Issue Creation

**From error logs:**
```
# Copy this error:
NullPointerException at UserService.java:45

# Paste into Ctrl+J
→ Auto-detects:
  - Type: Bug
  - Component: UserService
  - Priority: High
  - Description includes stacktrace
```

### 2. Quick Filters

**Save common searches:**
```
My Bugs: assignee = currentUser() AND type = Bug
This Sprint: sprint = currentSprint() AND status != Done
Needs Review: status = "In Review" AND assignee = currentUser()
```

**Access quickly:**
```
Ctrl+Shift+J → Select saved filter → Enter
```

### 3. Issue Templates

**Use slash commands:**
```
/bug [title]    → Bug template
/feature [title] → Feature template
/task [title]   → Task template
```

### 4. Bulk Operations

**Select multiple commits:**
```
# Multiple commits, same issue
git commit -m "feat(auth): part 1 #PROJ-123"
git commit -m "feat(auth): part 2 #PROJ-123"
git commit -m "feat(auth): part 3 #PROJ-123"

# All logged to PROJ-123
```

### 5. Code Reviews

**Link PR to issue:**
```
PR title: [PROJ-123] Implement OAuth login
PR description:
  Implements #PROJ-123
  
  Changes:
  - Added OAuth2 flow
  - Updated User model
  - Added tests
  
  Testing:
  - Manual: ✅
  - Automated: ✅
```

## Next Steps

- [Templates Setup](../05-configuration/template-setup.md) - Create custom templates
- [Git Hooks](../06-examples/git-hooks-setup.md) - Automate even more
- [Advanced Filters](../06-examples/advanced-filters.md) - Power user features

---

**Last Updated:** 2026-03-22
