using Microsoft.AspNetCore.Identity;
using IdentityJwt.Security;
using System;

namespace IdentityJwt.Repository
{
    public class UserRepository : IRepository<Models.User>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public int Add(Models.User user)
        {
            var applicationUser = GetApplicationUser(user);

            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager
                    .CreateAsync(applicationUser, user.Password).Result;

                if (resultado.Succeeded &&
                    !String.IsNullOrWhiteSpace(user.Roles[0]))
                {
                    _userManager.AddToRoleAsync(applicationUser, user.Roles[0]).Wait();
                }
            }

            return 1;
        }

        private ApplicationUser GetApplicationUser(Models.User user) => new ApplicationUser
        {
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed
        };
    }
}