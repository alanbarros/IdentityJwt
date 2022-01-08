using IdentityJwt.Models;
using Optional;

namespace IdentityJwt.Infra.Repository
{
    public interface IUserRepository
    {
        StatusLevel Add(User user);
        Option<User> FindByName(string name); 
    }
}