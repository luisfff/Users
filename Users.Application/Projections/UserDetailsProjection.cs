using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using RavenDb;
using Users.Messages.Events;

namespace Users.Application.Projections
{
    public static class UserDetailsProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            var getDbId = UserDetails.GetDatabaseId;

            return @event switch
            {
                UserRegistered e => 
                    () => Create(e.UserId, e.DisplayName, e.FullName),
                UserFullNameUpdated e =>
                    () => Update(e.UserId, x => x.FullName = e.FullName),
                _ => null
            };

            Task Update(long id, Action<UserDetails> update)
                => session.Update(getDbId(id), update);

            Task Create(long userId, string displayName, string fullName)
                => session.StoreAsync(
                    new UserDetails
                    {
                        Id = getDbId(userId),
                        DisplayName = displayName,
                        FullName = fullName
                    }
                );
        }
    }
}