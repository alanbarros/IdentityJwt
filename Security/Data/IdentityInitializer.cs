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
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(
            TokenConfigurations tokenConfigurations,
            APISecurityDbContext context,
            IRepository<Models.User> userRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _tokenConfigurations = tokenConfigurations;
            _context = context;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                if (!_roleManager.RoleExistsAsync(_tokenConfigurations.AccessRole).Result)
                {
                    var resultado = _roleManager.CreateAsync(
                        new IdentityRole(_tokenConfigurations.AccessRole)).Result;
                    if (!resultado.Succeeded)
                    {
                        throw new Exception(
                            $"Erro durante a criação da role {_tokenConfigurations.AccessRole}.");
                    }
                }

                _userRepository.Add(new Models.User(
                    userName: "admin_apicontagem",
                    email: "admin-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "AdminAPIContagem01!",
                    roles: _tokenConfigurations.AccessRole
                ));

                _userRepository.Add(new Models.User(
                    userName: "usrinvalido_apicontagem",
                    email: "usrinvalido-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "UsrInvAPIContagem01!"
                ));
            }
        }
    }
}
