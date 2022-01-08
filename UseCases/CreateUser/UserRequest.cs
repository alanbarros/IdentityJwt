using IdentityJwt.Models;
using MediatR;
using Optional;

namespace IdentityJwt.UseCases.CreateUser
{
    public class UserRequest : IRequest<Option<StatusLevel>>
    {
        public User User { get; set; }

        public UserRequest(User user)
        {
            User = user;
        }
    }
}