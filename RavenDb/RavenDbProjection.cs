using EventSourcing.Interfaces;
using Raven.Client.Documents.Session;
using RavenDb.Logging;

namespace RavenDb;

public class RavenDbProjection<T> : ISubscription
{
    private static readonly ILog Log = LogProvider.GetCurrentClassLogger();
    private static readonly string ReadModelName = typeof(T).Name;

    public RavenDbProjection(
        GetSession getSession,
        Projector projector
    )
    {
        _projector = projector;
        GetSession = getSession;
    }

    private GetSession GetSession { get; }
    private readonly Projector _projector;

    public async Task Project(object @event)
    {
        using var session = GetSession();

        var handler = _projector(session, @event);

        if ( handler == null) return;

        Log.Debug("Projecting {event} to {model}", @event, ReadModelName);

        await handler();
        await session.SaveChangesAsync();
    }

    public delegate Func<Task> Projector(
        IAsyncDocumentSession session,
        object @event
    );
}