using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace EventStore;

public class EventStoreService : IHostedService
{
    private readonly IEventStoreConnection _esConnection;
    private readonly IEnumerable<SubscriptionManager> _subscriptionManagers;

    public EventStoreService(
        IEventStoreConnection esConnection,
        IEnumerable<SubscriptionManager> subscriptionManagers)
    {
        _esConnection = esConnection;
        _subscriptionManagers = subscriptionManagers;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _esConnection.ConnectAsync();

        await Task.WhenAll(
            _subscriptionManagers
                .Select(projection => projection.Start())
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscriptionManagers.ForEach(x => x.Stop());
        _esConnection.Close();
        return Task.CompletedTask;
    }
}