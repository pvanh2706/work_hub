# Hướng Dẫn Cho Support Team

Escalate customer issues nhanh chóng, track resolution, và contribute vào knowledge base.

## Quick Start

### Setup (3 phút)

```
1. Connect Jira: Settings → Integrations → Jira
2. Link support ticketing system (Zendesk/Freshdesk)
3. Set default template: Support Escalation
4. Join #support-escalations Slack channel
```

### First Escalation (< 2 phút)

```
1. From support ticket #12345
2. Click "Escalate to Engineering" button
3. System auto-fills:
   - Customer info
   - Issue description
   - Ticket link
4. Add priority and submit
→ Jira issue PROJ-888 created
→ Dev team notified
```

## Core Workflows

### Customer Issue Escalation

**When to Escalate:**
✅ **Yes - Escalate when:**
- Cannot find solution in knowledge base
- Issue requires code fix
- Affects multiple customers
- Security concern
- Data loss risk

❌ **No - Handle in support when:**
- Known issue with workaround
- User error / misunderstanding
- Configuration issue
- Already answered in docs

**Escalation Workflow:**
```
1. Support ticket comes in: "Cannot export report"

2. Search knowledge base (30 sec)
   → Search: "export report error"
   → No solution found

3. Reproduce issue (2 min)
   → Try in test account
   → Confirm bug exists

4. Escalate (1 min)
   → Click "Escalate" in ticket
   → System creates Jira issue:
     
     Title: "Export report fails with timeout error"
     Type: Bug
     Priority: High (multiple customers affected)
     Component: Reports
     
     Customer Context:
     - Customer: Acme Corp (Enterprise tier)
     - Ticket: #12345
     - Urgency: High (blocking their month-end report)
     
     Description:
     [Auto-copied from ticket]
     Customer reports timeout when exporting report > 1000 rows.
     
     Steps to Reproduce:
     1. Go to Reports → Sales Report
     2. Set date range: Last 3 months
     3. Click Export → Excel
     4. Error: "Request timeout"
     
     Attachments:
     - Screenshot from customer
     - Network trace
     - Support ticket link
     
5. Dev team picks up (within SLA)
   → Engineer assigned: John Doe
   → Status: In Progress
   → ETA: 2 days

6. Track progress
   → Check Jira status daily
   → Updates auto-posted to ticket
   → Customer kept informed

7. Fix deployed
   → Status: Done
   → Version: 2.5.3
   → Auto-notify customer

8. Verification
   → Test with customer
   → Confirm resolved
   → Close ticket
   → Document solution
```

### Ticket-to-Issue Linking

**Automatic linking:**
```
Support system → WorkHub integration
→ Ticket #12345 auto-linked to PROJ-888
→ Bidirectional sync:
  - Jira status → Ticket status
  - Jira comments → Ticket internal notes
  - Fix version → Ticket resolution note
```

**Manual linking:**
```
If auto-link fails:
1. Copy Jira issue key: PROJ-888
2. In support ticket → Custom field: Jira Issue
3. Paste: PROJ-888
4. Save → Link established
```

**View linked issues:**
```
Ticket Details → Related Issues
→ PROJ-888: Export report timeout (In Progress)
   Assigned: John Doe
   ETA: 2 days
   Last update: 2 hours ago
```

### Status Tracking

**Real-time sync:**
```
Jira Status → Support Ticket Status

To Do → Waiting for Engineering
In Progress → Being Investigated
In Review → Testing Fix
Done → Fixed - Pending Verification
Closed → Resolved
```

**Customer updates:**
```
Automated email to customer:

Subject: Update on your issue #12345

Hi [Customer],

Good news! The engineering team has made progress on your issue.

Status: In Progress → Testing Fix
Assigned: John Doe
ETA: Fix will be deployed tomorrow

What this means:
- The issue has been fixed
- Currently undergoing testing
- Will be deployed to production tomorrow
- You'll be notified when available

Jira Issue: PROJ-888
You can track progress at: [link]

Thanks for your patience!
```

## Knowledge Base Integration

### Search Before Escalate

**Knowledge base search:**
```
Support ticket: "Cannot reset password"

Quick search:
→ Search KB: "reset password"
→ Results:
  1. "How to reset password" (100% match)
     Solution: Use "Forgot Password" link
     Steps: [detailed guide]
     Success rate: 95%
  
  2. "Password reset email not received" (80% match)
     Common causes: Spam filter, wrong email
     Solution: Check spam, verify email address
     Success rate: 90%
     
  3. "Reset link expired" (75% match)
     Solution: Request new link (valid 24h)
     
→ Apply solution #1
→ Customer issue resolved
→ No escalation needed ✅
```

