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
        public GenerateTokenRequest GetTokenRequest() => new()
        {
            UserID = this.UserId,
        };
    }

}