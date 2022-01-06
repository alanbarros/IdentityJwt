using MediatR;
using IdentityJwt.UseCases.AccessManagement;
using Optional;

namespace IdentityJwt.Models
{
    public class RefreshTokenData : IRequest<Option<ApplicationUser>>
    {

        /// <summary>
        /// GUID RefreshToken
        /// </summary>
        /// <example>ccf16c1031fe48dcafe323b54c45c3eb</example>
        public string RefreshToken { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        /// <example>admin_apicontagem</example>
        public string UserId { get; set; }
        public bool CompareTokens(string userID, string refreshToken) =>
            userID == this.UserId && refreshToken == this.RefreshToken;

        public bool Equals(RefreshTokenData obj) =>
            CompareTokens(obj.UserId, obj.RefreshToken);

        public GenerateTokenRequest GetTokenRequest(ApplicationUser applicationUser) => new()
        {
            ApplicationUser = applicationUser,
        };
    }
}