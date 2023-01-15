using Raven.Client.Documents;

namespace Users.Api.Configuration
{
    public static class RavenDbConfiguration
    {
        public static IDocumentStore Configure(
            string serverUrl
        )
        {
            var store = new DocumentStore
            {
                Urls = new[] { serverUrl }
            };
            store.Initialize();

            return store;
        }
    }
}