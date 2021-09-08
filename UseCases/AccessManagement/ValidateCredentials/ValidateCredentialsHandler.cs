using IdentityJwt.Security;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityJwt.UseCases.AccessManagement.ValidateCredentials
{
    public class ValidateCredentialsHandler : IRequestHandler<AccessCredentials, bool>
    {
        private readonly IValidateCredentialsStrategy validateCredentials;

        public ValidateCredentialsHandler(IValidateCredentialsStrategy validateCredentials)
        {
            this.validateCredentials = validateCredentials;
        }

        public Task<bool> Handle(AccessCredentials request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (request == null && string.IsNullOrWhiteSpace(request.UserID))
                    return false;

                return validateCredentials.Validate(request);
            });
        }
    }
}