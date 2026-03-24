# WorkHub Documentation System - Design Specification

**Date:** 2026-03-22  
**Author:** AI Assistant + User collaboration  
**Status:** Approved  
**Version:** 1.0

---

## 1. Tổng Quan

### 1.1. Mục Tiêu
Xây dựng hệ thống tài liệu toàn diện cho WorkHub và các sản phẩm khác trong công ty, phục vụ:
- **Developers** (Backend, Frontend, Mobile)
- **QA/Testers**
- **DevOps/Infrastructure**
- **Product Managers**
- **Business Analysts**
- **Support Team**
- **Marketing/Sales**
- **New Hires** (Onboarding)
- **AI Assistants** (GitHub Copilot, Claude, etc.)

### 1.2. Phạm Vi
- **Quy mô:** Enterprise (15+ sản phẩm)
- **Ngôn ngữ:** Tiếng Việt
- **Ưu tiên:** Module Jira (triển khai trước)
- **Phát triển:** Solo developer, scale dần lên team

### 1.3. Yêu Cầu Đặc Biệt
- **Hybrid approach:** Static docs (Git) + Dynamic knowledge tree (Database)
- **Template framework:** Có thể tái sử dụng cho các sản phẩm khác
- **AI-friendly:** Hai bộ docs riêng - một cho người, một cho AI
- **User contributions:** Workflow đóng góp và duyệt nội dung

---

## 2. Kiến Trúc Tổng Thể

### 2.1. Three-Tier Documentation Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    DOCUMENTATION SYSTEM                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────┐ │
│  │  Static Docs     │  │  Dynamic         │  │    AI     │ │
│  │  (Git/Markdown)  │  │  Knowledge Tree  │  │  Context  │ │
│  │                  │  │  (Database)      │  │           │ │
│  │ • Technical      │  │ • User           │  │ • Patterns│ │
│  │ • User Guides    │  │   Contributions  │  │ • Commands│ │
│  │ • API Docs       │  │ • Community      │  │ • Domain  │ │
│  │ • Templates      │  │   Driven         │  │   Logic   │ │
│  │                  │  │ • Rich Content   │  │           │ │
│  └──────────────────┘  └──────────────────┘  └───────────┘ │
│         │                      │                    │        │
│         └──────────────────────┴────────────────────┘        │
│                              │                               │
│                    Unified Access Layer                      │
│         (Developers, Users, Support, QA, AI)                 │
└─────────────────────────────────────────────────────────────┘
```

### 2.2. Component Roles

| Component | Purpose | Technology | Audience | Update Frequency |
|-----------|---------|------------|----------|------------------|
| **Static Docs** | Technical documentation, tutorials | Markdown in Git | Developers, DevOps | On code changes |
| **Dynamic Knowledge** | User contributions, troubleshooting | PostgreSQL + UI | All teams | Real-time |
| **AI Context** | Pattern library, quick reference | Repository memory | AI Assistants | On pattern changes |

---

## 3. Static Documentation Structure

### 3.1. Folder Organization

```
WorkHub/
└── docs/
    ├── README.md                          # Entry point - navigation hub
    ├── GETTING-STARTED.md                 # Quick start guide
    ├── ARCHITECTURE.md                    # Existing - ADRs
    ├── DEPLOYMENT.md                      # Production deployment
    │
    ├── templates/                         # Reusable templates
    │   ├── README.md
    │   ├── product-docs-structure/
    │   │   ├── 01-overview.template.md
    │   │   ├── 02-getting-started.template.md
    │   │   ├── 03-user-guides.template/
    │   │   ├── 04-technical.template/
    │   │   └── 05-business.template/
    │   └── ai-context-structure/
    │       ├── product-overview.template.md
    │       ├── architecture-patterns.template.md
    │       └── business-domain.template.md
    │
    ├── user-guides/                       # Non-technical users
    │   ├── README.md
    │   ├── for-product-managers.md
    │   ├── for-business-analysts.md
    │   ├── for-qa-testers.md
    │   ├── for-support-team.md
    │   └── for-end-users.md
    │
    ├── technical/                         # Technical staff
    │   ├── README.md
    │   ├── development-setup.md
    │   ├── coding-standards.md
    │   ├── api-documentation.md
    │   ├── database-schema.md
    │   ├── deployment-guide.md
    │   └── troubleshooting.md
    │
    ├── modules/                           # Per-module documentation
    │   ├── jira/                         # 🎯 PRIORITY MODULE
    │   │   ├── README.md
    │   │   ├── 01-overview.md
    │   │   ├── 02-features/
    │   │   │   ├── quick-issue-creation.md
    │   │   │   ├── issue-templates.md
    │   │   │   ├── auto-log-work.md
    │   │   │   ├── sync-jira.md
    │   │   │   └── bulk-operations.md
    │   │   ├── 03-user-guides/
    │   │   │   ├── for-developers.md
    │   │   │   ├── for-project-managers.md
    │   │   │   ├── for-qa-testers.md
    │   │   │   └── for-support-team.md
    │   │   ├── 04-technical/
    │   │   │   ├── architecture.md
    │   │   │   ├── api-endpoints.md
    │   │   │   ├── database-schema.md
    │   │   │   ├── domain-models.md
    │   │   │   └── jira-api-integration.md
    │   │   ├── 05-configuration/
    │   │   │   ├── jira-connection.md
    │   │   │   ├── template-setup.md
    │   │   │   └── workflow-mapping.md
    │   │   └── 06-examples/
    │   │       ├── create-bug-report.md
    │   │       ├── create-feature-request.md
    │   │       └── automation-scenarios.md
    │   │
    │   ├── knowledge/
    │   ├── tasks/
    │   ├── ai/
    │   ├── organization/
    │   └── workspace/
    │
    └── business/                          # Business stakeholders
        ├── product-vision.md
        ├── feature-roadmap.md
        ├── business-value.md
        └── competitive-analysis.md
