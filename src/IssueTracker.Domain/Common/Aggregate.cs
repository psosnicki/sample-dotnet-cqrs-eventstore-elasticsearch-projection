namespace IssueTracker.Domain.Common
{
    public interface IAggregate : IAggregate<Guid> { }
    public interface IAggregate<out T>
    {
        T Id { get; }
        object[] DequeueUncommittedEvents();
    }
    public abstract class Aggregate : Aggregate<Guid>, IAggregate { }
    public abstract class Aggregate<T> : IAggregate<T> where T : notnull
    {
        public T Id { get; protected set; } = default!;
        [NonSerialized] private readonly Queue<object> uncommittedEvents = new();
        public abstract void When(object @event);
        public object[] DequeueUncommittedEvents()
        {
            var dequeuedEvents = uncommittedEvents.ToArray();
            uncommittedEvents.Clear();
            return dequeuedEvents;
        }

        public void Load(IEnumerable<object> events)
        {
            foreach(var e in events)
                When(e);
        }

        protected void EnqueueDomainEvent(object @event)
        {
            uncommittedEvents.Enqueue(@event);
        }
    }
}
