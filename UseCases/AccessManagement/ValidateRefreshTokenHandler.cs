using System;

namespace IdentityJwt.UseCases.AccessManagement
{
    public interface IValidateRefreshToken
    {
        bool Validate(Guid refreshToken);
    }



    public class ValidateRefreshTokenHandler : IValidateRefreshToken
    {
        public bool Validate(Guid refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}