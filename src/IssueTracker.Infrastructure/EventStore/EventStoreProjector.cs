using EventStore.Client;
using IssueTracker.Application.Projections;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace IssueTracker.Infrastructure.EventStore;

internal class EventStoreProjector : IHostedService
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IEnumerable<IProjection> _projections;
    private readonly ConcurrentDictionary<string, ulong?> _checkPoints = new(); //in memory checkpoint dev
    private readonly string _subscriptionId = $"{nameof(EventStoreProjector)}Subscription";

    public EventStoreProjector(EventStoreClient eventStoreClient, IEnumerable<IProjection> projections)
    {
        _eventStoreClient = eventStoreClient;
        _projections = projections;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();
        _checkPoints.TryGetValue(_subscriptionId, out var checkpoint);
        await _eventStoreClient.SubscribeToAllAsync(checkpoint == null ? FromAll.Start : FromAll.After(new Position(checkpoint.Value, checkpoint.Value)), HandleEvent, cancellationToken: cancellationToken);
    }

    private async Task HandleEvent(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
    {
        object? @event = null;
        Type? eventType = null;
        try
        {
            var json = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            eventType = Type.GetType(Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
            if (eventType == null) return;
            @event = JsonSerializer.Deserialize(json, eventType);
        }
        catch (Exception)
        {
            //skip other events
            return;
        }

        if (@event != null)
        {
            foreach (var projection in _projections.Where(p => p.CanHandle(eventType)))
                await projection.Handle(@event, cancellationToken);

            var currentPosition = resolvedEvent.Event.Position.CommitPosition;
            _checkPoints.AddOrUpdate(subscription.SubscriptionId, currentPosition, (_, _) => currentPosition);
        }
        //TODO retry on failure / reconnect
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
