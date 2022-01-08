using System.Collections.Generic;
using System.Linq;

namespace IdentityJwt.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }

        public List<Role> GetRoles() => Roles.Select(s => new Role(s)).ToList();

        public User()
        {

        }

        public User(
            string userName,
            string email,
            bool emailConfirmed,
            string password,
            params string[] roles)
        {
            UserName = userName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Password = password;
            Roles = roles?.ToList() ?? new List<string>();
        }

    }
}