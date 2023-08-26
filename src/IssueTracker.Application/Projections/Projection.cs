using IssueTracker.Application.Interfaces;

namespace IssueTracker.Application.Projections
{
    public interface IProjection
    {
        Task Handle(object @event, CancellationToken cancellationToken);
        bool CanHandle(Type eventType);
    }

    public abstract class Projection<TView> : IProjection
    {
        protected readonly IViewRepository<TView> _viewRepository;
        private readonly Dictionary<Type, (Func<object, TView,CancellationToken, ValueTask<TView>>, Func<object, Guid>)> _eventHandlers = new();
        public Projection(IViewRepository<TView> viewRepository)
        {
            _viewRepository = viewRepository;
        }

        protected void Project<TEvent>(Func<TEvent, TView, CancellationToken, ValueTask<TView>> action, Func<TEvent, Guid> getViewId) where TEvent : class
        {
            Guid getViewFunc(object value) => getViewId((TEvent)value);
            _eventHandlers.Add(typeof(TEvent), ((@event, view, ct) => action((TEvent)@event, view, ct), getViewFunc));
        }

        public async Task Handle(object @event, CancellationToken cancellationToken)
        {
            var (action, getViewId) = _eventHandlers[@event.GetType()];
            var viewId = getViewId.Invoke(@event);
            var view = await _viewRepository.Get(getViewId.Invoke(@event), default);

            bool create = false;
            if (view is null)
            {
                create = true;
                view = (TView)Activator.CreateInstance(typeof(TView), true)!;
            }
            var projection = await action(@event, view, cancellationToken);
            //TODO create add/update projections
            if (create)
                await _viewRepository.Add(viewId, projection, cancellationToken);
            else await _viewRepository.Update(viewId, projection, cancellationToken);
        }
        public bool CanHandle(Type eventType) => _eventHandlers.Keys.Any(x => x == eventType);
    }
}
