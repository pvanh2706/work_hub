# Jira Module - Database Schema

PostgreSQL database schema for Jira Module.

## Tables

### jira.IssueTemplates

**Purpose:** Store issue templates for quick creation

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UUID | PK | Template ID |
| `Name` | VARCHAR(200) | NOT NULL | Template name |
| `Type` | VARCHAR(50) | NOT NULL | Issue type (Bug, Feature, etc.) |
| `Description` | TEXT | | Template description |
| `FieldsJson` | JSONB | NOT NULL | Template field configuration |
| `Visibility` | VARCHAR(20) | NOT NULL DEFAULT 'Personal' | Personal/Team/Organization |
| `UsageCount` | INT | DEFAULT 0 | How many times used |
| `IsActive` | BOOLEAN | DEFAULT true | Active or archived |
| `CreatedBy` | UUID | FK | User who created |
| `CreatedAt` | TIMESTAMPTZ | NOT NULL | Creation timestamp |
| `UpdatedBy` | UUID | FK | Last updater |
| `UpdatedAt` | TIMESTAMPTZ | | Last update timestamp |

**Indexes:**
```sql
CREATE INDEX IX_IssueTemplates_Type ON jira.IssueTemplates(Type);
CREATE INDEX IX_IssueTemplates_CreatedBy ON jira.IssueTemplates(CreatedBy);
CREATE INDEX IX_IssueTemplates_Visibility_IsActive 
    ON jira.IssueTemplates(Visibility, IsActive);
```

**Example row:**
```json
{
  \"Id\": \"3fa85f64-5717-4562-b3fc-2c963f66afa6\",
  \"Name\": \"Bug Report\",
  \"Type\": \"Bug\",
  \"FieldsJson\": {
    \"priority\": \"High\",
    \"labels\": [\"bug\", \"needs-investigation\"],
    \"descriptionTemplate\": \"## Bug\\n{description}...\"
  },
  \"Visibility\": \"Team\",
  \"UsageCount\": 145,
  \"IsActive\": true,
  \"CreatedAt\": \"2026-01-15T10:00:00Z\"
}
```

### jira.JiraIssueSyncs

**Purpose:** Track Jira issues and sync status

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UUID | PK | Internal ID |
| `JiraKey` | VARCHAR(50) | NOT NULL, UNIQUE | PROJ-123 |
| `JiraId` | VARCHAR(50) | NOT NULL | Jira internal ID |
| `ProjectKey` | VARCHAR(20) | NOT NULL | PROJ |
| `Title` | VARCHAR(500) | NOT NULL | Issue title/summary |
| `Description` | TEXT | | Full description |
| `Type` | VARCHAR(50) | NOT NULL | Bug/Feature/Task |
| `Priority` | VARCHAR(20) | | Highest/High/Medium/Low |
| `Status` | VARCHAR(50) | NOT NULL | To Do/In Progress/Done |
| `AssigneeId` | UUID | FK | Assigned user |
| `ReporterId` | UUID | FK | Creator |
| `LabelsJson` | JSONB | | Array of labels |
| `ComponentsJson` | JSONB | | Array of components |
| `SprintId` | VARCHAR(50) | | Current sprint ID |
| `StoryPoints` | DECIMAL(5,2) | | Estimated points |
| `SyncStatus` | VARCHAR(20) | NOT NULL DEFAULT 'Synced' | Synced/Pending/Failed |
| `LastSyncedAt` | TIMESTAMPTZ | | Last successful sync |
| `SyncErrorMessage` | TEXT | | Error if sync failed |
| `JiraUrl` | VARCHAR(500) | | Direct link to Jira |
| `CreatedAt` | TIMESTAMPTZ | NOT NULL | |
| `UpdatedAt` | TIMESTAMPTZ | | |

**Indexes:**
```sql
CREATE UNIQUE INDEX IX_JiraIssueSyncs_JiraKey 
    ON jira.JiraIssueSyncs(JiraKey);
CREATE INDEX IX_JiraIssueSyncs_ProjectKey_Status 
    ON jira.JiraIssueSyncs(ProjectKey, Status);
CREATE INDEX IX_JiraIssueSyncs_AssigneeId 
    ON jira.JiraIssueSyncs(AssigneeId);
CREATE INDEX IX_JiraIssueSyncs_SyncStatus 
    ON jira.JiraIssueSyncs(SyncStatus);
CREATE INDEX IX_JiraIssueSyncs_UpdatedAt 
    ON jira.JiraIssueSyncs(UpdatedAt DESC);
```

