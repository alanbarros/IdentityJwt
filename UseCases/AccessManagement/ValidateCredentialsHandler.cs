using IdentityJwt.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateCredentialsHandler : IRequestHandler<AccessCredentials, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly ILogger<ValidateCredentialsHandler> _logger;

        public ValidateCredentialsHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache,
            ILogger<ValidateCredentialsHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenConfigurations = tokenConfigurations;
            _logger = logger;
        }

        public Task<bool> Handle(AccessCredentials request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (request == null && string.IsNullOrWhiteSpace(request.UserId))
                    return false;

                return ValidatePassword(request);
            });
        }

        private bool ValidatePassword(AccessCredentials credenciais)
        {
            _logger.LogInformation("Validating credential");

            var (userId, password) = (credenciais.UserId, credenciais.Password);

            // Verifica a existência do usuário nas tabelas do
            // ASP.NET Core Identity
            var userIdentity = _userManager.FindByNameAsync(userId).Result;

            if (userIdentity == null)
                return false;

            // Efetua o login com base no Id do usuário e sua senha
            var resultadoLogin = _signInManager
                .CheckPasswordSignInAsync(userIdentity, password, false).Result;

            if (resultadoLogin.Succeeded)
            {
                _logger.LogInformation("Credential validated");
                // Verifica se o usuário em questão possui
                // a role de acesso
                return _userManager.IsInRoleAsync(
                    userIdentity, _tokenConfigurations.AccessRole).Result;
            }

            _logger.LogWarning("Invalid credential");
            return false;
        }

    }
}