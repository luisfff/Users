using EventStore;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using RavenDb;
using Users.Application.Projections;

namespace Users.Application
{
    public static class UsersModule
    {
        private const string SubscriptionName = "usersSubscription";

        public static IMvcCoreBuilder AddUsersModule(this IMvcCoreBuilder builder, string databaseName)
        {
            EventMappings.MapEventTypes();

            builder.Services.AddSingleton(
                    c =>
                        new UserProfileCommandService(
                            new EsAggregateStore(
                                c.GetRequiredService<IEventStoreConnection>()
                            )
                        )
                )
                .AddSingleton<GetUsersModuleSession>(
                    c =>
                    {
                        var store = c.GetRequiredService<IDocumentStore>();
                        store.CheckAndCreateDatabase(databaseName);

                        IAsyncDocumentSession GetSession()
                            => store.OpenAsyncSession(databaseName);

                        return GetSession;
                    }
                )
                .AddSingleton(
                    c =>
                    {
                        var getSession =
                            c.GetRequiredService<GetUsersModuleSession>();

                        return new SubscriptionManager(
                            c.GetRequiredService<IEventStoreConnection>(),
                            new RavenDbCheckpointStore(
                                () => getSession(),
                                SubscriptionName
                            ),
                            SubscriptionName,
                            new RavenDbProjection<UserDetails>(
                                () => getSession(),
                                UserDetailsProjection.GetHandler
                            )
                        );
                    }
                );

            builder.AddApplicationPart(typeof(UsersModule).Assembly);

            return builder;
        }
    }

    public delegate IAsyncDocumentSession GetUsersModuleSession();
}