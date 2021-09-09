using IdentityJwt.Models;
using IdentityJwt.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateCredentialsHandler : IRequestHandler<AccessCredentials, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IDistributedCache _cache;

        public ValidateCredentialsHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
        }

        public Task<bool> Handle(AccessCredentials request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (request == null && string.IsNullOrWhiteSpace(request.UserId))
                    return false;

                return request.GrantType switch
                {
                    "password" => ValidatePassword(request),
                    "refresh_token" => ValidateRefreshToken(request),
                    _ => false
                };
            });
        }

        #region ValidateCredentials
        private bool ValidatePassword(AccessCredentials credenciais)
        {
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
                // Verifica se o usuário em questão possui
                // a role de acesso
                return _userManager.IsInRoleAsync(
                    userIdentity, _tokenConfigurations.AccessRole).Result;
            }

            return false;
        }

        private bool ValidateRefreshToken(AccessCredentials credenciais)
        {
            var (userId, refreshToken) = (credenciais.UserId, credenciais.RefreshToken);

            if (string.IsNullOrWhiteSpace(credenciais.RefreshToken))
                return false;

            string storedToken = _cache.GetString(refreshToken);

            if (Util.JsonParse<RefreshTokenData>(storedToken) is RefreshTokenData)
            {
                var validatedCredentials = credenciais.CompareTokens(userId, refreshToken);

                // Elimina o token de refresh já que um novo será gerado
                if (validatedCredentials)
                    _cache.Remove(refreshToken);

                return validatedCredentials;
            }

            return false;
        }
        #endregion
    }
}