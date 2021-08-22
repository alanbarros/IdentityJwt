using IdentityJwt.UseCases.AccessManagement;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.Json;

namespace IdentityJwt.Security
{
    public class ApplicationUser : IdentityUser
    {
        public Guid UserId { get; set; }
    }

    public class AccessCredentials : IRequest<bool>
    {
        /// <summary>
        /// User name
        /// </summary>
        /// <example>admin_apicontagem</example>
        public string UserID { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        /// <example>AdminAPIContagem01!</example>
        public string Password { get; set; }

        /// <summary>
        /// GUID RefreshToken
        /// </summary>
        /// <example>ccf16c1031fe48dcafe323b54c45c3eb</example>
        public string RefreshToken { get; set; }

        /// <summary>
        /// GrantType: password or refresh_token
        /// </summary>
        /// <example>password</example>
        public string GrantType { get; set; }

        public bool CompareTokens(string userID, string refreshToken) =>
            userID == this.UserID && refreshToken == this.RefreshToken;

        public GenerateTokenRequest GetTokenRequest() => new()
        {
            UserID = this.UserID,
        };
    }

    public class TokenConfigurations
    {
        public string AccessRole { get; set; }
        public string SecretJWTKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
        public int FinalExpiration { get; set; }
    }

    public class Token
    {
        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }

    public class RefreshTokenData
    {
        public string RefreshToken { get; set; }
        public string UserID { get; set; }
    }

    public static class Util
    {
        public static T JsonParse<T>(string text) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return JsonSerializer.Deserialize<T>(text);
        }
    }
}