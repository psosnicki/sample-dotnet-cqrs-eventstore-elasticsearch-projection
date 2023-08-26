using IssueTracker.Domain.Common;

namespace IssueTracker.Domain.Repositories
{
    public interface IEventStoreRepository
    {
        public Task SaveAsync<T>(T aggregate) where T : Aggregate, new();
        public Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new();
    }
}
