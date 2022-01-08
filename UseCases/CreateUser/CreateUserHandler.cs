using IdentityJwt.Infra.Repository;
using IdentityJwt.Models;
using MediatR;
using Optional;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityJwt.UseCases.CreateUser
{
    public class CreateUserHandler : IRequestHandler<UserRequest, Option<StatusLevel>>
    {
        private readonly IUserRepository userRepository;

        public CreateUserHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Option<StatusLevel>> Handle(UserRequest request,
        CancellationToken cancellationToken) => userRepository.Add(request.User) switch
        {
            StatusLevel.FAIL => await Task.FromResult(Option.None<StatusLevel>()),
            _ => await Task.FromResult(StatusLevel.SUCCESS.Some())
        };
    }
}