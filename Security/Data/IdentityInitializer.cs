using System;
using Microsoft.AspNetCore.Identity;
using IdentityJwt.Repository;

namespace IdentityJwt.Security.Data
{

    public class IdentityInitializer
    {
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly APISecurityDbContext _context;
        private readonly IRepository<Models.User> _userRepository;
        private readonly IRepository<Models.Role> _roleRepository;

        public IdentityInitializer(
            TokenConfigurations tokenConfigurations,
            APISecurityDbContext context,
            IRepository<Models.User> userRepository,
            IRepository<Models.Role> roleRepository)
        {
            _tokenConfigurations = tokenConfigurations;
            _context = context;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                _roleRepository.Add(new Models.Role(_tokenConfigurations.AccessRole));

                _userRepository.Add(new Models.User(
                    userId: new Guid("06b2a676-dc43-4aa3-8db8-cdece8d28187"),
                    userName: "admin_apicontagem",
                    email: "admin-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "AdminAPIContagem01!",
                    roles: _tokenConfigurations.AccessRole
                ));

                _userRepository.Add(new Models.User(
                    userId: Guid.NewGuid(),
                    userName: "usrinvalido_apicontagem",
                    email: "usrinvalido-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "UsrInvAPIContagem01!"
                ));
            }
        }
    }
}
