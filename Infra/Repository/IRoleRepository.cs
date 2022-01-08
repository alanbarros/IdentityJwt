using IdentityJwt.Models;

namespace IdentityJwt.Infra.Repository
{
    public interface IRoleRepository
    {
        StatusLevel Add(Role role);
        bool Exists(string name); 
    }
}