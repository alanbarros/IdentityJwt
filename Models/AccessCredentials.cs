using IdentityJwt.UseCases.AccessManagement;
using MediatR;
using Optional;

namespace IdentityJwt.Models
{
    public class AccessCredentials : IRequest<Option<ApplicationUser>>
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
        public GenerateTokenRequest GetTokenRequest(ApplicationUser applicationUser) => new()
        {
            ApplicationUser = applicationUser
        };
    }

}