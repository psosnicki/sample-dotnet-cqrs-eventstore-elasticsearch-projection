namespace IssueTracker.Application.Interfaces
{
    public interface IViewRepository<T>
    {
        Task<T?> Get(Guid id, CancellationToken cancellationToken);
        Task Add(Guid id, T entity, CancellationToken cancellationToken);
        Task Update(Guid id, T entity, CancellationToken cancellationToken);
        Task Delete(Guid id, T entity, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken);
    }
}
