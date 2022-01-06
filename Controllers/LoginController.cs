using IdentityJwt.Models;
using IdentityJwt.UseCases.AccessManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

            var option = await mediator.Send(credenciais);

            return GetResponse(option, credenciais.GetTokenRequest);
        }

        [HttpPost]
        [Route("ByRefreshToken")]
        [ProducesResponseType(typeof(Token), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ByRefreshToken(RefreshTokenData refreshToken)
        {
            logger.LogInformation("Login by refresh token");

            var option = await mediator.Send(refreshToken);

            return GetResponse(option, refreshToken.GetTokenRequest);
        }

        private IActionResult GetResponse(
            Optional.Option<ApplicationUser> option, 
            Func<ApplicationUser, GenerateTokenRequest> GetTokenRequest) =>
            option.Match<IActionResult>(
                some: x => new OkObjectResult(mediator.Send(GetTokenRequest(x)).Result),
                none: () => new UnauthorizedResult()
                );
    }
}
