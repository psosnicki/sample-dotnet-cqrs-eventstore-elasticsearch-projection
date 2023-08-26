using IssueTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IssueTracker.Infrastructure.ElasticSearch;
using IssueTracker.Infrastructure.EventStore;
using IssueTracker.Application.Projections;
using IssueTracker.Domain.Repositories;

namespace IssueTracker.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
            => services.AddElasticsearch(configuration)
                       .AddHostedService<EventStoreProjector>();
        public static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration)
            => services.AddEventStoreClient(configuration["EventStore:Connection"])
                       .AddTransient<IEventStoreRepository, EventStoreRepository>();
        public static IServiceCollection AddProjection<TView,TProjection>(this IServiceCollection services) where TProjection : class , IProjection where TView : class
            => services.AddTransient<IProjection, TProjection>()
                       .AddTransient<IViewRepository<TView>, ElasticSearchRepository<TView>>();
    }
}