**When KB has partial solution:**
```
KB article: "Export report timeout (< 1000 rows)"
Customer issue: "Export report timeout (> 1000 rows)"

→ Similar but not exact match
→ Try workaround from KB
→ If doesn't work → Escalate with:
  "Tried KB solution #456, didn't work for > 1000 rows"
```

### Contribute Solutions

**After issue resolved:**
```
1. Issue fixed: PROJ-888
2. Customer verified it works
3. Add to knowledge base:

   Create KB Entry:
   Title: "Export Report Timeout for Large Datasets"
   
   Problem:
   Exporting reports with > 1000 rows times out
   
   Root Cause:
   Database query too slow for large datasets
   
   Solution:
   Fixed in version 2.5.3
   - Optimized query performance
   - Added pagination
   - Increased timeout to 60 seconds
   
   Workaround (if not on v2.5.3):
   - Export in batches (< 1000 rows)
   - Use date filters to reduce data
   
   Related:
   - Jira: PROJ-888
   - Tickets: #12345, #12456, #12567
   
   Tags: export, report, timeout, performance
```

**Benefits:**
- Future similar issues resolved faster
- Reduces escalations
- Team knowledge preserved
- Self-service for customers

### KB Quality

**Review KB entries:**
```
Monthly KB Review:
1. Check outdated entries
   → Remove if no longer relevant
   → Update if solution changed

2. Check success rate
   → Low success rate? Improve solution
   → High views but no resolution? Add details

3. Get feedback
   → Survey: "Was this helpful?"
   → Track: Yes/No/Partial

4. Update based on feedback
```

## Priority & SLA Management

### Priority Levels

**P0 - Critical (4h SLA):**
- System down for all users
- Data loss
- Security breach
- Payment processing blocked
- Example: "Login broken for all customers"

**P1 - High (1 day SLA):**
- Major feature broken
- Affects multiple customers
- Enterprise customer blocker
- Example: "Export failing for Enterprise accounts"

**P2 - Medium (3 days SLA):**
- Feature partially working
- Affects some users
- Workaround available
- Example: "UI glitch on mobile Safari"

**P3 - Low (1 week SLA):**
- Minor issue
- Enhancement request
- Few users affected
- Example: "Add dark mode"

### SLA Tracking

**Escalation SLA:**
```
Ticket #12345 created: 10:00 AM
Priority: P1 (High) → 1 day SLA

Timeline:
10:00 AM - Ticket created
10:30 AM - Reproduced by support
10:45 AM - Escalated to engineering (PROJ-888)
11:00 AM - Assigned to John Doe
02:00 PM - In Progress (3h from escalation ✅)
05:00 PM - Fix deployed to staging
Next Day 10:00 AM - Deployed to production
Next Day 11:00 AM - Verified by customer
→ Total: 25 hours (within 24h SLA ✅)
```

**SLA breach alerts:**
```
If SLA at risk:
- Alert sent 2 hours before breach
- Auto-escalate to manager
- Notify customer of delay
- Provide interim solution if possible
```

## Communication

### Customer Updates

**Best practices:**
✅ **Update proactively** - Don't wait for customer to ask  
✅ **Be transparent** - Share realistic timelines  
✅ **Use plain language** - Avoid technical jargon  
✅ **Set expectations** - Under-promise, over-deliver  

**Update template:**
```
Hi [Customer],

Thank you for reporting this issue.

What we know:
[Brief description of issue]

What we're doing:
Our engineering team is investigating (Jira: PROJ-888).
Assigned to: John Doe

Expected timeline:
- Investigation: Today
- Fix: Tomorrow
- Deployment: Day after tomorrow

We'll keep you updated every 24 hours or sooner if there's progress.

In the meantime, here's a workaround:
[Workaround if available]

Please let us know if you have questions.

Best regards,
[Support Team]
```

### Internal Communication

**With engineering:**
```
#support-escalations Slack channel:

[Support] New escalation: PROJ-888
Customer: Acme Corp (Enterprise)
Issue: Export timeout
Priority: P1 (blocking month-end report)
Ticket: #12345
Context: [link to full details]
cc: @dev-team @john-doe

[Engineering] @john-doe: On it. ETA 4 hours for fix.

[Support] Thanks! Customer notified.

[Engineering] @john-doe: Fix deployed to staging. 
Can you verify with customer on test account?

[Support] Verified ✅ Customer says it works!

[Engineering] @john-doe: Deploying to production now.
```

