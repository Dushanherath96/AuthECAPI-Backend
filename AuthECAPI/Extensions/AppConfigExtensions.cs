using AuthECAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigCORS(this WebApplication app,
            IConfiguration config)
        {
            app.UseCors(options =>
            options.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
            return app;
        }

        public static IServiceCollection AppConfig(this IServiceCollection services,
            IConfiguration config)
        {
            //Configure AppSettings class with JWTSecret from appsettings.json
            services.Configure<AppSettings>(
                config.GetSection("AppSettings"));
            return services;
        }
    }
}
