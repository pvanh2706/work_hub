# Example: Create Bug Report

Complete walkthrough of reporting a bug using WorkHub Jira Module.

## Scenario

**You discovered:** "Login button unresponsive on mobile Safari"  
**Device:** iPhone 13, iOS 16.5  
**Browser:** Safari  
**Severity:** High (blocks login for all iOS users)

## Step-by-Step Walkthrough

### Step 1: Quick Create (30 seconds)

**Press Ctrl+J:**
```
Type: Login button not working on mobile Safari iPhone
Press Enter
```

**System auto-detects:**
- Type: Bug (keyword "not working")
- Priority: High (keyword "button not working")
-Component: Authentication (keyword "login")

### Step 2: Review Template (1 minute)

WorkHub opens pre-filled bug report:

```markdown
## 🐛 Bug: Login button not working on mobile Safari iPhone

## 📱 Environment
- Version: 2.5.1 (auto-detected)
- Platform: Mobile (auto-detected)
- Browser: Safari (you need to specify version)
- OS: iOS (you need to specify version)

## 📋 Steps to Reproduce
1. [You fill this]
2. 
3. 

## ✅ Expected Behavior
[You fill this]

## ❌ Actual Behavior
[You fill this]

## 📸 Screenshots
[Drag & drop area]
```

### Step 3: Fill Details (2 minutes)

**Complete the template:**

```markdown
## 📱 Environment
- Version: 2.5.1
- Platform: Mobile - iPhone 13
- Browser: Safari 16.5
- OS: iOS 16.5

## 📋 Steps to Reproduce
1. Open WorkHub app on iPhone 13 (iOS 16.5)
2. Navigate to login page (https://workhub.company.com/login)
3. Enter valid credentials:
   - Email: test@company.com
   - Password: ••••••••
4. Tap "Login" button

## ✅ Expected Behavior
- Button responds to tap
- Shows loading indicator
- Redirects to dashboard on successful login

## ❌ Actual Behavior
- Button appears frozen
- No visual feedback when tapped
- No loading indicator
- Login doesn't happen
- No error message shown

## 📸 Screenshots
[Attaching screenshots - see below]
```

### Step 4: Attach Evidence (1 minute)

**Take screenshots:**
1. Before tap (showing login form)
2. After tap attempt (no change visible)
3. Browser console (if available)

**Drag & drop:**
```
📎 login-form-before.png (uploaded ✅)
📎 login-button-frozen.png (uploaded ✅)
📎 console-errors.png (uploaded ✅)
```

**Console shows:**
```javascript
Uncaught TypeError: Cannot read property 'addEventListener' of null
  at LoginButton.js:45
  at handleSubmit (auth.js:123)
```

### Step 5: Add Context (30 seconds)

**Additional Info section:**
```markdown
## ℹ️ Additional Context
- Bug does NOT occur on:
  - Desktop Chrome (Windows 11)
  - Desktop Safari (macOS)
  - Mobile Chrome (Android)
  - Mobile Chrome (iOS)
- Bug ONLY occurs on: Mobile Safari (iOS)
- Issue appeared after v2.5.1 deployment (worked in v2.5.0)
- Affects ALL iOS Safari users (confirmed with 3 different iPhones)
- Workaround: Use Chrome on iOS (works)

## 🚨 Business Impact
- Severity: HIGH
- Estimated users affected: ~200 iOS Safari users/day
- Customer complaints: 5 tickets today
- Revenue impact: $2,000/day (blocked purchases)
```

### Step 6: Create Issue (Click button)

**WorkHub creates:**
```
✅ Issue created: PROJ-456
🔗 https://your-domain.atlassian.net/browse/PROJ-456
📧 Assignee notified: John Doe (Mobile team lead)
```

## Complete Bug Report

**Final issue in Jira:**

