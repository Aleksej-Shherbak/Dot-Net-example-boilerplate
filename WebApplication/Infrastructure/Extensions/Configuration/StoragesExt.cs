using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Infrastructure.Extensions.Configuration
{
    public static class StoragesExt
    {
        public static IServiceCollection AddStorages(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ConnectionStringBlogDb"));
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "SampleInstance";
            });
            
            return services;
        }
    }
}