```

### 3.2. Module Documentation Template

Mỗi module sẽ follow structure 6 phần:

**01-overview.md:**
- Vấn đề cần giải quyết
- Giải pháp module cung cấp
- So sánh Before/After
- Key benefits

**02-features/:** Chi tiết từng tính năng
- Cách hoạt động
- UI/UX flow
- Screenshots/Diagrams
- Ví dụ cụ thể

**03-user-guides/:** Hướng dẫn theo role
- Developers
- Project Managers
- QA Testers
- Support Team

**04-technical/:** Technical deep-dive
- Architecture
- API endpoints
- Database schema
- Domain models
- External integrations

**05-configuration/:** Setup & config
- Installation
- Configuration
- Environment setup
- Integration setup

**06-examples/:** Practical examples
- Common use cases
- Step-by-step tutorials
- Best practices

---

## 4. Dynamic Knowledge Tree (Database)

### 4.1. Extended Database Schema

```csharp
// Flexible node types
public enum KnowledgeNodeType
{
    // Existing
    Software = 1,
    Module = 2,
    Issue = 3,
    
    // New types
    TechnicalDoc = 4,
    Troubleshooting = 5,
    BestPractice = 6,
    FAQ = 7,
    UserGuide = 8
}

// Rich content entry
public class KnowledgeEntry : AuditableEntity
{
    // Core fields (existing)
    public string IssueTitle { get; private set; }
    public string Description { get; private set; }
    public string RootCause { get; private set; }
    public string Fix { get; private set; }
    public string? FixVersion { get; private set; }
    public string? JiraIssueKey { get; private set; }
    public List<string> Tags { get; private set; }
    
    // Rich content fields (new)
    public string? FullContent { get; private set; }         // Markdown/HTML
    public string? CodeExamples { get; private set; }
    public List<string> ImageUrls { get; private set; }
    public string? VideoUrl { get; private set; }
    public List<string> RelatedLinks { get; private set; }
    public string? Severity { get; private set; }            // High/Medium/Low
    public string? ApplicableVersions { get; private set; }  // "1.0.0-2.5.0"
    public string? Department { get; private set; }          // Dev/QA/Support/Business
    
    // Contribution workflow (new)
    public EntryStatus Status { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }
    