### Escalation Handoff

**Weekly handoff meetings:**
```
Agenda:
1. Review open escalations (15 min)
2. Discuss difficult issues (15 min)
3. Review recently resolved (10 min)
4. Process improvements (10 min)
5. Action items (10 min)

Example discussion:
- PROJ-888: Resolved, added to KB
- PROJ-999: Blocked waiting for third-party
- PROJ-777: Needs more customer info
```

## Reporting

### Support Escalation Report

**Weekly report:**
```
Week of March 15-22, 2026

Total Escalations: 25
├── P0 (Critical): 2
├── P1 (High): 8
├── P2 (Medium): 12
└── P3 (Low): 3

By Status:
├── Resolved: 18 (72%)
├── In Progress: 5 (20%)
└── Waiting: 2 (8%)

SLA Compliance: 95%
├── Within SLA: 24
└── Breached: 1 (PROJ-777 - waiting for customer)

Top Issues:
1. Export timeouts (5 escalations) → KB article added
2. Login problems (3 escalations) → Bug fix deployed
3. UI glitches (2 escalations) → In progress

Resolution Time:
├── Average: 2.5 days
├── Fastest: 4 hours (P0)
└── Slowest: 5 days (P3, low priority)

Customer Satisfaction: 4.5/5
```

### Trend Analysis

**Monthly trends:**
```
Escalation trends:
- March: 100 escalations (↓15% from Feb)
- Top categories:
  1. Performance issues: 30%
  2. Bugs: 25%
  3. Feature requests: 20%
  4. User errors: 15%
  5. Other: 10%

Insights:
✅ Performance issues decreasing (was 40% in Feb)
⚠️ Bugs increasing (was 15% in Feb)
→ Action: More QA testing before release
```

## Tools & Integrations

### Support Ticketing Systems

**Zendesk integration:**
```
Settings → Integrations → Zendesk
→ API key: [your-key]
→ Email mapping: enabled
→ Auto-escalation: enabled

Features:
- Create Jira issue from ticket (one click)
- Sync status bidirectionally
- Link tickets to issues automatically
- Post Jira updates as ticket notes
```

**Freshdesk integration:**
```
Similar features:
- Escalation button in ticket
- Status sync
- Comment sync
- SLA tracking
```

### Slack Integration

**Notifications:**
```
#support-escalations channel:
- New escalations posted
- Status changes
- SLA warnings
- Resolution notifications
```

**Slash commands:**
```
/escalate #12345 → Create Jira issue from ticket
/jira PROJ-888 → Show issue status
/sla #12345 → Check SLA status
```

## Best Practices

### 1. Search Knowledge Base First
✅ Always search before escalating  
✅ Try documented solutions  
✅ Update KB with new solutions  

### 2. Provide Complete Context
✅ Customer tier (affects priority)  
✅ Business impact  
✅ Reproduction steps  
✅ Evidence (screenshots, logs)  

### 3. Set Realistic Expectations
✅ Honest timelines  
✅ Under-promise, over-deliver  
❌ Don't promise immediate fix  

### 4. Keep Customer Informed
✅ Proactive updates  
✅ Clear communication  
✅ Explain what's happening  

### 5. Document Everything
✅ Solutions in KB  
✅ Workarounds  
✅ Customer feedback  

### 6. Learn from Issues
✅ Review resolved issues  
✅ Identify patterns  
✅ Suggest improvements  

## Troubleshooting

### Cannot Escalate Ticket

**Check:**
1. Jira connection active
2. Have permission to create issues
3. Required fields filled
4. Ticket system integration configured

### Issue Not Syncing to Ticket

**Verify:**
1. Ticket-issue link exists
2. Integration enabled
3. API credentials valid
4. Check sync logs

### Customer Not Receiving Updates

**Check:**
1. Email notifications enabled
2. Customer email correct
3. Not in spam filter
4. Ticket status synced

## Next Steps

- [Escalation Template](../05-configuration/escalation-template.md)
- [Knowledge Base Setup](../../knowledge/03-user-guides/contributing.md)
- [SLA Configuration](../05-configuration/sla-setup.md)

---

**Last Updated:** 2026-03-22
