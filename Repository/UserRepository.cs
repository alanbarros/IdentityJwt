using Microsoft.AspNetCore.Identity;
using IdentityJwt.Security;
using System;
using System.Linq;
using IdentityJwt.Models;
using System.Collections.Generic;

namespace IdentityJwt.Repository
{
    public class UserRepository : IRepository<Models.User>
    {
        private const int warning = 2;
        private const int success = 1;
        private const int fail = 0;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotifications _notifications;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            INotifications notifications)
        {
            _userManager = userManager;
            _notifications = notifications;
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
                    AddRoles(applicationUser, user.Roles);
                    return success;
                }

                AddNotifications(resultado.Errors);
                return fail;
            }

            _notifications.AddNotification("User", $"The user {user.UserName} already exists");

            return fail;
        }

        private void AddRoles(ApplicationUser applicationUser, List<string> roles) =>
            _userManager.AddToRolesAsync(applicationUser, roles).Wait();

        public int Add(Models.User user, IEnumerable<Models.Role> roles)
        {
            var applicationUser = GetApplicationUser(user);

            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager
                    .CreateAsync(applicationUser, user.Password).Result;

                if (resultado.Succeeded && user.Roles.Any())
                {
                    AddRoles(applicationUser, roles.Select(a => a.Name).ToList());
                    return success;
                }

                AddNotifications(resultado.Errors);
                return fail;
            }

            _notifications.AddNotification("User", $"The user {user.UserName} already exists");

            return fail;

        }

        private void AddNotifications(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                _notifications.AddNotification(error.Code, error.Description);
            }
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