using IdentityJwt.Extensions;
using IdentityJwt.Infra.Data;
using IdentityJwt.Middleware;
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
                .AddAutoMapper()
                .AddJwtSecurity(CommonExtensions.TokenConfigurations(Configuration))
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

            // Cria????o de estruturas, usu??rios e permiss??es
            // na base do ASP.NET Identity Core (caso ainda n??o
            // existam)
            identityInitializer.Initialize();

            // Habilita o uso de autoriza????o para acesso a m??todos
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
