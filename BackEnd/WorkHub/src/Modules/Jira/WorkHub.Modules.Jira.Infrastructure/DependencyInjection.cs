using System.Net.Http.Headers;
using System.Text;
using AtlassianJira = Atlassian.Jira.Jira;
using WorkHub.Modules.Jira.Application.Abstractions;
using WorkHub.Modules.Jira.Domain.Repositories;
using WorkHub.Modules.Jira.Infrastructure.ExternalServices;
using WorkHub.Modules.Jira.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorkHub.Modules.Jira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJiraModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Bước 1: Đăng ký DbContext riêng cho Jira module ─────────────────────────
        //
        // Mỗi module có DbContext riêng (JiraDbContext) thay vì dùng chung một DbContext
        // toàn ứng dụng. Đây là nguyên tắc của Modular Monolith / Clean Architecture:
        // các module độc lập với nhau, không truy cập trực tiếp vào DB của module khác.
        //
        // UseSqlite: dùng SQLite. Connection string lấy từ appsettings.json
        // (key "DefaultConnection") — ví dụ: "Data Source=jira.db"
        services.AddDbContext<JiraDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // ── Bước 2: Đăng ký các Repository với lifetime Scoped ──────────────────────
        //
        // Interface (IJiraIssueSyncRepository) được đăng ký với implementation cụ thể
        // (JiraIssueSyncRepository). Nhờ vậy, Application layer chỉ phụ thuộc vào
        // interface — không biết gì về EF Core hay PostgreSQL (Dependency Inversion).
        //
        // Scoped = tạo mới một instance cho mỗi HTTP request, dùng chung trong suốt
        // request đó. Phù hợp với DbContext vì DbContext không thread-safe.
        services.AddScoped<IJiraIssueSyncRepository, JiraIssueSyncRepository>();
        services.AddScoped<IIssueTemplateRepository, IssueTemplateRepository>();

        // ── Bước 3: Đọc cấu hình Jira API từ configuration ──────────────────────────
        //
        // Các giá trị nhạy cảm (email, API token) được đọc từ IConfiguration thay vì
        // hardcode. Trong production, chúng được inject qua biến môi trường hoặc
        // secret manager — không bao giờ commit vào source code.
        //
        // ?? throw: fail-fast ngay khi app khởi động nếu thiếu config,
        // thay vì để lỗi xảy ra lúc runtime khi có request thật — dễ debug hơn.
        var baseUrl = configuration["Jira:BaseUrl"]
            ?? throw new InvalidOperationException("Jira:BaseUrl is not configured.");
        var email = configuration["Jira:Email"]
            ?? throw new InvalidOperationException("Jira:Email is not configured.");
        var apiToken = configuration["Jira:ApiToken"]
            ?? throw new InvalidOperationException("Jira:ApiToken is not configured.");

        // ── Bước 4: Đăng ký IJiraClient — chọn implementation qua config ────────────
        //
        // "Jira:UseSDK": true  → dùng Atlassian.SDK (JiraSdkClient)
        // "Jira:UseSDK": false → dùng raw HttpClient (JiraApiClient)  ← mặc định
        //
        // Tại sao tách thành 2 option?
        //   - SDK: code gọn hơn, có thể dùng được LINQ/object model của Atlassian
        //   - Raw API: kiểm soát hoàn toàn request/response, dễ debug, không phụ thuộc SDK
        // Cả hai implement cùng interface IJiraClient nên Application layer
        // không cần thay đổi gì khi switch.
        var useSDK = configuration.GetValue<bool>("Jira:UseSDK");

        if (useSDK)
        {
            // SDK tạo ra Atlassian.Jira.Jira object — đây là "connection" đến Jira.
            // Đăng ký Singleton vì object này thread-safe và tốn chi phí khởi tạo.
            var jiraConnection = AtlassianJira.CreateRestClient(baseUrl, email, apiToken);
            services.AddSingleton(jiraConnection);
            services.AddScoped<IJiraClient, JiraSdkClient>();
        }
        else
        {
            // Raw HttpClient — IHttpClientFactory quản lý socket pool,
            // tránh socket exhaustion nếu tự new HttpClient() liên tục.
            services.AddHttpClient<IJiraClient, JiraApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{apiToken}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", token);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        return services;
    }
}
