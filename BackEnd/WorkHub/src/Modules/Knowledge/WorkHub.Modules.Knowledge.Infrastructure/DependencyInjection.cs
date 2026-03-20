using WorkHub.Modules.Knowledge.Application.Abstractions;
using WorkHub.Modules.Knowledge.Domain.Repositories;
using WorkHub.Modules.Knowledge.Infrastructure.Persistence;
using WorkHub.Modules.Knowledge.Infrastructure.Repositories;
using WorkHub.Modules.Knowledge.Infrastructure.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorkHub.Modules.Knowledge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddKnowledgeModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<KnowledgeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
        services.AddScoped<ISearchIndexer, ElasticsearchIndexer>();
        services.AddScoped<IKnowledgeSearchService, ElasticsearchSearchService>();

        return services;
    }
}
