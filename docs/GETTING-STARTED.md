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
