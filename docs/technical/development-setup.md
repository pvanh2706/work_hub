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
