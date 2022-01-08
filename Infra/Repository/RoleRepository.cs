using IdentityJwt.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace IdentityJwt.Infra.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotifications _notifications;

        public RoleRepository(RoleManager<IdentityRole> roleManager, INotifications notifications)
        {
            _roleManager = roleManager;
            _notifications = notifications;
        }

        public StatusLevel Add(Role entity)
        {
            if (Exists(entity.Name))
            {
                _notifications.AddNotification("Rule", $"The rule {entity.Name} already exists");
                return StatusLevel.WARNING;
            }

            var resultado = _roleManager.CreateAsync(entity).Result;

            if (resultado.Succeeded)
                return StatusLevel.SUCCESS;

            _notifications.AddNotifications(resultado.Errors.Select(e => (e.Code, e.Description)));
            return StatusLevel.FAIL;
        }

        public bool Exists(string name) => _roleManager.RoleExistsAsync(name).Result;
    }
}