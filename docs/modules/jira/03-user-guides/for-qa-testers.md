# Hướng Dẫn Cho QA/Testers

Report bugs nhanh chóng, tìm issues tương tự, và track verification hiệu quả.

## Quick Start

### Setup (2 phút)

```
1. Connect Jira: Settings → Integrations → Jira
2. Set default project: PROJ
3. Install screenshot tool (optional but recommended)
```

### First Bug Report (30 giây)

```
1. Press Ctrl+J
2. Type: "Login button not clickable on iPhone"
3. Press Enter
→ Bug PROJ-123 created with template
```

## Report Bugs Effectively

### Quick Bug Report Workflow

**Method 1: Quick Command (Fastest)**
```
1. Ctrl+J
2. Type bug title: "Checkout fails after applying coupon"
3. Enter
4. System auto-detects:
   - Type: Bug
   - Priority: High (keyword "fails")
   - Component: Checkout (keyword detection)
5. Fill template:
   - Steps to reproduce (required)
   - Expected vs Actual (required)
   - Environment (auto-filled)
6. Attach screenshot (drag & drop)
7. Submit
```

**Method 2: From Bug Template**
```
1. Jira → Create → Bug Report Template
2. Title: Clear, specific
   ✅ Good: "Payment fails when cart total > $1000"
   ❌ Bad: "Payment not working"
3. Fill all sections (template guides you)
4. Attach evidence
5. Create
```

### Screenshot & Recording Tools

**Built-in screenshot:**
```
1. While filling bug form
2. Click "Attach Screenshot"
3. Select area to capture
4. Annotate if needed
5. Screenshot auto-uploaded
```

**Third-party tools (recommended):**
- **Snagit**: Advanced editing, video recording
- **ShareX**: Free, customizable
- **Greenshot**: Lightweight, integrates with Jira

**Video recording:**
```
For complex bugs:
1. Use Loom / OBS / ShareX
2. Record reproduction steps
3. Upload video (WorkHub supports video attachments)
4. Link in issue
```

### Bug Template Best Practices

**Good Bug Report Example:**
```markdown
## 🐛 Bug: Checkout fails when cart total > $1000

## 📋 Steps to Reproduce
1. Add 5x $250 items to cart (total: $1250)
2. Go to checkout
3. Enter payment details (card: 4242 4242 4242 4242)
4. Click "Place Order"

## ✅ Expected Behavior
- Order placed successfully
- Confirmation email sent
- Redirected to order confirmation page

## ❌ Actual Behavior
- Error popup: "Payment processing failed"
- No error details shown
- Cart cleared (items lost!)

## 📱 Environment
- Version: 2.5.1 (production)
- Browser: Chrome 120.0.6099.109
- OS: macOS 14.2
- Screen: Desktop (1920x1080)

## 📸 Evidence
[Screenshot: error-popup.png]
[Video: reproduction-steps.mp4]

## 🔍 Console Errors
```
TypeError: Cannot read property 'total' of undefined
  at checkout.js:234
  at PaymentService.processPayment (payment.js:89)
```

## 💡 Additional Info
- Bug does NOT occur when total < $1000
- Tested with 3 different cards, same result
- Cache cleared, issue persists
- Bug appeared after v2.5.1 deployment (worked in v2.5.0)

## 🚨 Severity
HIGH - Blocks all orders > $1000
Estimated customer impact: ~30 orders/day
```

**What makes it good:**
✅ Clear title  
✅ Numbered reproduction steps  
✅ Expected vs Actual clearly stated  
✅ Full environment details  
✅ Screenshots AND video  
✅ Console errors included  
✅ Severity with business impact  

## Finding Similar Issues

### Before Creating Bug

**Search for duplicates:**
```
1. Click "Search Similar" (shows before creating)
2. System searches by:
   - Title keywords
   - Component
   - Error messages
3. Shows results:
   
   Similar Issues Found:
   ├── PROJ-100 (90% match): "Checkout error for large orders"
   │   Status: In Progress
   │   Assignee: John Doe
   │   Comment: "Working on fix, deploy next week"
   │
   └── PROJ-85 (75% match): "Payment timeout on high amounts"
       Status: Closed
       Resolution: Fixed in v2.4.5
       
   [Create Anyway] [Link to PROJ-100]
```

**Link to existing:**
```
If similar exists:
1. Click "Link to PROJ-100"
2. Your details added as comment
3. You're added as watcher
4. No duplicate created ✅
```

### Advanced Search

**Find related bugs:**
```
Search: type = Bug AND component = Checkout AND status != Done
→ Shows all open checkout bugs
→ Review before creating new
```

**Search by error message:**
```
Search: "Cannot read property 'total' of undefined"
→ Finds all bugs with same error
```

## Test Management

### Link Test Cases to Issues

**When creating bug:**
```
Bug Report → Related Test Case
→ Select: TC-456 "Checkout Happy Path"
→ Links automatically
```

**Benefits:**
- See which test failed
- Re-run test after fix
- Track test coverage

### Verification Workflow

**Bug assigned to you for verification:**
```
1. Notification: "PROJ-123 ready for verification"
2. Open issue in WorkHub
3. Review:
   - Original bug description
   - Developer's fix description
   - Changed files (if linked)
4. Test on staging:
   → Follow original reproduction steps
   → Try edge cases
   → Check regression
5. Result:
   ✅ Verified - Works as expected
      → Transition to "Done"
      → Add comment: "Verified on staging v2.5.2"
   
   ❌ Still broken
      → Reopen
      → Add comment: "Still fails when..."
      → Attach new evidence
```

### Test Coverage Tracking

**Link bugs to test cases:**
```
Bug PROJ-123 → Test Cases tab
→ Failed in: TC-456, TC-789
→ Should add coverage: TC-999 (edge case)
```

