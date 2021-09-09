using MediatR;
using IdentityJwt.UseCases.AccessManagement;

namespace IdentityJwt.Models
{
    public class RefreshTokenData : IRequest<bool>
    {
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public bool CompareTokens(string userID, string refreshToken) =>
            userID == this.UserId && refreshToken == this.RefreshToken;

        public bool Equals(RefreshTokenData obj) =>
            CompareTokens(obj.UserId, obj.RefreshToken);

        public GenerateTokenRequest GetTokenRequest() => new()
        {
            UserID = this.UserId,
        };
    }
}