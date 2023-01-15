using System;
using System.Threading.Tasks;
using Users.Application.Projections;

namespace Users.Application
{
    public static class Queries
    {
        public static Task<UserDetails> GetUserDetails(this GetUsersModuleSession getSession, long id)
        {
            using var session = getSession();

            return session.LoadAsync<UserDetails>(UserDetails.GetDatabaseId(id));
        }
    }
}