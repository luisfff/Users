using System.Text;
using EventSourcing;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStore;

public static class EventStoreExtensions
{
    public static Task AppendEvents(
        this IEventStoreConnection connection,
        string streamName,
        long version,
        params object[] events)
    {
        if (events == null || !events.Any()) return Task.CompletedTask;

        var preparedEvents = events
            .Select(
                @event =>
                    new EventData(
                        Guid.NewGuid(),
                        TypeMapper.GetTypeName(@event.GetType()),
                        true,
                        Serialize(@event),
                        Serialize(
                            new EventMetadata
                            {
                                ClrType = @event.GetType().FullName
                            }
                        )
                    )
            )
            .ToArray();

        return connection.AppendToStreamAsync(
            streamName,
            version,
            preparedEvents
        );
    }

    private static byte[] Serialize(object data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
}