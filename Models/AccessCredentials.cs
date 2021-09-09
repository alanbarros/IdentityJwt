using IdentityJwt.UseCases.AccessManagement;
using MediatR;

namespace IdentityJwt.Models
{
    public class AccessCredentials : IRequest<bool>
    {
        /// <summary>
        /// User name
        /// </summary>
        /// <example>admin_apicontagem</example>
        public string UserId { get; set; }

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
            userID == this.UserId && refreshToken == this.RefreshToken;

        public GenerateTokenRequest GetTokenRequest() => new()
        {
            UserID = this.UserId,
        };
    }

}