```markdown
# 🐛 Login button not working on mobile Safari iPhone

**Type:** Bug  
**Priority:** High  
**Status:** To Do  
**Component:** Authentication  
**Affects Version:** 2.5.1  
**Assignee:** John Doe  
**Labels:** bug, mobile, safari, authentication, high-priority

---

## 📱 Environment
- **Version:** 2.5.1
- **Platform:** Mobile - iPhone 13
- **Browser:** Safari 16.5
- **OS:** iOS 16.5
- **Device Details:** iPhone 13, 256GB, iOS 16.5.1

## 📋 Steps to Reproduce
1. Open WorkHub app on iPhone 13 (iOS 16.5)
2. Navigate to login page (https://workhub.company.com/login)
3. Enter valid credentials:
   - Email: test@company.com
   - Password: ••••••••
4. Tap "Login" button

## ✅ Expected Behavior
- Button responds to tap with visual feedback
- Shows loading indicator
- Initiates authentication request
- Redirects to dashboard on successful login

## ❌ Actual Behavior
- Button appears completely unresponsive
- No visual feedback when tapped
- No loading indicator appears
- Login request not initiated
- User stuck on login page
- Browser console shows JavaScript error

## 📸 Evidence

**Screenshots:**
1. Login form (before tap attempt)
   ![login-form-before](https://attachments.jira.com/...)
   
2. After tap (no visible change)
   ![login-button-frozen](https://attachments.jira.com/...)
   
3. Console errors
   ![console-errors](https://attachments.jira.com/...)

**Console Error:**
```javascript
Uncaught TypeError: Cannot read property 'addEventListener' of null
  at LoginButton.js:45
  at handleSubmit (auth.js:123)

Stack trace:
  at LoginButton (LoginButton.js:45:12)
  at handleSubmit (auth.js:123:5)
  at onclick (login.html:89:3)
```

## 🔍 Additional Context

**Works on:**
✅ Desktop Chrome (Windows 11)  
✅ Desktop Safari (macOS 14.2)  
✅ Mobile Chrome (Android 13)  
✅ Mobile Chrome (iOS 16.5)  

**Does NOT work on:**
❌ Mobile Safari (iOS 16.5) - **ONLY affected browser**

**History:**
- Worked perfectly in v2.5.0
- Broke after v2.5.1 deployment (deployed 2026-03-21)
- Confirmed on 3 different iPhones (13, 14, 14 Pro)

**Workaround:**
Users can login using Chrome browser on iOS

## 🚨 Business Impact

**Severity:** HIGH - Complete feature failure for segment  
**Users Affected:** ~200 iOS Safari users/day (15% of mobile users)  
**Customer Complaints:** 5 support tickets today, trending up  
**Revenue Impact:** Estimated $2,000/day in blocked transactions  
**SLA:** Resolution needed within 24 hours (Enterprise customer complaints)

## 💡 Suggested Investigation

1. Review changes to LoginButton.js in v2.5.1
2. Check event listener attachment in auth.js
3. Test iOS Safari specific behaviors
4. Verify polyfills for iOS Safari compatibility

---

**Reported by:** Jane Smith (QA Team)  
**Reported at:** 2026-03-22 10:30 AM  
**Via:** WorkHub Quick Issue Creation  
**Jira Issue:** PROJ-456  
**Linked Tickets:** #SUPPORT-789, #SUPPORT-790  
```

## What Happens Next

### Hour 1: Issue Received

```
10:30 AM - Issue created
↓
10:31 AM - John Doe (mobile lead) notified
↓
10:45 AM - John moved to "In Progress"
↓
10:50 AM - John commented:
"Confirmed. Investigating. Likely event listener issue in v2.5.1 refactor."
```

### Hour 2-4: Investigation

```
11:00 AM - Root cause identified:
"Event listener attached before DOM ready on Safari mobile"
↓
12:00 PM - Fix implemented
↓
01:00 PM - PR created: #PRpj-456-fix-safari-login
↓
02:00 PM - Code review approved
```

### Hour 5: Fix & Deploy

```
02:30 PM - Merged to main
↓
02:45 PM - Deployed to staging
↓
03:00 PM - QA verified on staging ✅
↓
03:30 PM - Deployed to production
↓
03:45 PM - Verified on production ✅
```

### Result

```
Status: ✅ Resolved
Resolution: Fixed in v2.5.2
Time to fix: 5 hours (within 24h SLA ✅)
Customer impact: Minimal (fixed same day)
```

## Key Takeaways

### What Made This Bug Report Effective

✅ **Clear title** - Describes problem and affected platform  
✅ **Detailed reproduction steps** - Anyone can reproduce  
✅ **Evidence** - Screenshots + console errors  
✅ **Context** - Worked before, which platforms affected  
✅ **Business impact** - Justified high priority  
✅ **Suggested investigation** - Helped developers start faster  

### Time Savings

**Traditional way (Jira web):**
- Open Jira: 30s
- Create issue: 1min
- Fill 15 fields manually: 3min
- Find assignee: 30s
- Upload screenshots: 1min
- **Total: ~6 minutes**

**WorkHub way:**
- Ctrl+J: instant
- Type title: 10s
- Template pre-filled: 2min to complete
- Auto-detect assignee: 0s
- Drag-drop screenshots: 30s
- **Total: ~3 minutes**

**Saved: 50%** ⚡

---

**Last Updated:** 2026-03-22