    // Community features (for future Phase 2)
    public int ViewCount { get; private set; }
    public int HelpfulCount { get; private set; }
    public int NotHelpfulCount { get; private set; }
}

// Entry lifecycle
public enum EntryStatus
{
    Draft = 0,              // User đang soạn
    PendingReview = 1,      // Đã submit, chờ admin
    Published = 2,          // Live, mọi người xem được
    Rejected = 3,           // Admin từ chối
    Archived = 4            // Cũ, không còn áp dụng
}

// Version control in database
public class KnowledgeEntryVersion : AuditableEntity
{
    public Guid EntryId { get; private set; }
    public int Version { get; private set; }
    public string ChangeDescription { get; private set; }
    public string ContentSnapshot { get; private set; }     // JSON
    public Guid ModifiedBy { get; private set; }
}
```

### 4.2. Contribution Workflow (Phase 1 - Simple)

```
┌──────────┐                                    ┌──────────┐
│   User   │                                    │  Admin   │
└────┬─────┘                                    └────┬─────┘
     │                                                │
     │ 1. Create Entry (Status = Draft)               │
     ├───────────────────────────────────────────────▶│
     │                                                │
     │ 2. Submit for Review                           │
     ├───────────────────────────────────────────────▶│
     │   (Status = PendingReview)                     │
     │                                                │
     │                                    3. Review   │
     │                                    ┌───────────┤
     │                                    │ Read      │
     │                                    │ Validate  │
     │                                    │ Check     │
     │                                    └───────────┤
     │                                                │
     │                                    4a. Approve │
     │◀─────────────────────────────────────────────┤
     │   (Status = Published)                         │
     │                                                │
     │                                    4b. Reject  │
     │◀─────────────────────────────────────────────┤
     │   (Status = Rejected + ReviewNotes)            │
     │                                                │
     │ 5. View published in app                       │
     │                                                │
```

### 4.3. API Endpoints (Knowledge Module Extensions)

```http
### Entry Management
POST   /api/knowledge/entries                    # Create draft
PUT    /api/knowledge/entries/{id}               # Edit (creates version)
POST   /api/knowledge/entries/{id}/submit        # Submit for review
DELETE /api/knowledge/entries/{id}               # Delete (admin only)

### Review Workflow
GET    /api/knowledge/entries/pending            # List pending entries
GET    /api/knowledge/entries/{id}/review        # Get entry for review
POST   /api/knowledge/entries/{id}/approve       # Approve entry
POST   /api/knowledge/entries/{id}/reject        # Reject with notes

### Versioning
GET    /api/knowledge/entries/{id}/versions      # Get version history
GET    /api/knowledge/entries/{id}/versions/{v}  # Get specific version
POST   /api/knowledge/entries/{id}/restore/{v}   # Restore old version

### Search & Browse
GET    /api/knowledge/search                     # Full-text search
GET    /api/knowledge/tree                       # Get knowledge tree
GET    /api/knowledge/nodes/{nodeId}/entries     # Entries under node
```

### 4.4. UI Screens

**For All Users:**
1. **Knowledge Browser** - Tree view + search
2. **Entry Detail Page** - Full content with rich media
3. **Create/Edit Entry Form** - Rich text editor, image upload
4. **My Contributions** - Personal dashboard

**For Admins:**
5. **Pending Review Dashboard** - Queue of pending entries
6. **Review Entry Screen** - Side-by-side view + approve/reject
7. **Analytics Dashboard** - Metrics & insights

### 4.5. Permissions

```csharp
// Permission constants
public static class KnowledgePermissions
{
    // User permissions
    public const string EntryCreate = "knowledge.entry.create";
    public const string EntrySubmit = "knowledge.entry.submit";
    public const string EntryEditOwn = "knowledge.entry.edit_own";
    public const string EntryViewDraft = "knowledge.entry.view_draft";
    
