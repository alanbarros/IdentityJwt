using Microsoft.AspNetCore.Identity;
using IdentityJwt.Security;
using System;
using System.Linq;
using IdentityJwt.Models;

namespace IdentityJwt.Repository
{
    public class UserRepository : IRepository<Models.User>
    {
        private const int success = 1;
        private const int fail = 0;
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

                if (resultado.Succeeded && user.Roles.Any())
                {
                    _userManager.AddToRolesAsync(applicationUser, user.Roles).Wait();
                }

                return resultado.Succeeded ? success : fail;
            }

            return fail;
        }

        private ApplicationUser GetApplicationUser(Models.User user) => new ApplicationUser
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed
        };
    }
}