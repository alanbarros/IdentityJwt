using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityJwt.Extensions
{
    public static class ConfigExtensions
    {
        public static IHostBuilder ConfigureConfigurations(this IHostBuilder builder) =>
            builder.ConfigureAppConfiguration((builderContext, config) =>
                config.SetBasePath(builderContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                    .AddEnvironmentVariables());

        public static IHostBuilder ConfigureDotNetLogger(this IHostBuilder builder) =>
            builder.ConfigureLogging((builderContext, loggingBuilder) =>
            {
                loggingBuilder.AddConfiguration(builderContext.Configuration.GetSection("Logging"));

                loggingBuilder.AddSimpleConsole((options) =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                    options.IncludeScopes = Convert
                        .ToBoolean(builderContext.Configuration["Logging:IncludeScopes"]);
                });
            });
    }
}