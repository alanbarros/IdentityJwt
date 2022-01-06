using IdentityJwt.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityJwt.Controllers
{
    /// <summary>
    ///  API para login
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<LoginController> logger;

        public LoginController(IMediator mediator, ILogger<LoginController> logger)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("ByPassword")]
        [ProducesResponseType(typeof(Token), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ByPassword(AccessCredentials credenciais)
        {
            logger.LogInformation("Login by password");

            if (mediator.Send(credenciais).Result)
                return new OkObjectResult(await mediator.Send(credenciais.GetTokenRequest()));

            return new UnauthorizedResult();
        }

        [HttpPost]
        [Route("ByRefreshToken")]
        [ProducesResponseType(typeof(Token), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ByRefreshToken(RefreshTokenData refreshToken)
        {
            logger.LogInformation("Login by refresh token");

            if (mediator.Send(refreshToken).Result)
                return new OkObjectResult(await mediator.Send(refreshToken.GetTokenRequest()));

            return new UnauthorizedResult();
        }
    }
}
