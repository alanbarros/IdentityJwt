using IdentityJwt.Security;
using IdentityJwt.Security.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityJwt.Extensions
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Ativando o uso de cache via Redis
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration =
                    configuration.GetConnectionString("ConexaoRedis");
                options.InstanceName = "Identity Tokens JWT";
            });

            return services;
        }

        public static IServiceCollection AddDataBase(
            this IServiceCollection services)
        {
            // Configurando o uso da classe de contexto para
            // acesso às tabelas do ASP.NET Identity Core
            services.AddDbContext<APISecurityDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDatabase"));

            return services;
        }

        public static IServiceCollection AddJwtTokens(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);

            // Aciona a extensão que irá configurar o uso de
            // autenticação e autorização via tokens
            services.AddJwtSecurity(tokenConfigurations);

            return services;
        }
    }
}