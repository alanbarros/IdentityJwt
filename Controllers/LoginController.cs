using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityJwt.Security;
using IdentityJwt.UseCases.AccessManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityJwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public Token Post(AccessCredentials credenciais, 
            [FromServices] IMediator mediator)
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
