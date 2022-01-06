using IdentityJwt.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Optional;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateCredentialsHandler : IRequestHandler<AccessCredentials, Option<ApplicationUser>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ValidateCredentialsHandler> _logger;

        public ValidateCredentialsHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ValidateCredentialsHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Option<ApplicationUser>> Handle(AccessCredentials request, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                if (request == null && string.IsNullOrWhiteSpace(request.UserId))
                    return Option.None<ApplicationUser>();

                return ValidatePassword(request);
            });
        }

        private Option<ApplicationUser> ValidatePassword(AccessCredentials credenciais)
        {
            _logger.LogInformation("Validating credential");

            var (userId, password) = (credenciais.UserId, credenciais.Password);

            // Verifica a existência do usuário nas tabelas do
            // ASP.NET Core Identity
            ApplicationUser userIdentity = _userManager.FindByNameAsync(userId).Result;

            if (userIdentity == null)
                return Option.None<ApplicationUser>();

            // Efetua o login com base no Id do usuário e sua senha
            var resultadoLogin = _signInManager
                .CheckPasswordSignInAsync(userIdentity, password, false).Result;

            if (resultadoLogin.Succeeded)
            {
                _logger.LogInformation("Credential validated");

                return userIdentity.Some();
            }

            _logger.LogWarning("Invalid credential");
            return Option.None<ApplicationUser>();
        }

    }
}