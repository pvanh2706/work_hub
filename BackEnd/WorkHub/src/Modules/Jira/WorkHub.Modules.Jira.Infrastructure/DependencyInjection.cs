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
        services.AddDbContext<JiraDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IJiraIssueSyncRepository, JiraIssueSyncRepository>();
        services.AddScoped<IIssueTemplateRepository, IssueTemplateRepository>();

        var baseUrl = configuration["Jira:BaseUrl"]
            ?? throw new InvalidOperationException("Jira:BaseUrl is not configured.");
        var email = configuration["Jira:Email"]
            ?? throw new InvalidOperationException("Jira:Email is not configured.");
        var apiToken = configuration["Jira:ApiToken"]
            ?? throw new InvalidOperationException("Jira:ApiToken is not configured.");

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