    // Admin permissions
    public const string EntryReview = "knowledge.entry.review";
    public const string EntryApprove = "knowledge.entry.approve";
    public const string EntryReject = "knowledge.entry.reject";
    public const string EntryDelete = "knowledge.entry.delete";
    public const string NodeCreate = "knowledge.node.create";
    public const string NodeEdit = "knowledge.node.edit";
    public const string NodeDelete = "knowledge.node.delete";
}
```

---

## 5. AI Context Organization

### 5.1. Dual Documentation for AI

**Strategy:** Hai bộ docs riêng biệt
- **Human docs** (`docs/`) - Readable, visual, tutorial-style
- **AI docs** - Structured, concise, pattern-focused

### 5.2. AI Context Locations

**A) Repository Memory** (`/memories/repo/`) - ✅ Đã tạo
```
/memories/repo/
├── INDEX.md                      # Navigation guide
├── workhub-overview.md           # Project overview
├── architecture-patterns.md      # Code patterns & conventions
├── development-commands.md       # Commands & workflows
├── business-domain.md            # Domain logic & entities
└── frontend-architecture.md      # Frontend patterns
```

**B) GitHub Copilot Instructions** (`.github/copilot-instructions.md`)
```markdown
# WorkHub - Copilot Instructions

## Project Type
Modular Monolith, Clean Architecture, CQRS

## Code Patterns
- Result<T> not exceptions
- Private entity constructors + Factory methods
- MediatR for CQRS
- Each module: Domain/Application/Infrastructure

## When Writing Code
1. Check /memories/repo/ for patterns
2. Follow Knowledge module as reference
3. Use Result<T> for business errors
4. Write tests (Unit + Integration)
```

### 5.3. AI Query Flow

```
AI receives query: "How to implement a new command?"
    │
    ├─▶ Check /memories/repo/architecture-patterns.md
    │   └─▶ Find CQRS Command pattern
    │
    ├─▶ Check docs/modules/knowledge/04-technical/
    │   └─▶ See example Commands
    │
    └─▶ Synthesize answer with:
        - Pattern explanation
        - Code example
        - Best practices
```

### 5.4. Sync Strategy

```
Code changes
    │
    ├─▶ Developer updates technical docs
    │
    ├─▶ AI reviews changes via PR comments
    │   └─▶ Suggests memory updates if patterns change
    │
    └─▶ Quarterly review: Consolidate into templates
```

---

## 6. Templates for Other Products

### 6.1. Template Structure

```
docs/templates/
├── README.md                           # Usage guide
│
├── product-docs-structure/             # For human docs
│   ├── README.template.md
│   ├── GETTING-STARTED.template.md
│   ├── user-guides.template/
│   │   ├── for-{role}.template.md
│   │   └── ...
│   ├── technical.template/
│   │   ├── architecture.template.md
│   │   ├── api-docs.template.md
│   │   └── ...
│   ├── modules.template/
│   │   └── {module-name}/
│   │       ├── 01-overview.template.md
│   │       ├── 02-features.template/
│   │       ├── 03-user-guides.template/
│   │       ├── 04-technical.template/
│   │       ├── 05-configuration.template/
│   │       └── 06-examples.template/
│   └── business.template/
│       ├── product-vision.template.md
│       └── ...
│
└── ai-context-structure/               # For AI memory
    ├── product-overview.template.md
    ├── architecture-patterns.template.md
    ├── development-commands.template.md
    └── business-domain.template.md
```

### 6.2. Template Placeholders

```markdown
# {Product_Name} - {Brief_Description}

## Giới Thiệu
{Product_Name} là {Product_Type} giúp {Target_Users} giải quyết {Main_Problem}.

## Tech Stack
- **Backend:** {Backend_Stack}
- **Frontend:** {Frontend_Stack}
- **Database:** {Database_Stack}
- **Deployment:** {Deployment_Platform}

## Modules
{Product_Name} bao gồm {Number} modules chính:
{Module_List}