### jira.WorkLogEntries

**Purpose:** Track work time logged to issues

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UUID | PK | Entry ID |
| `IssueKey` | VARCHAR(50) | NOT NULL, FK | PROJ-123 |
| `JiraWorklogId` | VARCHAR(50) | UNIQUE | Jira's worklog ID |
| `TimeSpentSeconds` | INT | NOT NULL | Time in seconds |
| `StartedAt` | TIMESTAMPTZ | NOT NULL | When work started |
| `AuthorId` | UUID | FK | Who logged work |
| `Comment` | TEXT | | Work description |
| `Source` | VARCHAR(20) | NOT NULL | Git/Manual/Timer |
| `CommitHash` | VARCHAR(40) | | If from git commit |
| `SyncedToJira` | BOOLEAN | DEFAULT false | Synced? |
| `SyncedAt` | TIMESTAMPTZ | | When synced |
| `CreatedAt` | TIMESTAMPTZ | NOT NULL | |

**Indexes:**
```sql
CREATE INDEX IX_WorkLogEntries_IssueKey 
    ON jira.WorkLogEntries(IssueKey);
CREATE INDEX IX_WorkLogEntries_AuthorId_StartedAt 
    ON jira.WorkLogEntries(AuthorId, StartedAt DESC);
CREATE INDEX IX_WorkLogEntries_SyncedToJira 
    ON jira.WorkLogEntries(SyncedToJira) WHERE SyncedToJira = false;
```

### jira.SyncLogs

**Purpose:** Audit trail of sync operations

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | BIGSERIAL | PK | Log ID |
| `Timestamp` | TIMESTAMPTZ | NOT NULL DEFAULT NOW() | When |
| `Direction` | VARCHAR(20) | NOT NULL | ToJira/FromJira |
| `IssueKey` | VARCHAR(50) | | Issue synced |
| `Operation` | VARCHAR(20) | NOT NULL | Create/Update/Delete |
| `FieldsChangedJson` | JSONB | | Which fields |
| `Success` | BOOLEAN | NOT NULL | Success? |
| `ErrorMessage` | TEXT | | Error if failed |
| `DurationMs` | INT | | How long |
| `RetryCount` | INT | DEFAULT 0 | Retry attempts |
| `UserId` | UUID | | Initiating user |

**Indexes:**
```sql
CREATE INDEX IX_SyncLogs_Timestamp 
    ON jira.SyncLogs(Timestamp DESC);
CREATE INDEX IX_SyncLogs_IssueKey 
    ON jira.SyncLogs(IssueKey);
CREATE INDEX IX_SyncLogs_Success 
    ON jira.SyncLogs(Success) WHERE Success = false;
```

### jira.TemplateVariables

**Purpose:** Custom template variables

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | UUID | PK | Variable ID |
| `TemplateId` | UUID | FK | Parent template |
| `Name` | VARCHAR(100) | NOT NULL | Variable name |
| `DefaultValue` | VARCHAR(500) | | Default value |
| `IsRequired` | BOOLEAN | DEFAULT false | Required? |
| `ValidationRegex` | VARCHAR(500) | | Validation pattern |

## Relationships

```sql
-- IssueTemplates → Users
ALTER TABLE jira.IssueTemplates
  ADD CONSTRAINT FK_IssueTemplates_CreatedBy 
  FOREIGN KEY (CreatedBy) REFERENCES users.Users(Id);

-- JiraIssueSyncs → Users  
ALTER TABLE jira.JiraIssueSyncs
  ADD CONSTRAINT FK_JiraIssues_Assignee
  FOREIGN KEY (AssigneeId) REFERENCES users.Users(Id);

ALTER TABLE jira.JiraIssueSyncs
  ADD CONSTRAINT FK_JiraIssues_Reporter
  FOREIGN KEY (ReporterId) REFERENCES users.Users(Id);

-- WorkLogEntries → JiraIssueSyncs
ALTER TABLE jira.WorkLogEntries
  ADD CONSTRAINT FK_WorkLogs_Issue
  FOREIGN KEY (IssueKey) 
  REFERENCES jira.JiraIssueSyncs(JiraKey)
  ON DELETE CASCADE;

-- TemplateVariables → IssueTemplates
ALTER TABLE jira.TemplateVariables
  ADD CONSTRAINT FK_Variables_Template
  FOREIGN KEY (TemplateId)
  REFERENCES jira.IssueTemplates(Id)
  ON DELETE CASCADE;
```

