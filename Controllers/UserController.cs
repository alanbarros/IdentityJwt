using IdentityJwt.Models;
using IdentityJwt.UseCases.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityJwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        public IActionResult Add(
            [FromBody] UserRequest user,
            [FromServices] IMediator mediator,
            [FromServices] INotifications notification
            )
        {
            User result = mediator.Send(user).Result;

            if (result is not null)
                return new OkObjectResult(result);

            return new BadRequestObjectResult(notification.Notifications);
        }

    }
}