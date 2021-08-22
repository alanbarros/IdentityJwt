using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityJwt.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddControllers();

            return services;
        }
    }
}