using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityJwt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                // .ConfigureLogging((builderContext, loggingBuilder) =>
                // {
                //     //loggingBuilder.AddConfiguration(builderContext.Configuration.GetSection("Logging"));

                //     loggingBuilder.AddSimpleConsole((options) =>
                //     {
                //         options.SingleLine = true;
                //         options.TimestampFormat = "hh:mm:ss ";
                //         options.IncludeScopes = Convert
                //             .ToBoolean(builderContext.Configuration["Logging:IncludeScopes"]);
                //     });
                // })                
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.SetBasePath(builderContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables();
                });
    }
}
