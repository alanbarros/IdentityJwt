using System;
using IdentityJwt.Repository;
using IdentityJwt.Models;
using MediatR;
using IdentityJwt.UseCases.CreateUser;

namespace IdentityJwt.Security.Data
{

    public class IdentityInitializer
    {
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly APISecurityDbContext _context;
        private readonly IMediator _mediator;
        private readonly IRepository<Models.Role> _roleRepository;

        public IdentityInitializer(
            TokenConfigurations tokenConfigurations,
            APISecurityDbContext context,
            IMediator mediator,
            IRepository<Models.Role> roleRepository)
        {
            _tokenConfigurations = tokenConfigurations;
            _context = context;
            _mediator = mediator;
            _roleRepository = roleRepository;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                _roleRepository.Add(new Models.Role(_tokenConfigurations.AccessRole));

                var user1 = new UserWithRolesRequest(
                    userName: "admin_apicontagem",
                    email: "admin-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "AdminAPIContagem01!",
                    new System.Collections.Generic.List<string>{
                        _tokenConfigurations.AccessRole
                    }
                );

                var user2 = new UserRequest(
                    userName: "usrinvalido_apicontagem",
                    email: "usrinvalido-apicontagem@teste.com.br",
                    emailConfirmed: true,
                    password: "UsrInvAPIContagem01!"
                );

                _ = _mediator.Send(user1).Result;
                _ = _mediator.Send(user2).Result;
            }
        }
    }
}
