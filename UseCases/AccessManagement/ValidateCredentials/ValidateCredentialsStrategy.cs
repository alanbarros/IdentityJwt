using IdentityJwt.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityJwt.UseCases.AccessManagement.ValidateCredentials
{
    public class ValidateCredentialsStrategy : IValidateCredentialsStrategy
    {
        private readonly IValidateCredentialsStrategy validateRefreshToken;
        private readonly IValidateCredentialsStrategy validatePassword;

        public ValidateCredentialsStrategy(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache)
        {
            validatePassword = new ValidatePassword(userManager, signInManager, tokenConfigurations);
            validateRefreshToken = new ValidateRefreshToken(cache);
        }

        public bool Validate(AccessCredentials credenciais)
        {
            return credenciais.GrantType switch
            {
                "password" => validatePassword.Validate(credenciais),
                "refresh_token" => validateRefreshToken.Validate(credenciais),
                _ => false
            };
        }
    }
}
