# Documentation System Phase 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Thiết lập cấu trúc documentation system và hoàn thiện tài liệu cho Jir module

**Architecture:** Hybrid approach với static Markdown docs trong Git + templates tái sử dụng. Phase 1 focus vào foundation và Jira module làm ví dụ mẫu.

**Tech Stack:** Markdown, Git, Visual Studio Code

**Related Spec:** [docs/superpowers/specs/2026-03-22-documentation-system-design.md](../specs/2026-03-22-documentation-system-design.md)

---

## File Structure Overview

```
docs/
├── README.md                          # CREATE - Entry point
├── GETTING-STARTED.md                 # CREATE - Quick start
├── templates/                         # CREATE - Templates
│   ├── README.md
│   ├── product-docs-structure/
│   │   └── (various template files)
│   └── ai-context-structure/
│       └── (AI template files)
├── user-guides/                       # CREATE - Skeleton
│   ├── README.md
│   └── (placeholder files)
├── technical/                         # CREATE - Skeleton
│   ├── README.md
│   └── (placeholder files)
└── modules/
    └── jira/                         # CREATE - Complete
        ├── README.md
        ├── 01-overview.md
        ├── 02-features/
        ├── 03-user-guides/
        ├── 04-technical/
        ├── 05-configuration/
        └── 06-examples/
```

---

## Task 1: Create Documentation Root Structure

**Files:**
- Create: `docs/README.md`
- Create: `docs/GETTING-STARTED.md`
- Create: `docs/.gitkeep` (for empty dirs)

### Step 1.1: Create docs/README.md (Entry point)

- [ ] **Create the main documentation index**

```markdown
# WorkHub Documentation

Chào mừng đến với tài liệu WorkHub - nền tảng quản lý công việc và tri thức cho đội ngũ phát triển phần mềm.

## 📚 Mục Lục

### Bắt Đầu Nhanh
- [Hướng Dẫn Bắt Đầu](GETTING-STARTED.md) - Setup và chạy WorkHub lần đầu
- [Kiến Trúc Tổng Quan](ARCHITECTURE.md) - Hiểu thiết kế hệ thống

### Hướng Dẫn Theo Vai Trò
- [Dành cho Product Managers](user-guides/for-product-managers.md)
- [Dành cho Business Analysts](user-guides/for-business-analysts.md)
- [Dành cho QA/Testers](user-guides/for-qa-testers.md)
- [Dành cho Support Team](user-guides/for-support-team.md)
- [Dành cho End Users](user-guides/for-end-users.md)

### Tài Liệu Kỹ Thuật
- [Development Setup](technical/development-setup.md) - Cài đặt môi trường dev
- [Coding Standards](technical/coding-standards.md) - Quy chuẩn code
- [API Documentation](technical/api-documentation.md) - REST API reference
- [Database Schema](technical/database-schema.md) - Thiết kế database
- [Deployment Guide](technical/deployment-guide.md) - Deploy production
- [Troubleshooting](technical/troubleshooting.md) - Xử lý sự cố

### Tài Liệu Modules
- [🎯 Jira Module](modules/jira/README.md) - Tích hợp và tự động hóa Jira
- [Knowledge Module](modules/knowledge/README.md) - Hệ thống tri thức
- [Tasks Module](modules/tasks/README.md) - Quản lý công việc
- [AI Module](modules/ai/README.md) - Trợ lý AI
- [Organization Module](modules/organization/README.md) - Quản lý tổ chức
- [Workspace Module](modules/workspace/README.md) - Không gian làm việc

### Business Documentation
- [Product Vision](business/product-vision.md) - Tầm nhìn sản phẩm
- [Feature Roadmap](business/feature-roadmap.md) - Lộ trình phát triển
- [Business Value](business/business-value.md) - Giá trị business

### Templates
- [Templates cho Sản Phẩm Khác](templates/README.md) - Tái sử dụng structure này

## 🎯 Ưu Tiên Đọc

**Nếu bạn là Developer mới:**
1. [Getting Started](GETTING-STARTED.md)
2. [Development Setup](technical/development-setup.md)
3. [Architecture](ARCHITECTURE.md)
4. [Jira Module](modules/jira/README.md) - Ví dụ module hoàn chỉnh

**Nếu bạn là Product Manager:**
1. [Product Vision](business/product-vision.md)
2. [User Guide for PM](user-guides/for-product-managers.md)
3. [Jira Module Overview](modules/jira/01-overview.md)

**Nếu bạn là QA/Tester:**
1. [User Guide for QA](user-guides/for-qa-testers.md)
2. [Jira Module User Guide](modules/jira/03-user-guides/for-qa-testers.md)

**Nếu bạn là Support Team:**
1. [User Guide for Support](user-guides/for-support-team.md)
2. [Troubleshooting](technical/troubleshooting.md)
3. [Knowledge Module](modules/knowledge/README.md) - Tìm giải pháp

## 🤖 Dành Cho AI

AI assistants nên tham khảo:
- Repository memory: `/memories/repo/INDEX.md`
- Copilot instructions: `.github/copilot-instructions.md`
- Technical docs trong `docs/technical/`

## 📝 Đóng Góp

Để đóng góp vào tài liệu này:
1. Fork repository
2. Tạo branch mới: `docs/ten-cai-tien`
3. Commit changes
4. Tạo Pull Request

Hoặc sử dụng Knowledge Module trong app để đóng góp tri thức!

## 📞 Hỗ Trợ

- GitHub Issues: [WorkHub Issues](https://github.com/your-org/workhub/issues)
- Email: support@workhub.com
- Slack: #workhub-support

---

**Cập nhật lần cuối:** 2026-03-22
```

- [ ] **Commit the main README**

```bash
git add docs/README.md
git commit -m "docs: add main documentation index"
```

### Step 1.2: Create docs/GETTING-STARTED.md

- [ ] **Create getting started guide**

```markdown
# Hướng Dẫn Bắt Đầu Nhanh

Guide này giúp bạn setup và chạy WorkHub trong 15 phút.

## Yêu Cầu Hệ Thống

### Backend
- .NET 10 SDK
- PostgreSQL 16+
- Redis 7+
- Elasticsearch 8+

### Frontend  
- Node.js 20+
- npm hoặc yarn

### Tools
- Git
- Docker (recommended cho services)
- Visual Studio Code (recommended)

## Bước 1: Clone Repository

```bash
git clone https://github.com/your-org/workhub.git
cd workhub
```

## Bước 2: Setup Database & Services

### Option A: Docker Compose (Recommended)

```bash
cd BackEnd/WorkHub
docker-compose up -d
```

Services sẽ chạy tại:
- PostgreSQL: `localhost:5432`
- Redis: `localhost:6379`
- Elasticsearch: `localhost:9200`

### Option B: Manual Setup

Xem chi tiết: [Development Setup](technical/development-setup.md)

## Bước 3: Configure Backend

```bash
cd BackEnd/WorkHub/src/WorkHub.API
```

Tạo `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=WorkHub;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "WorkHub",
    "Audience": "WorkHub.Client"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  },
  "Jira": {
    "BaseUrl": "https://your-domain.atlassian.net",
    "Email": "your-email@company.com",
    "ApiToken": "your-api-token"
  }
}
```

## Bước 4: Run Migrations

```bash
# Trong folder WorkHub.API
dotnet ef database update
```

## Bước 5: Start Backend

```bash
dotnet run
```

API sẽ chạy tại: `https://localhost:7001`

## Bước 6: Setup Frontend

```bash
cd FrontEnd
npm install
npm run dev
```

Frontend sẽ chạy tại: `http://localhost:5173`

## Bước 7: Login

Mở browser và truy cập: `http://localhost:5173`

**Default credentials:**
- Email: `admin@workhub.com`
- Password: `Admin@123`

## Bước 8: Explore Features

1. **Jira Module:** Tạo issue nhanh với templates
2. **Knowledge Module:** Browse cây tri thức
3. **Tasks Module:** Quản lý công việc

## Tiếp Theo

- [Architecture Overview](ARCHITECTURE.md) - Hiểu kiến trúc
- [Development Setup (Full)](technical/development-setup.md) - Setup chi tiết
- [Jira Module Guide](modules/jira/README.md) - Khám phá Jira module