## Target Users
- {User_Role_1}: {Description}
- {User_Role_2}: {Description}
```

### 6.3. Usage Instructions

**Để tạo docs cho sản phẩm mới:**
1. Copy `docs/templates/product-docs-structure/` → `docs/`
2. Tìm và thay thế tất cả placeholders `{Variable}`
3. Xóa sections không cần thiết
4. Fill content theo guideline trong template
5. Copy AI context templates → `/memories/repo/`

---

## 7. Implementation Phases

### Phase 1: Foundation & Jira Module 🎯 (Current Priority)

**Deliverables:**
- ✅ Repository memory (completed)
- 🔲 `docs/README.md` (entry point)
- 🔲 `docs/GETTING-STARTED.md`
- 🔲 `docs/modules/jira/` (complete documentation)
- 🔲 `docs/templates/` (basic structure)
- 🔲 `docs/user-guides/` (skeleton)
- 🔲 `docs/technical/` (skeleton)

**Timeline:** 2-3 weeks

### Phase 2: Knowledge Module Enhancement

**Deliverables:**
- Extend Knowledge Module schema
- Implement contribution workflow
- Build UI screens (Browser, Create, Review)
- API endpoints for workflow
- Documentation for Knowledge module

**Timeline:** 4-6 weeks

### Phase 3: Complete Documentation Set

**Deliverables:**
- All module docs (Tasks, AI, Organization, Workspace)
- Complete user guides for all roles
- Business documentation
- Deployment guides
- Complete templates

**Timeline:** 8-12 weeks

### Phase 4: Advanced Features (Future)

**Deliverables:**
- Community voting system
- Advanced analytics
- Multi-language support
- Integration với external docs platforms

**Timeline:** TBD

---

## 8. Success Metrics

### Documentation Usage
- **Target:** 80% team members access docs weekly
- **Measure:** Page views, search queries

### Knowledge Contributions
- **Target:** 10+ entries per month
- **Measure:** Submission rate, approval rate

### Onboarding Time
- **Target:** Reduce from 2 months to 2 weeks
- **Measure:** Time to first meaningful contribution

### Support Efficiency
- **Target:** 50% reduction in repeat问 questions
- **Measure:** Support ticket volume

### AI Assistance Quality
- **Target:** AI gives relevant answers 90% of time
- **Measure:** Developer feedback surveys

---

## 9. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Documentation becomes outdated | High | Automated reminders on code changes, quarterly reviews |
| Too complex for non-tech users | Medium | Separate user guides, video tutorials |
| Low user contribution rate | Medium | Gamification, recognition system (Phase 4) |
| AI context not comprehensive | High | Regular AI effectiveness reviews, feedback loop |
| Template not flexible enough | Medium | Iterative improvement, user feedback |

---

## 10. Dependencies

### Technical
- PostgreSQL database (existing)
- Elasticsearch for search (existing)
- Rich text editor (need to choose: TipTap, Quill, ProseMirror)
- Image upload service (need to implement or use S3)

### Resources
- Solo developer for Phase 1
- Need UI/UX input for Knowledge Browser
- Need business input for user guides

### External
- Jira API access (existing)
- GitHub/GitLab for docs hosting
- CI/CD for docs deployment

---

## 11. Future Enhancements

### Phase 4 & Beyond
- **Multi-language support** - English translation
- **Docs as code** - Generate docs from code comments
- **Integration** - Confluence, Notion, SharePoint sync
- **AI chat** - Chat với knowledge base
- **Analytics** - Advanced usage analytics, ML recommendations
- **Mobile app** - Mobile-friendly knowledge browser
- **Offline mode** - Progressive Web App với offline access

---

## 12. Appendix

### A. File Naming Conventions
- Markdown files: `kebab-case.md`
- Folders: `kebab-case/`
- Templates: `name.template.md`

### B. Markdown Style Guide
- Headers: `#` đến `####` (không dùng `#####`, `######`)
- Code blocks: Luôn specify language
- Links: Relative paths trong repo
- Images: Store trong `docs/images/`, ref với relative path

### C. Review Checklist
- [ ] Technical accuracy
- [ ] Grammar & spelling (Vietnamese)
- [ ] Links work correctly
- [ ] Code examples có thể chạy được
- [ ] Screenshots up-to-date
- [ ] Cross-references đúng

---

## Approval

**Design approved by:** User  
**Date:** 2026-03-22  
**Next step:** Create implementation plan

