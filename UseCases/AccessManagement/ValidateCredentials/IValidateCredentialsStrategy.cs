using IdentityJwt.Security;

namespace IdentityJwt.UseCases.AccessManagement.ValidateCredentials
{
    public interface IValidateCredentialsStrategy
    {
        bool Validate(AccessCredentials credenciais);
    }
}
