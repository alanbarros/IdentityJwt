using AutoMapper;
using IdentityJwt.Infra.Data;
using IdentityJwt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

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

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            IMapper mapper = new MapperConfiguration(cfg =>
            {
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.BaseType == typeof(Profile))
                    .ToList()
                    .ForEach(profile => cfg.AddProfile(profile));
            }).CreateMapper();

            services.AddSingleton(mapper);

            return services;
        }

        public static TokenConfigurations TokenConfigurations(IConfiguration configuration) =>
            configuration.GetSection("TokenConfigurations").Get<TokenConfigurations>();

    }
}