using IdentityJwt.Models;
using IdentityJwt.UseCases.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityJwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        public async Task<IActionResult> Add(
            [FromBody] UserRequest user,
            [FromServices] IMediator mediator,
            [FromServices] INotifications notification
            )
        {
            if (await mediator.Send(user) is User result)
                return new OkObjectResult(result);

            return new BadRequestObjectResult(notification.Notifications);
        }

    }
}