**Test case document:**
```markdown
# Test Case: Checkout with Large Order

## Test Steps
1. Add items totaling > $1000
2. Proceed to checkout
3. Enter valid payment
4. Submit order

## Expected Result
Order placed successfully

## Related Bugs
- PROJ-123: Fixed in v2.5.2 ✅
- PROJ-456: In progress 🔄

## Last Run
Date: 2026-03-22
Result: PASSED ✅
Tested by: QA Team
Environment: Staging
```

## Regression Testing

### Track Regressions

**Mark as regression:**
```
If bug reappears:
1. Update original bug PROJ-123
2. Add label: "regression"
3. Link to original fix
4. Escalate priority: High → Highest
```

**Regression report:**
```
Reports → Regressions
→ Shows bugs that reopened after fix
→ Trend analysis
→ Root cause patterns
```

### Regression Test Suite

**Create regression suite:**
```
Test Suites → New → "Checkout Regressions"
→ Add all bugs fixed in Checkout component
→ Run before each release
→ Automated where possible
```

## Automation Integration

### Automated Test Results

**Link test runs to issues:**
```
Cypress test fails → Auto-creates bug
{
  "test": "Checkout Happy Path",
  "status": "failed",
  "error": "Payment timeout",
  "screenshot": "auto-captured.png",
  "video": "test-run.mp4"
}

→ Bug PROJ-999 created automatically
→ Assignee: Component owner
→ All evidence attached
```

### CI/CD Integration

**Failed deployment blocks release:**
```
Jenkins build fails on staging
→ Creates blocker: PROJ-777
→ Type: Bug
→ Priority: Highest
→ Blocks: Release v2.5.2
→ Assigned: DevOps team
```

## Severity & Priority Guidelines

### Severity Levels

**Critical (P0 - Highest):**
- System down / unusable
- Data loss / corruption
- Security vulnerability
- Affects ALL users
- Example: "App crashes on login"

**High (P1 - High):**
- Major feature broken
- Workaround exists but difficult
- Affects many users
- Example: "Checkout fails for orders > $1000"

**Medium (P2 - Medium):**
- Feature partially broken
- Easy workaround
- Affects some users
- Example: "Search filters reset on page refresh"

**Low (P3 - Low):**
- Minor issue
- Cosmetic problem
- Affects few users
- Example: "Button alignment off by 2px"

### Priority vs Severity

**Severity:** Technical impact  
**Priority:** Business urgency

**Example:**
```
Bug: "Logo misaligned on homepage"
- Severity: Low (cosmetic)
- Priority: HIGH (major client demo tomorrow!)

Bug: "Export feature broken in admin panel"
- Severity: High (feature unusable)
- Priority: Low (admin uses workaround, 3 users affected)
```

## Common Workflows

### Daily Routine

**Morning (30 min):**
```
1. Check new bugs assigned (10 min)
2. Verify fixed bugs from yesterday (15 min)
3. Review test suite failures (5 min)
```

**During Testing (varies):**
```
1. Execute test cases
2. Report bugs immediately (don't batch)
3. Verify fixes as they come
4. Update test results
```

**End of Day (15 min):**
```
1. Close verified bugs
2. Update test coverage report
3. Plan tomorrow's testing
```

### Sprint Testing Workflow

**Sprint Start:**
```
1. Review sprint issues
2. Plan test cases
3. Setup test environment
```

**Mid-Sprint:**
```
1. Test completed features daily
2. Report bugs with priority
3. Verify fixes quickly
4. Regression test before demo
```

**Sprint End:**
```
1. Full regression test
2. Verify all bugs closed
3. Sign off on release
4. Document known issues
```

### Release Testing

**Pre-Release (1 week before):**
```
1. Full regression suite (2 days)
2. New feature testing (2 days)
3. Performance testing (1 day)
4. Security testing (1 day)
5. Sign-off meeting (1 day)
```

**Release Day:**
```
1. Smoke tests on staging (1 hour)
2. Deploy to production
3. Smoke tests on production (30 min)
4. Monitor for 24 hours
5. Post-release report
```

## Best Practices

### 1. Reproduce Before Reporting
✅ Ensure bug is reproducible  
✅ Try different browsers/devices  
❌ Don't report "it happened once"  

### 2. Clear Evidence
✅ Screenshot + Video  
✅ Console errors  
✅ Step-by-step reproduction  
❌ Don't say "it doesn't work"  

### 3. Search First
✅ Search for duplicates  
✅ Link to similar issues  
❌ Don't create duplicate bugs  

### 4. Timely Verification
✅ Verify fixes within 24 hours  
✅ Test thoroughly  
❌ Don't delay verification  

### 5. Detailed Reports
✅ Complete template fields  
✅ Business impact included  
❌ Don't leave fields empty  

### 6. Good Communication
✅ Update status regularly  
✅ Comment on progress  
✅ Tag developers if blocked  
❌ Don't go silent  

## Troubleshooting

### Cannot Create Bug

**Check:**
1. Jira connection: Settings → Jira → Test
2. Permissions: Can you create issues in project?
3. Required fields: Title and description filled?

### Screenshot Not Attaching

**Solutions:**
1. Check file size < 10 MB
2. Try different format (PNG instead of BMP)
3. Use drag & drop instead of browse
4. Check browser permissions

### Search Not Finding Issues

**Improve search:**
1. Use keywords from title
2. Check spelling
3. Search across all statuses (not just open)
4. Use component filters

## Next Steps

- [Bug Report Template](../05-configuration/bug-report-template.md)
- [Test Management](../06-examples/test-case-management.md)
- [Advanced Search](../06-examples/advanced-search-filters.md)

---

**Last Updated:** 2026-03-22
