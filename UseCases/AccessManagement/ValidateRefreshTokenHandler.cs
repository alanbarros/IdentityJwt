using System.Threading;
using System.Threading.Tasks;
using IdentityJwt.Models;
using IdentityJwt.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Optional;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateRefreshTokenHandler : IRequestHandler<RefreshTokenData, Option<ApplicationUser>>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ValidateCredentialsHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ValidateRefreshTokenHandler(IDistributedCache cache,
            ILogger<ValidateCredentialsHandler> logger, 
            UserManager<ApplicationUser> userManager)
        {
            _cache = cache;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Option<ApplicationUser>> Handle(RefreshTokenData request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refreshing token");

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return await Task.FromResult(RefreshingTokenFailed("Refresh token is empty"));

            RefreshTokenData storedToken = _cache
                .GetString(request.RefreshToken)
                .ConvertTo<RefreshTokenData>();

            if (storedToken is null)
                return await Task.FromResult(RefreshingTokenFailed("Refresh token was not found"));

            // Elimina o token de refresh já que um novo será gerado
            if (storedToken.Equals(request))
            {
                _cache.Remove(request.RefreshToken);
                _logger.LogInformation("Token successfully refreshed");

                var user = await _userManager.FindByIdAsync(request.UserId);

                return user.Some();
            }

            return await Task.FromResult(RefreshingTokenFailed("Refresh token is invalid!"));
        }

        private Option<ApplicationUser> RefreshingTokenFailed(string message)
        {
            _logger.LogWarning($"Refreshing token failed because: {message}");

            return Option.None<ApplicationUser>();
        }
    }
}