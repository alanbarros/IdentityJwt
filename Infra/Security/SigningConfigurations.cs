using System.Text;
using IdentityJwt.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityJwt.Infra.Security
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(TokenConfigurations tokenConfigurations)
        {
            Key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenConfigurations.SecretJWTKey));

            SigningCredentials = new(
                Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}