## Troubleshooting

### Backend không chạy được

**Error: Database connection failed**
- Check PostgreSQL có chạy: `docker ps` hoặc `Get-Service postgresql*`
- Check connection string trong `appsettings.Development.json`

**Error: Port already in use**
- Change port trong `Properties/launchSettings.json`

### Frontend không build được

**Error: Module not found**
```bash
rm -rf node_modules package-lock.json
npm install
```

**Error: CORS**
- Check `Program.cs` có config CORS cho `http://localhost:5173`

## Cần Hỗ Trợ?

- [Troubleshooting Guide](technical/troubleshooting.md)
- [GitHub Issues](https://github.com/your-org/workhub/issues)
- Slack: #workhub-support
```

- [ ] **Commit getting started guide**

```bash
git add docs/GETTING-STARTED.md
git commit -m "docs: add getting started guide"
```

---

## Task 2: Create Template Structure

**Files:**
- Create: `docs/templates/README.md`
- Create: `docs/templates/product-docs-structure/README.template.md`
- Create: `docs/templates/ai-context-structure/product-overview.template.md`

### Step 2.1: Create templates README

- [ ] **Create templates/README.md**

```markdown
# Documentation Templates

Templates này giúp tạo documentation structure cho các sản phẩm khác trong công ty.

## Cách Sử Dụng

### 1. Copy Template Structure

```bash
# Copy toàn bộ structure
cp -r docs/templates/product-docs-structure/ ../YourProduct/docs/

# Hoặc cherry-pick từng phần
cp docs/templates/product-docs-structure/01-overview.template.md ../YourProduct/docs/overview.md
```

### 2. Replace Placeholders

Tìm và thay thế các placeholders:
- `{Product_Name}` → Tên sản phẩm của bạn
- `{Product_Type}` → Loại sản phẩm (Web app, Mobile app, API...)
- `{Target_Users}` → Người dùng mục tiêu
- `{Main_Problem}` → Vấn đề chính giải quyết
- `{Backend_Stack}` → Tech stack backend
- `{Frontend_Stack}` → Tech stack frontend
- ... (xem full list trong mỗi template)

### 3. Customize Content

- Xóa sections không cần thiết
- Thêm sections đặc thù cho sản phẩm
- Fill content theo guideline trong template

## Templates Có Sẵn

### A. Product Documentation Structure

Cho người đọc (developers, users, business):

```
product-docs-structure/
├── README.template.md
├── 01-overview.template.md
├── 02-getting-started.template.md
├── 03-user-guides.template/
│   ├── README.template.md
│   └── for-{role}.template.md
├── 04-technical.template/
│   ├── architecture.template.md
│   ├── api-docs.template.md
│   └── ...
├── 05-modules.template/
│   └── {module-name}/
│       ├── 01-overview.template.md
│       ├── 02-features.template/
│       ├── 03-user-guides.template/
│       ├── 04-technical.template/
│       ├── 05-configuration.template/
│       └── 06-examples.template/
└── 06-business.template/
```

### B. AI Context Structure

Cho AI assistants:

```
ai-context-structure/
├── product-overview.template.md
├── architecture-patterns.template.md
├── development-commands.template.md
└── business-domain.template.md
```

## Examples

Xem WorkHub documentation làm ví dụ:
- [WorkHub docs/](../) - Human-readable docs
- [WorkHub /memories/repo/](../../..) - AI context

## Best Practices

1. **Viết cho audience cụ thể:** Dev, QA, PM cần info khác nhau
2. **DRY:** Tránh duplicate, dùng links
3. **Keep updated:** Review docs mỗi sprint/release
4. **Examples matter:** Code examples thực tế > lý thuyết
5. **Search-friendly:** Dùng keywords, headers rõ ràng

## Customization

Nếu template không fit:
1. Fork và modify theo nhu cầu
2. Share improvements qua PR
3. Document why you deviated (trong product README)

## Support

Questions về templates:
- Check WorkHub docs làm reference
- Open issue với label `docs-templates`
- Slack: #documentation-guild
```

- [ ] **Commit templates README**

```bash
git add docs/templates/README.md
git commit -m "docs: add templates README with usage guide"
```

### Step 2.2: Create product docs template

- [ ] **Create product-docs-structure/README.template.md**

```markdown
# {Product_Name} Documentation

{Brief 1-2 sentence description của sản phẩm}

## About {Product_Name}

{Product_Name} là {Product_Type} giúp {Target_Users} giải quyết {Main_Problem}.

## Quick Links

- [Getting Started](02-getting-started.template.md)
- [Architecture](04-technical/architecture.template.md)
- [API Documentation](04-technical/api-docs.template.md)

## Documentation Structure

- **`01-overview.md`** - Tổng quan sản phẩm
- **`02-getting-started.md`** - Setup và chạy lần đầu
- **`03-user-guides/`** - Hướng dẫn theo vai trò
- **`04-technical/`** - Technical documentation
- **`05-modules/`** - Documentation từng module
- **`06-business/`** - Business documentation

## Tech Stack

- **Backend:** {Backend_Stack}
- **Frontend:** {Frontend_Stack}
- **Database:** {Database_Stack}
- **Deployment:** {Deployment_Platform}

## Target Audience

- **Developers:** {Dev_Description}
- **QA/Testers:** {QA_Description}
- **Product Managers:** {PM_Description}
- **Support Team:** {Support_Description}
- **End Users:** {User_Description}

## Contributing

[Contribution guidelines]

---

**Template Version:** 1.0  
**Based on:** WorkHub Documentation Structure  
**Last Updated:** {Date}
```

- [ ] **Commit product docs template**

```bash
git add docs/templates/product-docs-structure/
git commit -m "docs: add product documentation template structure"
```

### Step 2.3: Create AI context template

- [ ] **Create ai-context-structure/product-overview.template.md**

```markdown
# {Product_Name} AI Context - Overview

## Project Identity

**Name:** {Product_Name}  
**Type:** {Product_Type}  
**Stage:** {Development_Stage}  
**Team Size:** {Team_Size}  
**Primary Language:** {Programming_Language}

## Architecture Type

{Architecture_Description}
- Example: "Modular Monolith following Clean Architecture"
- Example: "Microservices với event-driven architecture"

## Tech Stack

### Backend
- **Framework:** {Backend_Framework}
- **Language:** {Backend_Language}  
- **Database:** {Database_System}
- **Cache:** {Cache_System}
- **Message Queue:** {MQ_System} (if applicable)

### Frontend
- **Framework:** {Frontend_Framework}
- **Language:** {Frontend_Language}
- **State Management:** {State_Mgmt}
- **UI Library:** {UI_Library}

## Project Structure

```
{Root_Folder}/
├── {Backend_Folder}/
│   ├── src/
│   │   ├── {Core_Projects}
│   │   └── {Module_Structure}
│   └── tests/
└── {Frontend_Folder}/
    ├── src/
    │   ├── components/
    │   ├── views/
    │   └── ...
    └── tests/
```

## Key Modules

### Module 1: {Module_Name}
**Purpose:** {Module_Purpose}  
**Owns:** {Module_Entities}  
**Does NOT own:** {Module_Boundaries}

### Module 2: {Module_Name}
...

## Common Patterns

### {Pattern_Name_1}
**When:** {When_To_Use}  
**Example:**
```{language}
{Code_Example}
```

### {Pattern_Name_2}
...

## Module Boundaries

| Module | Owns | Does NOT Own |
|--------|------|--------------|
| {Module} | {entities} | {boundaries} |

## Configuration

- **Database:** {Connection_Info}
- **Cache:** {Cache_Config}
- **External APIs:** {API_List}

## Development Commands

See: `/memories/repo/development-commands.md`

## Key Decisions (ADRs)

1. **{Decision_Name}:** {Brief_Reason}
2. **{Decision_Name}:** {Brief_Reason}

---

**Template Version:** 1.0  
**For:** AI assistants (GitHub Copilot, Claude, etc.)
```

- [ ] **Commit AI context templates**

```bash
git add docs/templates/ai-context-structure/
git commit -m "docs: add AI context template structure"
```

---

## Task 3: Create User Guides Structure (Skeleton)

**Files:**
- Create: `docs/user-guides/README.md`
- Create: `docs/user-guides/for-product-managers.md`
- Create: `docs/user-guides/for-qa-testers.md`
- Create: `docs/user-guides/for-support-team.md`

### Step 3.1: Create user-guides/README.md

- [ ] **Create user guides index**

```markdown
# Hướng Dẫn Sử Dụng WorkHub

Tài liệu này hướng dẫn cách sử dụng WorkHub cho từng vai trò trong tổ chức.

## Chọn Hướng Dẫn Theo Vai Trò

### 👔 Business Roles

- [**Product Managers**](for-product-managers.md) - Quản lý roadmap, templates, priorities
- [**Business Analysts**](for-business-analysts.md) - Phân tích requirements, tạo specs
- [**Marketing/Sales**](for-marketing-sales.md) - Product positioning, demos

### 🧪 Technical Roles

- [**Developers**](for-developers.md) - Code, create issues, log work
- [**QA/Testers**](for-qa-testers.md) - Test cases, bug reports, verification
- [**DevOps/Infrastructure**](for-devops.md) - Deploy, monitor, maintain

### 🤝 Support Roles

- [**Customer Support**](for-support-team.md) - Troubleshoot, find solutions, escalate
- [**End Users**](for-end-users.md) - Daily usage, basic features

## Common Workflows

### Creating Issues
- [Quick Issue Creation](../modules/jira/03-user-guides/quick-issue-workflow.md)
- [Using Templates](../modules/jira/03-user-guides/using-templates.md)

### Managing Tasks
- [Task Board](../modules/tasks/03-user-guides/task-board.md)
- [Work Sessions](../modules/tasks/03-user-guides/work-sessions.md)

### Searching Knowledge
- [Knowledge Browser](../modules/knowledge/03-user-guides/knowledge-browser.md)
- [Contributing Knowledge](../modules/knowledge/03-user-guides/contributing.md)

## Getting Help

- [FAQ](faq.md)
- [Troubleshooting](../technical/troubleshooting.md)
- [Support Channels](support.md)
```

- [ ] **Commit user guides index**

```bash
git add docs/user-guides/README.md
git commit -m "docs: add user guides index"
```

### Step 3.2: Create placeholder user guides

- [ ] **Create for-product-managers.md**

```markdown
# Hướng Dẫn Cho Product Managers

## Tổng Quan

Là Product Manager, bạn sử dụng WorkHub để:
- Quản lý issue templates
- Theo dõi progress của team
- Ưu tiên công việc
- Tương tác với Jira

## Quick Start

[Content sẽ được bổ sung trong Phase 2]

## Key Features for PMs

### Issue Templates
- Tạo và quản lý templates
- Customize fields
- Set default values

### Dashboard
- Overview metrics
- Team velocity
- Issue distribution

### Jira Integration
- Sync status
- Bulk operations
- Reports export

## Workflows

[Chi tiết workflows sẽ được bổ sung]

## Best Practices

[Best practices sẽ được bổ sung]

---

**Status:** 🚧 Under Development - Full content in Phase 2
```

- [ ] **Create for-qa-testers.md**

```markdown
# Hướng Dẫn Cho QA/Testers

## Tổng Quan

Là QA/Tester, bạn sử dụng WorkHub để:
- Tạo bug reports nhanh chóng
- Tìm kiếm issues tương tự
- Track bug status
- Document test cases

## Quick Start

[Content sẽ được bổ sung trong Phase 2]

## Key Features for QA

### Bug Reporting
- Quick bug creation với template
- Auto-fill common fields
- Attach screenshots/logs

### Knowledge Base
- Search for known issues
- Find root causes & fixes
- Contribute solutions

### Test Management
- Link tests to issues
- Track test coverage
- Report test results

## Workflows

[Chi tiết workflows sẽ được bổ sung]

## Best Practices

[Best practices sẽ được bổ sung]

---

**Status:** 🚧 Under Development - Full content in Phase 2
```

- [ ] **Create for-support-team.md**

```markdown
# Hướng Dẫn Cho Support Team

## Tổng Quan

Là Support Team member, bạn sử dụng WorkHub để:
- Tìm giải pháp cho customer issues nhanh chóng
- Escalate vấn đề phức tạp lên dev team
- Track support tickets
- Contribute troubleshooting knowledge

## Quick Start

[Content sẽ được bổ sung trong Phase 2]

## Key Features for Support

### Knowledge Search
- Full-text search across all entries
- Filter by product/module/severity
- Find root causes & fixes quickly

### Issue Creation
- Create issues for dev team
- Link to customer tickets
- Set priority & severity

### Contribution
- Add new solutions to knowledge base
- Update existing entries
- Share workarounds

## Workflows

### Typical Support Flow
1. Customer reports issue
2. Search knowledge base
3. If found: Apply solution
4. If not found: Escalate to dev
5. After resolution: Document in knowledge base

[Chi tiết workflows sẽ được bổ sung]

## Best Practices

[Best practices sẽ được bổ sung]

---

**Status:** 🚧 Under Development - Full content in Phase 2
```

- [ ] **Commit user guide placeholders**

```bash
git add docs/user-guides/*.md
git commit -m "docs: add user guide placeholders (Phase 2 content TBD)"
```

---

## Task 4: Create Technical Documentation Structure (Skeleton)

**Files:**
- Create: `docs/technical/README.md`
- Create: `docs/technical/development-setup.md`
- Create: `docs/technical/coding-standards.md`

### Step 4.1: Create technical/README.md

- [ ] **Create technical docs index**

```markdown
# Technical Documentation

Tài liệu kỹ thuật dành cho developers và DevOps engineers.

## Getting Started

- [Development Setup](development-setup.md) - Chi tiết setup môi trường
- [Coding Standards](coding-standards.md) - Quy chuẩn code
- [Architecture Overview](../ARCHITECTURE.md) - ADRs và design decisions

## Development

- [Local Development](development-setup.md#local-development)
- [Running Tests](testing.md)
- [Debugging](debugging.md)
- [Database Migrations](database-migrations.md)

## Architecture & Design

- [Clean Architecture Patterns](../ARCHITECTURE.md)
- [Module Structure](module-structure.md)
- [CQRS with MediatR](cqrs-patterns.md)
- [Domain Events](domain-events.md)

## API Documentation

- [REST API Overview](api-documentation.md)
- [Authentication & Authorization](auth-api.md)
- [Error Handling](api-errors.md)
- [Rate Limiting](rate-limiting.md)

## Database

- [Schema Overview](database-schema.md)
- [Migrations Guide](database-migrations.md)
- [Seeding Data](database-seeding.md)

## Deployment

- [Deployment Guide](deployment-guide.md)
- [Environment Configuration](environment-config.md)
- [Docker Setup](docker-setup.md)
- [CI/CD Pipeline](cicd.md)

## Operations

- [Monitoring & Logging](monitoring.md)
- [Performance Tuning](performance.md)
- [Troubleshooting](troubleshooting.md)
- [Backup & Recovery](backup-recovery.md)

## Contributing

- [Git Workflow](git-workflow.md)
- [Code Review Process](code-review.md)
- [Pull Request Template](pr-template.md)

---

**Note:** Some sections marked 🚧 are under development
```

- [ ] **Commit technical docs index**

```bash
git add docs/technical/README.md
git commit -m "docs: add technical documentation index"
```

### Step 4.2: Create development-setup.md (Detailed)

- [ ] **Create comprehensive development setup guide**

```markdown
# Development Setup - Hướng Dẫn Chi Tiết

Guide này cung cấp hướng dẫn đầy đủ để setup môi trường development.

## Prerequisites

### Required Software

| Software | Version | Download |
|----------|---------|----------|
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| Node.js | 20.0+ | https://nodejs.org/ |
| PostgreSQL | 16.0+ | https://www.postgresql.org/download/ |
| Redis | 7.0+ | https://redis.io/download |
| Elasticsearch | 8.11+ | https://www.elastic.co/downloads/elasticsearch |
| Git | Latest | https://git-scm.com/ |

### Recommended Tools

- **IDE:** Visual Studio Code hoặc Visual Studio 2022
- **Database Client:** pgAdmin, DBeaver
- **API Client:** Postman, Insomnia, hoặc VS Code REST Client
- **Docker Desktop:** Để chạy services

## Setup Instructions

### 1. Clone Repository

```bash
git clone https://github.com/your-org/workhub.git
cd workhub
```

### 2. Backend Setup

#### 2.1. Install Dependencies

```bash
cd BackEnd/WorkHub
dotnet restore
```

#### 2.2. Setup Services

**Option A: Docker Compose (Recommended)**

```bash
docker-compose up -d
```

Verify services:
```bash
docker ps
# Should see: postgres, redis, elasticsearch
```

**Option B: Local Installation**

**PostgreSQL:**
```bash
# Windows (PowerShell as Admin)
choco install postgresql

# Hoặc download installer

# Create database
psql -U postgres
CREATE DATABASE WorkHub;
\q
```

**Redis:**
```bash
# Windows
choco install redis-64

# Start service
redis-server
```

**Elasticsearch:**
```bash
# Download và extract
# Start
.\bin\elasticsearch.bat
```

#### 2.3. Configuration

Create `src/WorkHub.API/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=WorkHub;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long-for-security",
    "Issuer": "WorkHub",
    "Audience": "WorkHub.Client",
    "ExpiresInMinutes": 60
  },
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jira": {
    "BaseUrl": "https://your-domain.atlassian.net",
    "Email": "your-email@company.com",
    "ApiToken": "your-jira-api-token"
  }
}
```

**Get Jira API Token:**
1. Go to https://id.atlassian.com/manage-profile/security/api-tokens
2. Create API token
3. Copy và paste vào config

#### 2.4. Run Migrations

```bash
cd src/WorkHub.API
dotnet ef database update
```

#### 2.5. Seed Data (Optional)

```bash
dotnet run --seed
```

Tạo default admin user:
- Email: `admin@workhub.com`
- Password: `Admin@123`

#### 2.6. Start Backend

```bash
dotnet run
```

API available at: `https://localhost:7001`

Test API:
```bash
curl https://localhost:7001/health
```

### 3. Frontend Setup

#### 3.1. Install Dependencies

```bash
cd FrontEnd
npm install
```

#### 3.2. Configuration

Create `.env.development.local`:

```env
VITE_API_URL=https://localhost:7001/api
VITE_APP_NAME=WorkHub
```

#### 3.3. Start Frontend

```bash
npm run dev
```

Frontend available at: `http://localhost:5173`

### 4. Verify Installation

#### 4.1. Backend Health Check

```bash
curl https://localhost:7001/health
```

Expected: `{"status": "Healthy"}`

#### 4.2. Database Connection

```bash
curl https://localhost:7001/health/db
```

Expected: `{"status": "Healthy", "database": "Connected"}`

#### 4.3. Login to Frontend

1. Open `http://localhost:5173`
2. Login với:
   - Email: `admin@workhub.com`
   - Password: `Admin@123`

## Development Workflow

### Running Tests

**Backend:**
```bash
# Unit tests
dotnet test tests/WorkHub.UnitTests/

# Integration tests (need services running)
dotnet test tests/WorkHub.IntegrationTests/

# All tests
dotnet test
```

**Frontend:**
```bash
cd FrontEnd
npm run test
npm run test:coverage
```

### Code Formatting

**Backend:**
```bash
dotnet format
```

**Frontend:**
```bash
npm run lint
npm run format
```

### Database Migrations

**Create new migration:**
```bash
cd src/Modules/Knowledge/WorkHub.Modules.Knowledge.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../../../WorkHub.API/
```

**Apply migrations:**
```bash
dotnet ef database update --startup-project ../../../WorkHub.API/
```

**Rollback:**
```bash
dotnet ef database update PreviousMigrationName --startup-project ../../../WorkHub.API/
```

## Troubleshooting

### Backend Issues

**Error: "Connection refused" to PostgreSQL**
- Check PostgreSQL service: `Get-Service postgresql*`
- Check connection string in appsettings
- Verify port 5432 not blocked

**Error: "Unable to resolve service for IMediator"**
- Run `dotnet clean && dotnet build`
- Check DependencyInjection.cs in each module

**Error: "JWT token invalid"**
- Check Jwt.Key length >= 32 characters
- Regenerate token

### Frontend Issues

**Error: "CORS policy blocked"**
- Check Backend `Program.cs` has correct origin:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

**Error: "Module not found"**
```bash
rm -rf node_modules package-lock.json
npm install
```

### Docker Issues

**Services not starting**
```bash
docker-compose down
docker-compose up -d --force-recreate
```

**Port already in use**
```bash
# Find process using port (Windows)
netstat -ano | findstr :5432
# Kill process
taskkill /PID <PID> /F
```

## IDE Setup

### Visual Studio Code

**Recommended Extensions:**
- C# Dev Kit
- Vue - Official
- Tailwind CSS IntelliSense
- REST Client
- GitLens

**Settings:**
```json
{
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.organizeImports": true
  }
}
```

### Visual Studio 2022

**Extensions:**
- ReSharper (optional)
- Vue.js Pack

## Next Steps

- [Coding Standards](coding-standards.md)
- [Architecture Overview](../ARCHITECTURE.md)
- [Module Development Guide](module-development.md)

---

**Last Updated:** 2026-03-22
```

- [ ] **Commit development setup**

```bash
git add docs/technical/development-setup.md
git commit -m "docs: add comprehensive development setup guide"
```

### Step 4.3: Create coding-standards.md

- [ ] **Create coding standards document**

```markdown
# Coding Standards

Quy chuẩn code cho WorkHub project.

## General Principles

- **SOLID principles**
- **DRY** (Don't Repeat Yourself)
- **YAGNI** (You Aren't Gonna Need It)
- **KISS** (Keep It Simple, Stupid)

## C# Backend Standards

### Naming Conventions

```csharp
// Classes, Interfaces, Methods: PascalCase
public class KnowledgeEntry { }
public interface IKnowledgeRepository { }
public void CreateEntry() { }

// Private fields: _camelCase
private readonly IMediator _mediator;

// Parameters, local variables: camelCase
public void Method(string parameterName)
{
    var localVariable = "";
}

// Constants: PascalCase
public const string DefaultValue = "value";
```

### Entity Pattern

```csharp
public class KnowledgeEntry : AuditableEntity
{
    // Properties - private set
    public string Title { get; private set; } = default!;
    
    // Collections - private backing field
    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    // Constructor MUST be private
    private KnowledgeEntry() { }
    
    // Factory method
    public static KnowledgeEntry Create(...)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return new KnowledgeEntry { Title = title, ... };
    }
    
    // Domain methods
    public void UpdateTitle(string newTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newTitle);
        Title = newTitle;
        SetUpdated(updatedBy);
    }
}
```

### CQRS Pattern

**Command:**
```csharp
public record CreateEntryCommand(
    string Title,
    string Content,
    Guid CreatedBy
) : ICommand<Result<Guid>>;
```

**Validator:**
```csharp
public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
    }
}
```

**Handler:**
```csharp
internal sealed class CreateEntryCommandHandler 
    : ICommandHandler<CreateEntryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateEntryCommand cmd, 
        CancellationToken ct)
    {
        // Validation
        if (validationFails)
            return Result<Guid>.Failure("Error message");
        
        // Business logic
        var entry = KnowledgeEntry.Create(...);
        await _repository.AddAsync(entry, ct);
        
        return Result<Guid>.Success(entry.Id);
    }
}
```

### Result Pattern

```csharp
// ✅ DO: Return Result<T>
public async Task<Result<Guid>> Handle(...)
{
    if (node is null)
        return Result<Guid>.Failure("Node not found");
    
    return Result<Guid>.Success(entityId);
}

// ❌ DON'T: Throw exceptions for business failures
public async Task<Guid> Handle(...)
{
    if (node is null)
        throw new NotFoundException("Node", id); // Bad!
    
    return entityId;
}
```

### File Organization

```
Module/
├── Domain/
│   ├── Entities/          # One entity per file
│   ├── Enums/
│   ├── Events/
│   └── Repositories/      # Interfaces only
├── Application/
│   ├── Abstractions/      # Interfaces
│   ├── Commands/
│   │   └── CreateEntry/
│   │       ├── CreateEntryCommand.cs
│   │       ├── CreateEntryCommandValidator.cs
│   │       └── CreateEntryCommandHandler.cs
│   └── Queries/
│       └── SearchEntries/
│           ├── SearchEntriesQuery.cs
│           ├── SearchEntriesQueryHandler.cs
│           └── SearchEntriesResult.cs
└── Infrastructure/
    ├── DependencyInjection.cs
    ├── Persistence/
    │   ├── {Module}DbContext.cs
    │   ├── Configurations/
    │   └── Migrations/
    └── Repositories/
```

## TypeScript Frontend Standards

### Naming Conventions

```typescript
// Interfaces/Types: PascalCase
interface User { }
type UserRole = 'admin' | 'user';

// Variables, functions: camelCase
const userName = 'John';
function getUserName() { }

// Constants: UPPER_SNAKE_CASE
const API_BASE_URL = 'https://api.example.com';

// Components: PascalCase
const UserProfile = defineComponent({ });
```

### Vue 3 Composition API

```vue
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Props
interface Props {
  title: string
  count?: number
}

const props = withDefaults(defineProps<Props>(), {
  count: 0
})

// Emits
interface Emits {
  (e: 'update', value: number): void
  (e: 'close'): void
}

const emit = defineEmits<Emits>()

// Reactive state
const isLoading = ref(false)
const items = ref<Item[]>([])

// Computed
const itemCount = computed(() => items.value.length)

// Methods
const handleClick = () => {
  emit('update', props.count + 1)
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="container">
    <h1>{{ props.title }}</h1>
    <p>Count: {{ itemCount }}</p>
  </div>
</template>

<style scoped>
.container {
  padding: 1rem;
}
</style>
```

### API Service Pattern

```typescript
// services/api.ts
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api

// services/knowledge.ts
export interface CreateEntryRequest {
  title: string
  content: string
}

export const knowledgeService = {
  async createEntry(data: CreateEntryRequest) {
    return api.post('/knowledge/entries', data)
  },
  
  async getEntry(id: string) {
    return api.get(`/knowledge/entries/${id}`)
  }
}
```

## Testing Standards

### Unit Tests

```csharp
// Arrange-Act-Assert pattern
public class CreateEntryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateEntryCommand(...);
        var handler = new CreateEntryCommandHandler(...);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
    
    [Fact]
    public async Task Handle_InvalidTitle_ReturnsFailure()
    {
        // Arrange
        var command = new CreateEntryCommand("", ...); // Empty title
        var handler = new CreateEntryCommandHandler(...);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("title", result.Error.ToLower());
    }
}
```

### Integration Tests

```csharp
public class KnowledgeApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task POST_CreateEntry_ReturnsCreated()
    {
        // Arrange
        var request = new { title = "Test", content = "Content" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/knowledge/entries", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Git Commit Messages

Format: `<type>(<scope>): <subject>`

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance

**Examples:**
```
feat(jira): add quick issue creation command
fix(knowledge): resolve search indexing bug
docs(api): update authentication endpoints
refactor(tasks): simplify task state machine
test(knowledge): add entry validation tests
chore(deps): update Entity Framework to 10.0.5
```

## Code Review Checklist

- [ ] Follows naming conventions
- [ ] No duplicated code
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No hardcoded values
- [ ] Error handling present
- [ ] Logging added where appropriate
- [ ] Performance considered

## References

- [Clean Architecture](../ARCHITECTURE.md)
- [CQRS Patterns](cqrs-patterns.md)
- [Testing Guide](testing.md)

---

**Last Updated:** 2026-03-22
```

- [ ] **Commit coding standards**

```bash
git add docs/technical/coding-standards.md
git commit -m "docs: add coding standards and best practices"
```

---

## Task 5: Create Jira Module Documentation (Complete)

This is the PRIORITY task - full documentation for Jira module.

**Files:**
- Create: `docs/modules/jira/README.md`
- Create: `docs/modules/jira/01-overview.md`
- Create: Multiple files in 02-features/, 03-user-guides/, 04-technical/, 05-configuration/, 06-examples/

### Step 5.1: Create jira/README.md

- [ ] **Create Jira module main README**

```markdown
# Module Jira - Tăng Tốc Làm Việc Với Jira

Module này giúp team tương tác với Jira nhanh hơn, giảm thời gian thao tác thủ công từ 80-90%.

## 🎯 Tổng Quan Nhanh

**Vấn đề:** Tạo issue trên Jira mất 3-5 phút, phải điền nhiều field, quên log work  
**Giải pháp:** Quick create với templates, auto-fill, auto-log → chỉ còn 30 giây

## 📚 Nội Dung

### 1. [Tổng Quan](01-overview.md)
- Vấn đề cần giải quyết
- Giải pháp module cung cấp
- So sánh Before/After
- Key benefits

### 2. Tính Năng Chi Tiết
- [Tạo Issue Nhanh](02-features/quick-issue-creation.md)
- [Issue Templates](02-features/issue-templates.md)
- [Tự Động Log Work](02-features/auto-log-work.md)
- [Đồng Bộ Với Jira](02-features/sync-jira.md)
- [Thao Tác Hàng Loạt](02-features/bulk-operations.md)

### 3. Hướng Dẫn Theo Vai Trò
- [Dành cho Developers](03-user-guides/for-developers.md)
- [Dành cho Project Managers](03-user-guides/for-project-managers.md)
- [Dành cho QA/Testers](03-user-guides/for-qa-testers.md)
- [Dành cho Support Team](03-user-guides/for-support-team.md)

### 4. Tài Liệu Kỹ Thuật
- [Kiến Trúc Module](04-technical/architecture.md)
- [API Endpoints](04-technical/api-endpoints.md)
- [Database Schema](04-technical/database-schema.md)
- [Domain Models](04-technical/domain-models.md)
- [Tích Hợp Jira API](04-technical/jira-api-integration.md)

### 5. Configuration
- [Kết Nối Jira Account](05-configuration/jira-connection.md)
- [Tạo & Quản Lý Templates](05-configuration/template-setup.md)
- [Map Workflow](05-configuration/workflow-mapping.md)

### 6. Ví Dụ Thực Tế
- [Tạo Bug Report](06-examples/create-bug-report.md)
- [Tạo Feature Request](06-examples/create-feature-request.md)
- [Automation Scenarios](06-examples/automation-scenarios.md)

## 🚀 Bắt Đầu Nhanh

**1. Kết nối Jira:**
```
Settings → Integrations → Jira → Connect Account
```

**2. Tạo template đầu tiên:**
```
Jira → Templates → New Template → Bug Template
```

**3. Tạo issue đầu tiên:**
```
Ctrl+J → Nhập title → Enter
```

Done! Issue được tạo trên Jira với đầy đủ fields.

## 💡 Use Cases

- **Developer:** Tạo task nhanh khi coding, auto-log work khi commit
- **PM:** Quản lý templates, ưu tiên công việc
- **QA:** Report bug với screenshot, tìm issues tương tự
- **Support:** Escalate customer issues lên dev team

## 📊 Metrics

- **Tiết kiệm thời gian:** 80-90% (từ 3-5 phút → 30 giây)
- **Giảm lỗi:** 70% (nhờ templates và validation)
- **Tăng log work:** 3x (nhờ tự động hóa)

## 🔗 Related Modules

- [Knowledge Module](../knowledge/README.md) - Link issues với knowledge entries
- [Tasks Module](../tasks/README.md) - Sync tasks với Jira issues

## 🆘 Support

- [Troubleshooting](../../technical/troubleshooting.md#jira-module)
- [FAQ](03-user-guides/faq.md)
- [GitHub Issues](https://github.com/your-org/workhub/issues?q=is%3Aissue+label%3Ajira)

---

**Last Updated:** 2026-03-22  
**Status:** ✅ Phase 1 Complete
```

- [ ] **Commit Jira README**

```bash
git add docs/modules/jira/README.md
git commit -m "docs(jira): add module main README"
```

### Step 5.2: Create jira/01-overview.md

- [ ] **Create overview document**

```markdown
# Jira Module - Tổng Quan

## Vấn Đề Cần Giải Quyết

### 1. Tạo Issue Trên Jira Tốn Thời Gian

**Hiện tại:**
- Mở browser → Login Jira
- Navigate đến project
- Click "Create Issue"
- Fill 10-15 fields manually:
  - Issue Type
  - Summary
  - Description
  - Priority
  - Assignee
  - Sprint
  - Components
  - Labels
  - Story Points
  - Epic Link
  - ...
- Click "Create"
- Copy link share với team

**Thời gian:** 3-5 phút/issue × 10+ issues/ngày = **30-50 phút/ngày**

### 2. Thiếu Chuẩn Hóa

- Mỗi người viết issue theo kiểu riêng
- Thiếu thông tin quan trọng
- Khó search và filter
- Quality không đồng đều

### 3. Quên Log Work

- Developers quên log work time
- PM khó track progress
- Reports không chính xác

### 4. Context Switching

- Đang code → phải chuyển sang browser
- Mất focus
- Giảm productivity

## Giải Pháp

### 1. Quick Issue Creation

**Workflow mới:**
```
Ctrl+J → Nhập title → Enter
```

Hệ thống tự động:
- Detect issue type từ keywords
- Apply template phù hợp
- Fill default values
- Create trên Jira
- Return link

**Thời gian:** 30 giây

### 2. Smart Templates

**Issue Templates có sẵn:**
- 🐛 Bug Report
- ✨ Feature Request
- 🔥 Hotfix
- 🎫 Support Ticket
- ♻️ Refactor
- 📝 Technical Debt

**Mỗi template bao gồm:**
- Pre-filled fields
- Structured description format
- Validation rules
- Auto-assignment rules

**Ví dụ Bug Template:**
```markdown
## Bug Description
[Auto-filled from title]

## Steps to Reproduce
1. 
2. 
3. 

## Expected Behavior
[Empty for user to fill]

## Actual Behavior
[Empty for user to fill]

## Environment
- Version: [Auto-detected]
- Browser: [Auto-detected]
- OS: [Auto-detected]

## Screenshots
[Drag & drop area]

## Additional Context
[Optional]
```

### 3. Auto Log Work

**Tích hợp với Git:**
```bash
git commit -m "feat(auth): implement login #PROJ-123"
# Tự động log 30 minutes work vào PROJ-123
```

**Tích hợp với Timer:**
- Start work session → auto track time
- Stop session → auto log to Jira

### 4. Stay trong App

- Không cần rời VS Code hoặc app
- Quick shortcuts (Ctrl+J, Ctrl+Shift+J)
- Inline preview của issues

## So Sánh Before/After

| Thao Tác | Before (Jira Web) | After (WorkHub) | Tiết Kiệm |
|----------|------------------|----------------|-----------|
| **Tạo bug report** | 3-5 phút | 30 giây | **83-90%** |
| **Tạo feature** | 4-6 phút | 45 giây | **81-88%** |
| **Log work** | 1-2 phút (thường quên) | 0 giây (tự động) | **100%** |
| **Update status** | 30 giây × 5 times/day | 0 giây (auto-sync) | **100%** |
| **Search issues** | 2-3 phút | 10 giây (integrated search) | **83-90%** |

**Tổng tiết kiệm trung bình:** **30-45 phút/ngày/người**

Với team 10 người: **5-7.5 giờ/ngày** = **25-37.5 giờ/tuần**

## Key Benefits

### Cho Developers
✅ Tạo issues nhanh không rời code  
✅ Auto-log work time  
✅ Keyboard shortcuts  
✅ Templates giảm thinking overhead  

### Cho Project Managers
✅ Chuẩn hóa issue quality  
✅ Accurate time tracking  
✅ Better insights (vì đầy đủ data)  
✅ Manage templates centralized  

### Cho QA/Testers
✅ Bug template với screenshots  
✅ Link đến test cases  
✅ Find similar bugs (AI-powered)  
✅ Quick reproduce steps template  

### Cho Support Team
✅ Escalate customer issues dễ dàng  
✅ Link tickets với Jira issues  
✅ Track resolution status  
✅ Knowledge base integration  

## Technical Highlights

- **Jira REST API integration:** Full CRUD operations
- **Webhook support:** Real-time sync Jira → WorkHub
- **Offline mode:** Queue actions khi mất mạng
- **Bulk operations:** Xử lý nhiều issues cùng lúc
- **Custom fields:** Support mọi custom fields của Jira

## Architecture Overview

```
┌─────────────┐         ┌──────────────┐         ┌────────────┐
│   User      │────────▶│  WorkHub API │────────▶│ Jira Cloud │
│  Interface  │         │  Jira Module │         │   REST API │
└─────────────┘         └──────────────┘         └────────────┘
      │                         │
      │                         ▼
      │                 ┌──────────────┐
      │                 │   Database   │
      │                 │  (Templates, │
      │                 │    Mapping)  │
      │                 └──────────────┘
      │
      ▼
┌─────────────┐
│  Real-time  │
│   Updates   │
│  (SignalR)  │
└─────────────┘
```

## What's Next?

- [Quick Issue Creation](02-features/quick-issue-creation.md) - Chi tiết feature
- [Setup Guide](05-configuration/jira-connection.md) - Kết nối Jira
- [Templates](02-features/issue-templates.md) - Tạo templates
- [Examples](06-examples/create-bug-report.md) - Ví dụ thực tế

---

**Last Updated:** 2026-03-22
```

- [ ] **Commit overview**

```bash
git add docs/modules/jira/01-overview.md
git commit -m "docs(jira): add detailed overview with before/after comparison"
```

Due to length constraints, I'll create a few more key files for the Jira module, then we'll complete the plan.

### Step 5.3: Create 02-features/quick-issue-creation.md

- [ ] **Create quick issue creation feature doc**

```markdown
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
```

- [ ] **Commit quick issue creation doc**

```bash
git add docs/modules/jira/02-features/quick-issue-creation.md
git commit -m "docs(jira): add quick issue creation feature documentation"
```

### Step 5.4: Create remaining 02-features/ files

**Note:** Follow the structure of `02-features/quick-issue-creation.md` (created in Step 5.3) as reference. Each feature document should include: Overview, How It Works, UI Flow, Configuration, Examples, Error Handling.

- [ ] **Create 02-features/issue-templates.md**

Create file with these sections:
- **Overview:** Template system purpose & benefits
- **Template Types:** Bug, Feature, Hotfix, Support, Refactor, Tech Debt (list with descriptions)
- **Template Structure:** Required fields, optional fields, validation rules
- **Creating Templates:** Step-by-step with screenshots
- **Managing Templates:** Edit, delete, share, permissions
- **Variable Placeholders:** {title}, {user}, {date}, {autoDetect*}
- **Example Templates:** 2-3 complete template examples

```bash
git add docs/modules/jira/02-features/issue-templates.md
git commit -m "docs(jira): add issue templates documentation"
```

- [ ] **Create 02-features/auto-log-work.md**

Create file with these sections:
- **Overview:** Why auto-logging matters
- **Git Integration:** Commit message patterns (#PROJ-123), time estimation rules
- **Timer Integration:** Work session tracking, auto-pause, manual override
- **Configuration:** Enable/disable, time rounding, default time estimates
- **Accuracy:** How system estimates time from commits
- **Manual Override:** When to manually adjust logged time
- **Reports:** View logged time, export data

```bash
git add docs/modules/jira/02-features/auto-log-work.md
git commit -m "docs(jira): add auto-log work documentation"
```

- [ ] **Create 02-features/sync-jira.md**

Create file with these sections:
- **Overview:** Bidirectional sync architecture
- **Webhook Setup:** Jira → WorkHub real-time updates
- **Polling:** WorkHub → Jira periodic sync (frequency config)
- **Sync Scope:** What data syncs (status, assignee, comments, etc.)
- **Conflict Resolution:** Last-write-wins, manual resolution UI
- **Offline Handling:** Queue changes, sync when online
- **Monitoring:** Sync status dashboard, error logs

```bash
git add docs/modules/jira/02-features/sync-jira.md
git commit -m "docs(jira): add Jira sync documentation"
```

- [ ] **Create 02-features/bulk-operations.md**

Create file with these sections:
- **Overview:** When to use bulk operations
- **Bulk Create:** CSV import, template application, preview before create
- **Bulk Assign:** Filter + assign, team distribution
- **Bulk Update:** Status transitions, field updates, sprint assignment
- **Bulk Log Work:** Time entry across multiple issues
- **Performance:** Limits, batching, progress indication
- **Error Handling:** Partial failures, rollback, retry

```bash
git add docs/modules/jira/02-features/bulk-operations.md
git commit -m "docs(jira): add bulk operations documentation"
```

### Step 5.5: Create 03-user-guides/ files

**Note:** User guides should be task-oriented, showing common workflows with step-by-step instructions and screenshots.

- [ ] **Create 03-user-guides/for-developers.md**

Create file with these sections:
- **Quick Start:** 3-minute getting started
- **Daily Workflows:** Create issues from code, quick shortcuts (Ctrl+J patterns)
- **IDE Integration:** VS Code extension usage, IntelliJ plugin
- **Git Integration:** Commit message format, auto-log work, branch naming
- **Keyboard Shortcuts:** Complete shortcut reference
- **Best Practices:** When to create issues, naming conventions, linking code
- **Troubleshooting:** Common dev issues & solutions

```bash
git add docs/modules/jira/03-user-guides/for-developers.md
git commit -m "docs(jira): add developer user guide"
```

- [ ] **Create 03-user-guides/for-project-managers.md**

Create file with these sections:
- **Quick Start:** PM first-time setup
- **Template Management:** Create, edit, share templates across team
- **Team Configuration:** Assign component owners, sprint defaults
- **Reporting:** Velocity metrics, time tracking reports, export options
- **Priority Management:** Bulk priority updates, filters
- **Sprint Planning:** Auto-assign to sprints, burndown integration
- **Best Practices:** Template governance, team standards

```bash
git add docs/modules/jira/03-user-guides/for-project-managers.md
git commit -m "docs(jira): add PM user guide"
```

- [ ] **Create 03-user-guides/for-qa-testers.md**

Create file with these sections:
- **Quick Start:** QA setup & first bug report
- **Bug Reporting Workflow:** Screenshot tools, reproduce steps template
- **Test Case Linking:** Link bugs to test cases, traceability
- **Screenshot & Logs:** How to attach evidence, auto-capture
- **Finding Similar Issues:** Search before creating, de-duplication
- **Verification Workflow:** Mark as verified, re-open bugs
- **Best Practices:** Complete bug reports, severity guidelines

```bash
git add docs/modules/jira/03-user-guides/for-qa-testers.md
git commit -m "docs(jira): add QA tester user guide"
```

- [ ] **Create 03-user-guides/for-support-team.md**

Create file with these sections:
- **Quick Start:** Support team onboarding
- **Customer Issue Escalation:** From ticket to Jira issue workflow
- **Ticket-to-Issue Workflow:** Auto-link, bidirectional sync
- **Status Tracking:** Monitor Jira status from support dashboard
- **Knowledge Base Integration:** Link issues to knowledge entries
- **Templates:** Support ticket template, escalation template
- **Best Practices:** When to escalate, required information

```bash
git add docs/modules/jira/03-user-guides/for-support-team.md
git commit -m "docs(jira): add support team user guide"
```

### Step 5.6: Create 04-technical/ files

**Note:** Technical docs should include architecture diagrams, code examples, and API specs. Reference the coding standards from `docs/technical/coding-standards.md`.

- [ ] **Create 04-technical/architecture.md**

Create file with these sections:
- **Module Structure:** Domain/Application/Infrastructure projects folder tree
- **Design Patterns:** CQRS, Result<T>, Factory methods, Repository
- **Dependencies:** MediatR, FluentValidation, Jira SDK
- **Data Flow:** Request → Command/Query → Handler → Repository → Jira API (diagram)
- **Domain Events:** JiraIssueSynced, TemplateCreated (with subscribers)
- **Clean Architecture:** Dependency rule enforcement

```bash
git add docs/modules/jira/04-technical/architecture.md
git commit -m "docs(jira): add architecture documentation"
```

- [ ] **Create 04-technical/api-endpoints.md**

Create file with complete API reference:
- **Base URL:** /api/jira
- **Authentication:** JWT Bearer token required
- **Endpoints:**
  - `POST /templates` - Create template (request/response JSON)
  - `GET /templates` - List templates
  - `POST /issues/quick-create` - Quick create issue
  - `POST /issues/bulk` - Bulk operations
  - `GET /sync/status` - Sync health check
  - (Include ~10-15 total endpoints with full specs)
- **Error Codes:** 400/401/403/404/409/429/500 with examples
- **Rate Limiting:** 100 requests/minute per user

```bash
git add docs/modules/jira/04-technical/api-endpoints.md
git commit -m "docs(jira): add API endpoints documentation"
```

- [ ] **Create 04-technical/database-schema.md**

Create file with these sections:
- **Tables:**
  - `jira.IssueTemplates` - Schema with all columns, types, constraints
  - `jira.JiraIssueSyncs` - Sync tracking table
  - `jira.WorkLogEntries` - Auto-logged work time
- **Relationships:** Foreign keys, cascading deletes
- **Indexes:** Performance indexes on search columns
- **Migrations:** How to add/run migrations for Jira module
- **ERD Diagram:** Table relationships visualization

```bash
git add docs/modules/jira/04-technical/database-schema.md
git commit -m "docs(jira): add database schema documentation"
```

- [ ] **Create 04-technical/domain-models.md**

Create file with these sections:
- **Entities:**
  - `IssueTemplate` - Properties, factory methods, domain methods (code examples)
  - `JiraIssueSync` - Sync state machine
- **Value Objects:** JiraIssueKey, TemplateVariable
- **Enums:** IssueType, SyncStatus, TemplateCategory
- **Domain Events:** When fired, who subscribes, payload
- **Factory Pattern:** Why private constructors, Create() method examples
- **Validation:** Business rules, guard clauses

```bash
git add docs/modules/jira/04-technical/domain-models.md
git commit -m "docs(jira): add domain models documentation"
```

- [ ] **Create 04-technical/jira-api-integration.md**

Create file with these sections:
- **Jira REST API v3:** Base URL, authentication
-**API Token Setup:** Step-by-step with screenshots
- **SDK Usage:** Atlassian.NET SDK or raw HttpClient
- **Common Operations:** Create issue, get issue, update, search (with code)
- **Rate Limiting:** 10,000 req/hour, exponential backoff
- **Error Handling:** Network errors, API errors, retries with Polly
- **Webhooks:** Register webhooks, payload parsing, security
- **Testing:** Mock Jira API for unit tests

```bash
git add docs/modules/jira/04-technical/jira-api-integration.md
git commit -m "docs(jira): add Jira API integration documentation"
```

### Step 5.7: Create 05-configuration/ files

**Note:** Configuration guides should be step-by-step with numbered instructions and screenshots at each step.

- [ ] **Create 05-configuration/jira-connection.md**

Create file with these sections:
- **Prerequisites:** Jira Cloud account, admin access (or get from admin)
- **Step 1: Generate API Token** - Screenshots of Atlassian account settings
- **Step 2: Configure in WorkHub** - Settings → Integrations → Jira form
- **Step 3: Test Connection** - "Test Connection" button, success/error messages
- **Troubleshooting:**
  - Invalid credentials → check email/token
  - Permission denied → need Jira admin rights
  - Connection timeout → firewall/proxy issues
- **Security Notes:** Token storage, rotation policy

```bash
git add docs/modules/jira/05-configuration/jira-connection.md
git commit -m "docs(jira): add Jira connection guide"
```

- [ ] **Create 05-configuration/template-setup.md**

Create file with these sections:
- **Creating First Template:**
  1. Navigate to Jira → Templates
  2. Click "New Template"
  3. Choose type (Bug, Feature, etc.)
  4. Fill template builder form (screenshots)
- **Field Mapping:** Jira fields → WorkHub fields table
- **Validation Rules:** Required fields, regex patterns, conditional fields
- **Default Values:** Static defaults, dynamic (${user}, ${date})
- **Template Variables:** Complete variable reference
- **Testing Templates:** Preview before save
- **Sharing:** Team-wide vs personal templates, permissions
- **Best Practices:** Naming conventions, documentation

```bash
git add docs/modules/jira/05-configuration/template-setup.md
git commit -m "docs(jira): add template setup guide"
```

- [ ] **Create 05-configuration/workflow-mapping.md**

Create file with these sections:
- **Understanding Workflows:** Jira workflow vs WorkHub status
- **Default Mapping Table:**
  - WorkHub "To Do" → Jira "Open" / "To Do"
  - WorkHub "In Progress" → Jira "In Progress"
  - WorkHub "Done" → Jira "Done" / "Closed"
- **Custom Mapping:** Add custom statuses, transition rules
- **Sync Direction:** WorkHub → Jira, Jira → WorkHub, bidirectional
- **Transition Validation:** Required fields for transitions
- **Conflict Resolution:** What happens if statuses mismatch
- **Testing Mapping:** Create test issue, change status, verify sync

```bash
git add docs/modules/jira/05-configuration/workflow-mapping.md
git commit -m "docs(jira): add workflow mapping guide"
```

### Step 5.8: Create 06-examples/ files

**Note:** Examples should tell a story - real scenario from start to finish with actual content, not placeholders.

- [ ] **Create 06-examples/create-bug-report.md**

Create file as a complete tutorial:
- **Scenario:** "Login button unresponsive on mobile Safari"
- **Discovery:** QA tester finds bug during testing
- **Step 1:** Open quick create (Ctrl+J)
- **Step 2:** Type title "Login button not working on mobile Safari"
- **Step 3:** System detects: Type=Bug, Priority=High, Component=Authentication
- **Step 4:** Fill template (with actual content shown):
  ```
  Steps to Reproduce:
  1. Open app on iPhone 12 with Safari
  2. Navigate to /login
  3. Tap login button
  Expected: Login form submits
  Actual: Button appears frozen, no response
  ```
- **Step 5:** Attach screenshot (show how)
- **Step 6:** Click Create → gets PROJ-456
- **Result:** Issue visible in Jira with all fields filled
- **Screenshots:** 5-6 screenshots showing actual UI

```bash
git add docs/modules/jira/06-examples/create-bug-report.md
git commit -m "docs(jira): add bug report example"
```

- [ ] **Create 06-examples/create-feature-request.md**

Create file as complete tutorial:
- **Scenario:** PM wants "Dark mode" feature
- **User Story Format:**
  ```
  As a user who works late at night
  I want a dark mode option
  So that I can reduce eye strain
  ```
- **Acceptance Criteria:**
  - [ ] Toggle in settings
  - [ ] Persists across sessions
  - [ ] Affects all screens
- **Walkthrough:** PM creates feature request using template
- **Result:** PROJ-789 created with story points estimated

```bash
git add docs/modules/jira/06-examples/create-feature-request.md
git commit -m "docs(jira): add feature request example"
```

- [ ] **Create 06-examples/automation-scenarios.md**

Create file with 3-5 automation scenarios:
- **Scenario 1: Auto-assign bugs to component owner**
  - Rule: If Type=Bug AND Component=Auth → Assign to @johndoe
  - Configuration steps
  - Example execution
- **Scenario 2: Auto-add to current sprint**
  - Rule: If Priority=High AND Status=Approved → Add to active sprint
- **Scenario 3: Auto-label from keywords**
  - Rule: Title contains "mobile" → Add label "mobile"
- **Scenario 4: Conditional templates**
  - Rule: If customer-reported → Use "Support Ticket" template
- **Advanced:** Chain rules, webhook triggers

```bash
git add docs/modules/jira/06-examples/automation-scenarios.md
git commit -m "docs(jira): add automation scenarios examples"
```

---

## Task 6: Finalize and Review

### Step 6.1: Create directory index files

- [ ] **Create .gitkeep for empty directories**

```bash
# For directories that will be filled in Phase 2
touch docs/user-guides/.gitkeep
touch docs/technical/.gitkeep
touch docs/business/.gitkeep
touch docs/modules/knowledge/.gitkeep
touch docs/modules/tasks/.gitkeep
touch docs/modules/ai/.gitkeep
touch docs/modules/organization/.gitkeep
touch docs/modules/workspace/.gitkeep
```

- [ ] **Commit gitkeeps**

```bash
git add docs/**/.gitkeep
git commit -m "docs: add directory structure for Phase 2"
```

### Step 6.2: Final verification

- [ ] **Verify all documentation files exist**

Run verification:
```powershell
# Check all required files
$requiredFiles = @(
    "docs/README.md",
    "docs/GETTING-STARTED.md",
    "docs/templates/README.md",
    "docs/user-guides/README.md",
    "docs/technical/README.md",
    "docs/technical/development-setup.md",
    "docs/technical/coding-standards.md",
    "docs/modules/jira/README.md",
    "docs/modules/jira/01-overview.md"
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "✓ $file" -ForegroundColor Green
    } else {
        Write-Host "✗ $file MISSING" -ForegroundColor Red
    }
}
```

Expected: All files should exist

- [ ] **Check for broken links**

```powershell
# Manual review - open each README and click links
code docs/README.md
```

- [ ] **Final commit**

```bash
git add -A
git commit -m "docs: complete Phase 1 documentation system

