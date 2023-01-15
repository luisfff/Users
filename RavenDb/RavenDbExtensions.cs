using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide.Operations;

namespace RavenDb;

public static class RavenDbExtensions
{
    public static async Task Update<T>(
        this IAsyncDocumentSession session,
        string id,
        Action<T> update
    )
    {
        var item = await session.LoadAsync<T>(id);

        if (item == null) return;

        update(item);
    }

    public static Task Del(
        this IAsyncDocumentSession session,
        string id
    )
    {
        session.Delete(id);
        return Task.CompletedTask;
    }

    public static void CheckAndCreateDatabase(
        this IDocumentStore store,
        string databaseName
    )
    {
        var record = store.Maintenance.Server.Send(
            new GetDatabaseRecordOperation(databaseName)
        );

        if (record == null)
            store.Maintenance.Server.Send(
                new CreateDatabaseOperation(
                    new(databaseName)
                )
            );
    }
}