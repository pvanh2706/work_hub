# Jira Connection Setup

Step-by-step guide to connect WorkHub with your Jira account.

## Prerequisites

- Jira Cloud account (not Server/Data Center)
- Admin access to Jira OR permission to create API tokens
- WorkHub account with appropriate permissions

## Step 1: Generate Jira API Token

### 1.1 Go to Atlassian Account Settings

1. Open browser
2. Navigate to: https://id.atlassian.com/manage-profile/security/api-tokens
3. Login with your Atlassian account

### 1.2 Create API Token

1. Click "Create API token" button
2. Enter label: "WorkHub Integration"
3. Click "Create"
4. **IMPORTANT:** Copy the token immediately (you won't see it again!)
5. Save it securely (password manager recommended)

**Screenshot locations would go here in actual doc**

## Step 2: Get Jira Domain

Find your Jira Cloud domain (format: `https://your-company.atlassian.net`)

**Where to find:**
- In Jira → Look at browser URL
- Example: `https://acme-corp.atlassian.net/jira/software/projects/PROJ/boards/1`
- Your domain is: `acme-corp.atlassian.net`

## Step 3: Configure in WorkHub

### 3.1 Open Settings

1. Login to WorkHub
2. Click Settings icon (top right)
3. Navigate to: Integrations → Jira

### 3.2 Enter Connection Details

Fill in the form:
```
Jira Domain: your-company.atlassian.net
Email: your-email@company.com
API Token: [paste token from Step 1]
Default Project: PROJ (optional)
```

### 3.3 Test Connection

1. Click "Test Connection" button
2. Wait for verification (5-10 seconds)
3. Expected result: ✅ "Connection successful!"

**If failed:**
- ❌ Invalid credentials → Check email/token
- ❌ Network error → Check firewall/VPN
- ❌ Permission denied → Need Jira access

## Step 4: Configure Default Settings

### 4.1 Default Project

Set default project for quick issue creation:
```
Default Project Key: PROJ
```

### 4.2 Auto-Sync Settings

```
☑ Enable real-time sync (webhooks)
☑ Enable auto-log work from commits
☐ Enable offline mode
Sync interval: 15 minutes
```

### 4.3 Template Settings

```
☑ Use team templates
☑ Auto-detect issue type from keywords
Default assignee: Component owner
```

## Step 5: Verify Setup

### 5.1 Create Test Issue

1. Press Ctrl+J
2. Type: "Test issue - please ignore"
3. Press Enter
4. Check: Issue created in Jira?

### 5.2 Verify Sync

1. In Jira: Change status of test issue to "In Progress"
2. In WorkHub: Check if status updated (wait max 30 seconds)
3. Expected: Status synced ✅

### 5.3 Clean Up

1. In WorkHub or Jira: Delete test issue
2. Verify deleted in both systems

## Troubleshooting

### Connection Failed

**Error: "Invalid credentials"**
- Check email is exact (case-sensitive)
- Regenerate API token and try again
- Ensure using Jira Cloud (not Server)

**Error: "Network timeout"**
- Check internet connection
- Disable VPN temporarily
- Check corporate firewall settings

**Error: "Permission denied"**
- Contact Jira admin for permissions
- Need: Browse projects, Create issues, Edit issues

### Sync Not Working

**Webhook not receiving updates:**
1. Admin → Jira → Webhooks → Check registration
2. Re-register webhook if missing
3. Check WorkHub URL is accessible from Jira

**Polling not working:**
1. Check sync logs: Admin → Sync Status
2. Look for errors
3. Verify API token still valid

## Security Best Practices

### API Token Security

✅ **Do:**
- Store token in password manager
- Rotate token every 90 days
- Use separate token per integration
- Revoke immediately if compromised

❌ **Don't:**
- Share token with others
- Commit token to git
- Email token
- Reuse token across services

### Access Control

- Use service account for WorkHub integration
- Grant minimum required permissions
- Review access logs monthly
- Remove access for inactive users

## Next Steps

- [Template Setup](template-setup.md) - Create your first template
- [Workflow Mapping](workflow-mapping.md) - Map Jira workflows
- [User Guide](../03-user-guides/for-developers.md) - Start using Jira module

---

**Last Updated:** 2026-03-22
