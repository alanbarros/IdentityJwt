using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IdentityJwt.Security
{
    public class AccessManager
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private SigningConfigurations _signingConfigurations;
        private TokenConfigurations _tokenConfigurations;
        private IDistributedCache _cache;

        public AccessManager(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
        }

        public bool ValidateCredentials(AccessCredentials credenciais)
        {
            if (credenciais == null && String.IsNullOrWhiteSpace(credenciais.UserID))
                return false;

            return credenciais.GrantType switch
            {
                "password" => ValidatePassword(credenciais),
                "refresh_token" => ValidateRefreshToken(credenciais),
                _ => false
            };
        }
        
        public Token GenerateToken(AccessCredentials credenciais)
        {
            var identity = CreateClaims(credenciais.UserID);

            var (dateFrom, dateTo) = GetTokenValidPeriod();

            var strToken = GetStringToken(identity, dateFrom, dateTo);

            var token = GetToken(strToken, dateFrom, dateTo);

            StoreRefreshToken(credenciais.UserID, token.RefreshToken);

            return token;
        }

        #region ValidateCredentials
        private bool ValidatePassword(AccessCredentials credenciais)
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

        private bool ValidateRefreshToken(AccessCredentials credenciais)
        {
            var (userId, refreshToken) = (credenciais.UserID, credenciais.RefreshToken);

            if (String.IsNullOrWhiteSpace(credenciais.RefreshToken))
                return false;

            string storedToken = _cache.GetString(refreshToken);

            if (Util.JsonParse<RefreshTokenData>(storedToken)
                is RefreshTokenData refreshTokenBase)
            {
                var credenciaisValidas = credenciais
                    .CompareTokens(userId, refreshToken);

                // Elimina o token de refresh já que um novo será gerado
                if (credenciaisValidas)
                    _cache.Remove(refreshToken);
            }

            return false;
        }
        #endregion

        #region GenerateToken

        private ClaimsIdentity CreateClaims(string userId) =>
            new ClaimsIdentity(
                new GenericIdentity(userId, "Login"),
                new[] 
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId)
                }
            );

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

        private Token GetToken(string token, DateTime dateFrom, DateTime dateTo) => 
            new Token()
            {
                Authenticated = true,
                Created = dateFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dateTo.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                Message = "OK"
            };

        private (DateTime dateFrom, DateTime dateTo) GetTokenValidPeriod() =>
            (DateTime.Now, DateTime.Now + TimeSpan.FromSeconds(_tokenConfigurations.Seconds));

        private void StoreRefreshToken(string userId, string refreshToken)
        {
            // Armazena o refresh token em cache através do Redis 
            var refreshTokenData = new RefreshTokenData();
            refreshTokenData.RefreshToken = refreshToken;
            refreshTokenData.UserID = userId;

            // Calcula o tempo máximo de validade do refresh token
            // (o mesmo será invalidado automaticamente pelo Redis)
            TimeSpan finalExpiration =
                TimeSpan.FromSeconds(_tokenConfigurations.FinalExpiration);

            DistributedCacheEntryOptions opcoesCache =
                new DistributedCacheEntryOptions();

            opcoesCache.SetAbsoluteExpiration(finalExpiration);

            _cache.SetString(refreshToken,
                JsonSerializer.Serialize(refreshTokenData),
                opcoesCache);
        }
        #endregion
    }
}