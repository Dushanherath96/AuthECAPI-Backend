using AuthECAPI.Models;
using Microsoft.EntityFrameworkCore;

//addDbContext is extension method for inject AppDbContext with SQL Server
namespace AuthECAPI.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(this IServiceCollection services,
            IConfiguration config) 
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DevDB")));
            return services;
        }
    }
}
