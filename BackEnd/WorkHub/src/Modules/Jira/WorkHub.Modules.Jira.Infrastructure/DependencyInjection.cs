using System.Net.Http.Headers;
using System.Text;
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
        // UseNpgsql: dùng PostgreSQL. Connection string lấy từ appsettings.json
        // (key "DefaultConnection") — không hardcode trong code để dễ đổi môi trường.
        services.AddDbContext<JiraDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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

        // ── Bước 4: Đăng ký HttpClient có typed cho IJiraClient ─────────────────────
        //
        // AddHttpClient<Interface, Implementation> = "Typed HttpClient":
        //   - IHttpClientFactory quản lý vòng đời của HttpMessageHandler bên dưới,
        //     tránh socket exhaustion nếu tự new HttpClient() liên tục.
        //   - JiraApiClient được inject IJiraClient interface vào Application layer,
        //     không cần biết đây là HTTP call ra ngoài.
        //
        // BaseAddress: gắn base URL một lần ở đây, các method trong JiraApiClient
        // chỉ cần dùng path tương đối (vd: "rest/api/3/issue").
        //
        // Authorization Basic: Jira Cloud dùng Basic Auth với email + API token,
        // encode dạng Base64("{email}:{token}"). Token này được tạo trên Jira account
        // settings, không phải password tài khoản.
        //
        // Accept: application/json: báo với Jira API rằng client muốn nhận JSON.
        services.AddHttpClient<IJiraClient, JiraApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{apiToken}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
