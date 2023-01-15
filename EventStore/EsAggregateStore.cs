using EventSourcing;
using EventSourcing.Interfaces;
using EventStore.ClientAPI;
using EventStore.Logging;

namespace EventStore;

public class EsAggregateStore : IAggregateStore
{
    private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

    private readonly IEventStoreConnection _connection;

    public EsAggregateStore(IEventStoreConnection connection) 
        => _connection = connection;

    public async Task Save<T>(T aggregate) where T : AggregateRoot
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var streamName = GetStreamName(aggregate);
        var changes = aggregate.GetChanges().ToArray();

        foreach (var change in changes)
            Log.Debug("Persisting event {event}", change.ToString());

        await _connection.AppendEvents(streamName, aggregate.Version, changes);

        aggregate.ClearChanges();
    }

    public async Task<T> Load<T>(AggregateId<T> aggregateId)
        where T : AggregateRoot
    {
        if (aggregateId == null)
            throw new ArgumentNullException(nameof(aggregateId));

        var stream = GetStreamName(aggregateId);
        var aggregate = (T) Activator.CreateInstance(typeof(T), true);

        var page = await _connection.ReadStreamEventsForwardAsync(
            stream, 0, 1024, false
        );

        Log.Debug("Loading events for the aggregate {aggregate}", aggregate.ToString());

        aggregate.Load(
            page.Events.Select(
                    resolvedEvent => resolvedEvent.Deserialze()
                )
                .ToArray()
        );

        return aggregate;
    }

    public async Task<bool> Exists<T>(AggregateId<T> aggregateId) 
        where T : AggregateRoot
    {
        var stream = GetStreamName(aggregateId);
        var result = await _connection.ReadEventAsync(stream, 1, false);
        return result.Status != EventReadStatus.NoStream;
    }

    private static string GetStreamName<T>(AggregateId<T> aggregateId) 
        where T : AggregateRoot 
        => $"{typeof(T).Name}-{aggregateId}";

    private static string GetStreamName<T>(T aggregate)
        where T : AggregateRoot
        => $"{typeof(T).Name}-{aggregate.Id.ToString()}";
}