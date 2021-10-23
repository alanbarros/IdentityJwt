using IdentityJwt.Extensions;
using IdentityJwt.Middleware;
using IdentityJwt.Security.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace IdentityJwt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRedisCache(Configuration)
                .AddDataBase()
                .AddJwtTokens(Configuration)
                .AddServices()
                .AddLogging((builder) =>
                {
                    builder.AddSerilog(dispose: true);
                })
                .AddSwashBuckle();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IdentityInitializer identityInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<ScopedLoggingMiddleware>();
            app.UseMiddleware<ScopedSerilogSpecificLoggingMiddleware>();

            app.UseSwashBuckle();

            app.UseCors(builder => builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));

            // Criação de estruturas, usuários e permissões
            // na base do ASP.NET Identity Core (caso ainda não
            // existam)
            identityInitializer.Initialize();

            // Habilita o uso de autorização para acesso a métodos
            // protegidos de API
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
