using WorkHub.Modules.Knowledge.Application.Abstractions;
using WorkHub.Modules.Knowledge.Domain.Repositories;
using WorkHub.Modules.Knowledge.Infrastructure.Persistence;
using WorkHub.Modules.Knowledge.Infrastructure.Repositories;
using WorkHub.Modules.Knowledge.Infrastructure.Search;
using Elastic.Clients.Elasticsearch;
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

        var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        services.AddSingleton(new ElasticsearchClient(new Uri(elasticsearchUri)));

        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
        services.AddScoped<ISearchIndexer, ElasticsearchIndexer>();
        services.AddScoped<IKnowledgeSearchService, ElasticsearchSearchService>();

        return services;
    }
}
