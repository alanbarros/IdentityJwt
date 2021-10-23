using System.Threading;
using System.Threading.Tasks;
using IdentityJwt.Models;
using IdentityJwt.Security;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateRefreshTokenHandler : IRequestHandler<RefreshTokenData, bool>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ValidateCredentialsHandler> _logger;

        public ValidateRefreshTokenHandler(IDistributedCache cache,
            ILogger<ValidateCredentialsHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<bool> Handle(RefreshTokenData request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing token");

            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                    return RefreshingTokenFailed("Refresh token is empty");

                RefreshTokenData storedToken = _cache
                    .GetString(request.RefreshToken)
                    .ConvertTo<RefreshTokenData>();

                if (storedToken is null)
                    return RefreshingTokenFailed("Refresh token was not found");

                // Elimina o token de refresh já que um novo será gerado
                if (storedToken.Equals(request))
                {
                    _cache.Remove(request.RefreshToken);
                    _logger.LogInformation("Token successfully refreshed");
                    return true;
                }

                return RefreshingTokenFailed("Refresh token is invalid!");
            });
        }

        private bool RefreshingTokenFailed(string message)
        {
            _logger.LogWarning($"Refreshing token failed because: {message}");
            return false;
        }
    }
}