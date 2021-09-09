using IdentityJwt.Models;
using IdentityJwt.UseCases.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityJwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly INotifications _notification;

        public UserController(IMediator mediator, INotifications notification)
        {
            this.mediator = mediator;
            _notification = notification;
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserRequest user)
        {
            var result = mediator.Send(user).Result;

            if (result is Models.User)
                return new OkObjectResult(result);

            return new BadRequestObjectResult(_notification.SerializedNotifications);
        }

    }
}