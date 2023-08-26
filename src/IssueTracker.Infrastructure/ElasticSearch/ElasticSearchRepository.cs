using Elastic.Clients.Elasticsearch;
using IssueTracker.Application.Interfaces;

namespace IssueTracker.Infrastructure.ElasticSearch
{
    public class ElasticSearchRepository<T> : IViewRepository<T> where T : class
    {
        private readonly ElasticsearchClient _elasticClient;

        public ElasticSearchRepository(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task<T?> Get(Guid id, CancellationToken cancellationToken)
        {
            var response = await _elasticClient.GetAsync<T>(id,i=>i.Index(GetIndexName()), cancellationToken).ConfigureAwait(false);
            return response?.Source;
        }

        public Task Add(Guid id, T entity, CancellationToken cancellationToken)
        {
            return _elasticClient.IndexAsync(entity, i => i.Id(id).Index(GetIndexName()), cancellationToken);
        }

        public Task Update(Guid id, T entity, CancellationToken cancellationToken)
        {
            return _elasticClient.UpdateAsync<T, object>(GetIndexName(), id, i => i.Doc(entity), cancellationToken);
        }

        public Task Delete(Guid id, T entity, CancellationToken cancellationToken)
        {
            return _elasticClient.DeleteAsync<T>(id,i=>i.Index(GetIndexName()), cancellationToken);
        }

        private static string GetIndexName()
        {
            var entityType = typeof(T);
            var modulePrefix = entityType.Namespace!.Split(".").First();
            return $"{modulePrefix}-{entityType.Name}".ToLower();
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default)
        {
            var response = await _elasticClient.SearchAsync<T>(s=>s.Index(GetIndexName()), cancellationToken);
            if (response.ApiCallDetails.HttpStatusCode == (int)System.Net.HttpStatusCode.NotFound)
                return Enumerable.Empty<T>();
            return response?.Documents ?? Enumerable.Empty<T>();
        }
    }
}
