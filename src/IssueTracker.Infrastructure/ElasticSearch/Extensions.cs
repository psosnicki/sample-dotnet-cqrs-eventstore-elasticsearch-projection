using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Infrastructure.ElasticSearch
{
    public static class Extensions
    {
        public static IServiceCollection AddElasticsearch(
             this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Connection"]));
            var client = new ElasticsearchClient(settings);
            return services.AddSingleton(client);
        }
    }
}
