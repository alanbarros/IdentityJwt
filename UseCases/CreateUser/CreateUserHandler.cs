using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityJwt.Models;
using IdentityJwt.Repository;
using MediatR;

namespace IdentityJwt.UseCases.CreateUser
{
    public class CreateUserHandler : IRequestHandler<UserRequest, User>, IRequestHandler<UserWithRolesRequest, User>
    {
        private readonly IRepository<User> userRepository;

        public CreateUserHandler(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> Handle(UserRequest request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => CreateUser(request));
        }

        public async Task<User> Handle(UserWithRolesRequest request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => CreateUser(request));
        }

        private User CreateUser(UserRequest userRequest)
        {
            var user = userRequest.UserBuilder();

            if (userRepository.Add(user) >= 1)
                return user;

            return null;
        }
    }

    public class UserRequest : IRequest<User>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }

        public UserRequest(string userName, string email, bool emailConfirmed, string password)
        {
            UserName = userName;
            Email = email;
            EmailConfirmed = emailConfirmed;
            Password = password;
        }

        public User UserBuilder(List<string> roles) => new(
            System.Guid.NewGuid(),
            this.UserName, 
            this.Email, 
            this.EmailConfirmed, 
            this.Password, 
            roles);

        public virtual User UserBuilder() => UserBuilder(new List<string>());
    }

    public class UserWithRolesRequest : UserRequest, IRequest<User>
    {
        public List<string> Roles { get; }

        public UserWithRolesRequest(string userName, string email,
            bool emailConfirmed, string password, List<string> roles)
            : base(userName, email, emailConfirmed, password)
        {
            Roles = roles;
        }

        public override User UserBuilder() => base.UserBuilder(this.Roles);
    }
}