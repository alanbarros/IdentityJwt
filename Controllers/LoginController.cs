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

        /// <summary>
        /// Obtenha aqui o token ou o refresh token
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna o JWT token e o refresh token</response>
        [HttpPost]
        public Token Post(AccessCredentials credenciais)
        {
            if (mediator.Send(credenciais).Result)
                return mediator.Send(credenciais.GetTokenRequest()).Result;

            return new()
            {
                Authenticated = false,
                Message = "Falha ao autenticar"
            };
        }
    }
}
