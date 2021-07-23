using Microsoft.AspNetCore.Identity;

namespace IdentityJwt.Models
{
    public class Role : IdentityRole
    {

        public Role(string roleName) : base(roleName)
        {

        }
    }
}