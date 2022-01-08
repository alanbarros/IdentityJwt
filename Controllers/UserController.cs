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
        [Route("Add")]
        public async Task<IActionResult> Add(
            [FromBody] UserRequest user,
            [FromServices] IMediator mediator,
            [FromServices] INotifications notification) => 
            (await mediator.Send(user))
                .Match<IActionResult>(
                    some: _ => new OkObjectResult(user.User),
                    none: () => new BadRequestObjectResult(notification.Notifications));

    }
}