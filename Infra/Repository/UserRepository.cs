using AutoMapper;
using IdentityJwt.Models;
using Microsoft.AspNetCore.Identity;
using Optional;
using System.Collections.Generic;
using System.Linq;

namespace IdentityJwt.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository roleRepository;
        private readonly INotifications _notifications;
        private readonly IMapper mapper;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IRoleRepository roleRepository,
            INotifications notifications,
            IMapper mapper)
        {
            _userManager = userManager;
            this.roleRepository = roleRepository;
            _notifications = notifications;
            this.mapper = mapper;
        }

        public StatusLevel Add(User user) => FindByName(user.UserName).Match(
        some: _ =>
        {
            _notifications.AddNotification("User", $"The user {user.UserName} already exists");

            return StatusLevel.FAIL;
        },
        none: () =>
        {
            var applicationUser = GetApplicationUser(user);

            var resultado = _userManager
                .CreateAsync(applicationUser, user.Password).Result;

            if (resultado.Succeeded)
            {
                if (user.Roles.Any())
                {
                    user.Roles.ForEach(role => roleRepository.Add(new Role(role)));

                    AddRoles(applicationUser, user.Roles);
                    return StatusLevel.SUCCESS;
                }

                _notifications.AddNotification("User", $"The user {user.UserName} was added, but with no rules");
                return StatusLevel.WARNING;
            }

            _notifications.AddNotifications(resultado.Errors.Select(e => (e.Code, e.Description)));
            return StatusLevel.FAIL;
        });

        private void AddRoles(ApplicationUser applicationUser, List<string> roles) =>
            _userManager.AddToRolesAsync(applicationUser, roles).Wait();

        public StatusLevel Add(User user, IEnumerable<Role> roles)
        {
            user.Roles = roles.Select(a => a.Name).ToList();

            return Add(user);
        }
                

        private ApplicationUser GetApplicationUser(User user) => mapper.Map<User, ApplicationUser>(user);

        public Option<User> FindByName(string name)
        {
            var user = _userManager.FindByNameAsync(name).Result;

            if (user is null)
                return Option.None<User>();

            return mapper.Map<ApplicationUser, User>(user).Some();
        }
    }
}