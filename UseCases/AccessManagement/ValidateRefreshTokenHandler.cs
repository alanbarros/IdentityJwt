using System.Threading;
using System.Threading.Tasks;
using IdentityJwt.Models;
using IdentityJwt.Security;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class ValidateRefreshTokenHandler : IRequestHandler<RefreshTokenData, bool>
    {
        private readonly IDistributedCache _cache;

        public ValidateRefreshTokenHandler(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task<bool> Handle(RefreshTokenData request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                    return false;

                RefreshTokenData storedToken = _cache
                    .GetString(request.RefreshToken)
                    .ConvertTo<RefreshTokenData>();

                if (storedToken is null)
                    return false;

                // Elimina o token de refresh já que um novo será gerado
                if (storedToken.Equals(request))
                {
                    _cache.Remove(request.RefreshToken);
                    return true;
                }

                return false;
            });
        }
    }
}