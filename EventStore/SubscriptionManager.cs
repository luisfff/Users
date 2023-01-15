using EventSourcing.Interfaces;
using EventStore.ClientAPI;
using EventStore.Logging;

namespace EventStore;

public class SubscriptionManager
{
    private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

    private readonly ICheckpointStore _checkpointStore;
    private readonly string _name;
    private readonly IEventStoreConnection _connection;
    private readonly ISubscription[] _subscriptions;
    private EventStoreAllCatchUpSubscription _subscription;

    public SubscriptionManager(
        IEventStoreConnection connection,
        ICheckpointStore checkpointStore,
        string name,
        params ISubscription[] subscriptions)
    {
        _connection = connection;
        _checkpointStore = checkpointStore;
        _name = name;
        _subscriptions = subscriptions;
    }

    public async Task Start()
    {
        var settings = new CatchUpSubscriptionSettings(
            2000, 500,
            Log.IsDebugEnabled(),
            false, _name
        );

        Log.Debug("Starting the projection manager...");

        var position = await _checkpointStore.GetCheckpoint();
        Log.Debug("Retrieved the checkpoint: {checkpoint}", position);

        _subscription = _connection.SubscribeToAllFrom(
            GetPosition(),
            settings, 
            EventAppeared
        );
        Log.Debug("Subscribed to $all stream");

        Position? GetPosition()
            => position.HasValue
                ? new Position(position.Value, position.Value)
                : AllCheckpoint.AllStart;
    }

    private async Task EventAppeared(
        EventStoreCatchUpSubscription _,
        ResolvedEvent resolvedEvent)
    {
        if (resolvedEvent.Event.EventType.StartsWith("$")) return;

        var @event = resolvedEvent.Deserialze();

        Log.Debug("Projecting event {event}", @event.ToString());

        try
        {
            await Task.WhenAll(_subscriptions.Select(x => x.Project(@event)));

            await _checkpointStore.StoreCheckpoint(
                // ReSharper disable once PossibleInvalidOperationException
                resolvedEvent.OriginalPosition.Value.CommitPosition
            );
        }
        catch (Exception e)
        {
            Log.Error(
                e,
                "Error occured when projecting the event {event}",
                @event
            );
            throw;
        }
    }

    public void Stop() => _subscription.Stop();
}