using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Application;
using Users.Application.Projections;

namespace Users.Api.Controllers
{
    [Route("user")]
    public class UserQueryApi : ControllerBase
    {
        private readonly GetUsersModuleSession _getSession;

        public UserQueryApi(
            GetUsersModuleSession getSession)
            => _getSession = getSession;

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDetails>> Get(long userId)
        {
            var user = await _getSession.GetUserDetails(userId);

            if (user == null) return NotFound();

            return user;
        }
    }
}