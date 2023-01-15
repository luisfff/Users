using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Application;
using Users.Domain;
using Users.Messages.Commands;
using WebApi;

namespace Users.Api.Controllers
{
    [Route("user")]
    public class UserCommandsApi
        : CommandApi<UserProfile>
    {
        public UserCommandsApi(
            UserProfileCommandService applicationService,
            ILoggerFactory loggerFactory)
            : base(applicationService, loggerFactory) { }

        [HttpPost]
        public Task<IActionResult> Post([FromBody] RegisterUser request)
            => HandleCommand(new RegisterUser()
            {
                DisplayName = request.DisplayName,
                FullName = request.FullName,
                UserId = request.UserId
            });

        [Route("fullname"), HttpPut]
        public Task<IActionResult> Put(UpdateUserFullName request)
            => HandleCommand(request);
    }
}