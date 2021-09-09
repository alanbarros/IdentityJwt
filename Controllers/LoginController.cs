using IdentityJwt.Models;
using IdentityJwt.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        public LoginController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("ByPassword")]
        public Token ByPassword(AccessCredentials credenciais)
        {
            if (mediator.Send(credenciais).Result)
                return mediator.Send(credenciais.GetTokenRequest()).Result;

            return new()
            {
                Authenticated = false,
                Message = "Falha ao autenticar"
            };
        }

        [HttpPost]
        [Route("ByRefreshToken")]
        public Token ByRefreshToken(RefreshTokenData refreshToken)
        {
            if (mediator.Send(refreshToken).Result)
                return mediator.Send(refreshToken.GetTokenRequest()).Result;

            return new()
            {
                Authenticated = false,
                Message = "Falha ao autenticar"
            };
        }
    }
}
