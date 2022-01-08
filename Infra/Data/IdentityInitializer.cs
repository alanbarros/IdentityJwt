using IdentityJwt.Infra.Repository;
using IdentityJwt.Models;
using IdentityJwt.UseCases.CreateUser;
using MediatR;
using System.Collections.Generic;

namespace IdentityJwt.Infra.Data
{
    public class IdentityInitializer
    {
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly APISecurityDbContext _context;
        private readonly IMediator _mediator;
        private readonly IRoleRepository _roleRepository;
        private readonly INotifications _notifications;

        public IdentityInitializer(
            TokenConfigurations tokenConfigurations,
            APISecurityDbContext context,
            IMediator mediator,
            IRoleRepository roleRepository,
            INotifications notifications)
        {
            _tokenConfigurations = tokenConfigurations;
            _context = context;
            _mediator = mediator;
            _roleRepository = roleRepository;
            _notifications = notifications;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated() == false)
                throw new System.Exception("Falha ao criar banco de dados");

            _roleRepository.Add(new Role(_tokenConfigurations.AccessRole));

            List<User> users = new()
            {
                new(
                    "admin_apicontagem",
                    "admin-apicontagem@teste.com.br",
                    true,
                    "AdminAPIContagem01!",
                    _tokenConfigurations.AccessRole),

                new("usrinvalido_apicontagem",
                    "usrinvalido-apicontagem@teste.com.br",
                    true,
                    "UsrInvAPIContagem01!")
            };

            users.ForEach(user =>
            {
                UserRequest request = new(user);
                _mediator.Send(request).Result.MatchNone(() =>
                    throw new System.Exception($"Falha ao adicionar user, erros: {_notifications.SerializedNotifications}"));
            });
        }
    }
}
