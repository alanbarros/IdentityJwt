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

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserRequest user)
        {
            var result = mediator.Send(user).Result;

            if (result is Models.User)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }

    }
}