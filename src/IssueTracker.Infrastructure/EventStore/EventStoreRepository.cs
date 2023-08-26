using EventStore.Client;
using IssueTracker.Domain.Common;
using IssueTracker.Domain.Repositories;
using System.Text;
using System.Text.Json;

namespace IssueTracker.Infrastructure.EventStore;

internal class EventStoreRepository : IEventStoreRepository
{
    private readonly EventStoreClient _eventStoreClient;
    public EventStoreRepository(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }
    public async Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new()
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException(nameof(aggregateId));

        var streamName = GetStreamName<T>(aggregateId);
        var aggregate = new T();

        var readStreamResult = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start);

        if (await readStreamResult.ReadState == ReadState.StreamNotFound)
            return null;

        var events = new List<object>();
        await foreach (var @event in readStreamResult)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
            var type = Type.GetType(Encoding.UTF8.GetString(@event.Event.Metadata.ToArray()));
            var @object = JsonSerializer.Deserialize(json, type);
            events.Add(@object);
        }

        aggregate.Load(events);
        return aggregate;
    }

    public async Task SaveAsync<T>(T aggregate) where T : Aggregate, new()
    {
        var events = aggregate.DequeueUncommittedEvents();
        if (!events.Any())
            return;
        var streamName = GetStreamName<T>(aggregate.Id);
        var eventsToSave = new List<EventData>();
        foreach (var @event in events)
        {
            var eventData = new EventData(
                eventId: Uuid.NewUuid(),
                type: @event.GetType().Name,
                data: JsonSerializer.SerializeToUtf8Bytes(@event),
                metadata: Encoding.UTF8.GetBytes(@event.GetType().AssemblyQualifiedName));

            eventsToSave.Add(eventData);
        }
        await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, eventsToSave);
    }
    private static string GetStreamName<T>(Guid aggregateId) => $"{typeof(T).Name}-{aggregateId}";
}