## ERD

```
┌─────────────────────┐
│   IssueTemplates    │
├─────────────────────┤
│ • Id (PK)           │
│ • Name              │
│ • Type              │
│ • FieldsJson        │
│ • CreatedBy (FK)    │
└──────────┬──────────┘
           │
           │ 1:N
           │
┌──────────▼──────────┐
│ TemplateVariables   │
├─────────────────────┤
│ • Id (PK)           │
│ • TemplateId (FK)   │
│ • Name              │
│ • DefaultValue      │
└─────────────────────┘


┌─────────────────────┐
│  JiraIssueSyncs     │
├─────────────────────┤
│ • Id (PK)           │
│ • JiraKey (UQ)      │
│ • Title             │
│ • Status            │
│ • AssigneeId (FK)   │
│ • ReporterId (FK)   │
└──────────┬──────────┘
           │
           │ 1:N
           │
┌──────────▼──────────┐
│  WorkLogEntries     │
├─────────────────────┤
│ • Id (PK)           │
│ • IssueKey (FK)     │
│ • TimeSpentSeconds  │
│ • AuthorId (FK)     │
│ • Source            │
└─────────────────────┘


┌─────────────────────┐
│     SyncLogs        │
├─────────────────────┤
│ • Id (PK)           │
│ • Timestamp         │
│ • IssueKey          │
│ • Operation         │
│ • Success           │
└─────────────────────┘
```

## Migrations

### Create Migration

```bash
cd src/Modules/Jira/WorkHub.Modules.Jira.Infrastructure
dotnet ef migrations add AddJiraTables --startup-project ../../../WorkHub.API/
```

### Apply Migration

```bash
dotnet ef database update --startup-project ../../../WorkHub.API/
```

### Migration Example

```csharp
public partial class AddJiraTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: \"jira\");
        
        migrationBuilder.CreateTable(
            name: \"IssueTemplates\",
            schema: \"jira\",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                Type = table.Column<string>(maxLength: 50, nullable: false),
                FieldsJson = table.Column<string>(type: \"jsonb\", nullable: false),
                // ... other columns
            },
            constraints: table =>
            {
                table.PrimaryKey(\"PK_IssueTemplates\", x => x.Id);
            });
        
        migrationBuilder.CreateIndex(
            name: \"IX_IssueTemplates_Type\",
            schema: \"jira\",
            table: \"IssueTemplates\",
            column: \"Type\");
    }
}
```

## Seed Data

```sql
-- Default templates
INSERT INTO jira.IssueTemplates (Id, Name, Type, FieldsJson, Visibility, CreatedBy, CreatedAt)
VALUES 
(gen_random_uuid(), 'Bug Report', 'Bug', '{\"priority\":\"High\"}', 'Team', '...', NOW()),
(gen_random_uuid(), 'Feature Request', 'Feature', '{\"priority\":\"Medium\"}', 'Team', '...', NOW());
```

## Performance Optimization

### Partitioning (for SyncLogs)

```sql
-- Partition by month
CREATE TABLE jira.SyncLogs_2026_03 PARTITION OF jira.SyncLogs
    FOR VALUES FROM ('2026-03-01') TO ('2026-04-01');

CREATE TABLE jira.SyncLogs_2026_04 PARTITION OF jira.SyncLogs
    FOR VALUES FROM ('2026-04-01') TO ('2026-05-01');
```

### Query Optimization

```sql
-- Find issues needing sync
SELECT Id, JiraKey, SyncStatus
FROM jira.JiraIssueSyncs
WHERE SyncStatus = 'Pending'
  AND UpdatedAt > NOW() - INTERVAL '1 hour'
ORDER BY UpdatedAt ASC
LIMIT 100;
```

## Backup & Maintenance

### Backup

```bash
pg_dump -Fc -t jira.* workhub_db > jira_module_backup.dump
```

### Restore

```bash
pg_restore -d workhub_db jira_module_backup.dump
```

### Cleanup old logs

```sql
-- Delete sync logs older than 90 days
DELETE FROM jira.SyncLogs
WHERE Timestamp < NOW() - INTERVAL '90 days';
```

---

**Last Updated:** 2026-03-22
