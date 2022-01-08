using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using IdentityJwt.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using IdentityJwt.Infra.Security;

namespace IdentityJwt.UseCases.AccessManagement
{
    public class GenerateTokenHandler : IRequestHandler<GenerateTokenRequest, Token>
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GenerateTokenHandler> logger;

        public GenerateTokenHandler(SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache,
            ILogger<GenerateTokenHandler> logger, 
            UserManager<ApplicationUser> userManager)
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
            this.logger = logger;
            _userManager = userManager;
        }

        public async Task<Token> Handle(GenerateTokenRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Generating token");

            var roles = await _userManager.GetRolesAsync(request.ApplicationUser);

            var identity = CreateClaims(request.ApplicationUser, () =>
            {
                var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

                claims.AddRange(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, request.ApplicationUser.Id),
                    new Claim(ClaimTypes.Name, request.ApplicationUser.UserName)
                });

                return claims.ToArray();
            });

            var (dateFrom, dateTo) = GetTokenValidPeriod();

            var strToken = GetStringToken(identity, dateFrom, dateTo);

            var token = GetToken(strToken, dateFrom, dateTo);

            StoreRefreshToken(request.ApplicationUser.UserName, token.RefreshToken);

            return await Task.FromResult(token);
        }

        #region GenerateToken

        private static ClaimsIdentity CreateClaims(ApplicationUser applicationUser, Func<Claim[]> roles) => new(roles());

        private string GetStringToken(ClaimsIdentity identity, DateTime dateFrom, DateTime dateTo)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dateFrom,
                Expires = dateTo
            });

            return handler.WriteToken(securityToken);
        }

        private static Token GetToken(string token, DateTime dateFrom, DateTime dateTo) =>
            new()
            {
                Authenticated = true,
                Created = dateFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dateTo.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", string.Empty),
                Message = "OK"
            };

        private (DateTime dateFrom, DateTime dateTo) GetTokenValidPeriod() =>
            (DateTime.Now, DateTime.Now + TimeSpan.FromSeconds(_tokenConfigurations.Seconds));

        private void StoreRefreshToken(string userId, string refreshToken)
        {
            // Armazena o refresh token em cache através do Redis 
            var refreshTokenData = new RefreshTokenData
            {
                RefreshToken = refreshToken,
                UserId = userId
            };

            // Calcula o tempo máximo de validade do refresh token
            // (o mesmo será invalidado automaticamente pelo Redis)
            TimeSpan finalExpiration =
                TimeSpan.FromSeconds(_tokenConfigurations.FinalExpiration);

            DistributedCacheEntryOptions opcoesCache = new();

            opcoesCache.SetAbsoluteExpiration(finalExpiration);

            _cache.SetString(refreshToken,
                JsonSerializer.Serialize(refreshTokenData),
                opcoesCache);
        }
        #endregion
    }

    public class GenerateTokenRequest : IRequest<Token>
    {
        public ApplicationUser ApplicationUser { get; set; }
    }
}
