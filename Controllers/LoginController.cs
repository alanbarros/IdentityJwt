using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityJwt.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityJwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public Token Post(AccessCredentials credenciais, [FromServices] AccessManager accessManager)
        {
            if (accessManager.ValidateCredentials(credenciais))
                return accessManager.GenerateToken(credenciais);
            else
                return new ()
                {
                    Authenticated = false,
                    Message = "Falha ao autenticar"
                };
        }
    }
}
