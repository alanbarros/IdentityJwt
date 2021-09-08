using IdentityJwt.Security;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityJwt.UseCases.AccessManagement.ValidateCredentials
{
    public class ValidateRefreshToken : IValidateCredentialsStrategy
    {
        private readonly IDistributedCache _cache;

        public ValidateRefreshToken(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool Validate(AccessCredentials credenciais)
        {
            var (userId, refreshToken) = (credenciais.UserID, credenciais.RefreshToken);

            if (string.IsNullOrWhiteSpace(credenciais.RefreshToken))
                return false;

            string storedToken = _cache.GetString(refreshToken);

            if (Util.JsonParse<RefreshTokenData>(storedToken) is RefreshTokenData)
            {
                var credenciaisValidas = credenciais.CompareTokens(userId, refreshToken);

                // Elimina o token de refresh já que um novo será gerado
                if (credenciaisValidas)
                    _cache.Remove(refreshToken);

                return credenciaisValidas;
            }

            return false;
        }
    }
}
