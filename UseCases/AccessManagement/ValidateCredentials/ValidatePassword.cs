using IdentityJwt.Security;
using Microsoft.AspNetCore.Identity;

namespace IdentityJwt.UseCases.AccessManagement.ValidateCredentials
{
    public class ValidatePassword : IValidateCredentialsStrategy
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenConfigurations _tokenConfigurations;

        public ValidatePassword(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenConfigurations tokenConfigurations)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenConfigurations = tokenConfigurations;
        }

        public bool Validate(AccessCredentials credenciais)
        {
            var (userId, password) = (credenciais.UserID, credenciais.Password);

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
    }
}