- Created main documentation index and structure
- Added comprehensive templates for reuse
- Completed Jira module documentation (priority)
- Added user guides and technical docs structure
- Setup for Phase 2 expansion

Phase 1 deliverables complete ✅"
```

### Step 6.3: Update main project README

- [ ] **Update root README.md to link to docs**

At the end of d:\MiniProject\WorkHub\README.md, add:

```markdown
## 📚 Documentation

Comprehensive documentation is available in the [docs/](docs/) directory:

- [Getting Started](docs/GETTING-STARTED.md) - Setup in 15 minutes
- [Architecture](docs/ARCHITECTURE.md) - System design and ADRs
- [User Guides](docs/user-guides/) - Guides for all roles
- [Technical Docs](docs/technical/) - For developers
- [Module Docs](docs/modules/) - Per-module documentation

**Priority:** Start with [Jira Module](docs/modules/jira/) for a complete example.
```

- [ ] **Commit README update**

```bash
git add README.md
git commit -m "docs: add link to documentation from main README"
```

---

## Completion Checklist

- [ ] All Task 1 steps completed (Documentation root)
- [ ] All Task 2 steps completed (Templates)
- [ ] All Task 3 steps completed (User guides skeleton)
- [ ] All Task 4 steps completed (Technical docs)
- [ ] All Task 5 steps completed (Jira module - priority)
- [ ] All Task 6 steps completed (Finalization)
- [ ] All files committed to Git
- [ ] Links verified working
- [ ] README updated

---

## Post-Implementation

### Verification Commands

```bash
# Count documentation files created
Get-ChildItem -Path docs -Recurse -File | Measure-Object | Select-Object Count

# View commit history
git log --oneline --graph docs/

# Check documentation coverage
tree docs /F
```

### Success Criteria

✅ Phase 1 complete khi:
- [ ] docs/ structure exists and populated
- [ ] Jira module fully documented (README + 01-overview + features)
- [ ] Templates ready for reuse
- [ ] Getting started guide comprehensive
- [ ] Development setup detailed
- [ ] All committed to Git

### Next Phase

After Phase 1 completion:
1. User review documentation
2. Identify gaps or unclear sections
3. Plan Phase 2:  Knowledge Module enhancement
4. Continue with other modules

---

**Plan created:** 2026-03-22  
**Estimated time:** 2-3 weeks (solo developer)  
**Status:** Ready for execution
