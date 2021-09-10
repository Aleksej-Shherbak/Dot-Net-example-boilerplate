using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Security.JwtToken.Options;

namespace WebApplication.Infrastructure.Extensions.Configuration
{
    public static class OptionsExt
    {
        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtAuthOptions>(configuration.GetSection(nameof(JwtAuthOptions)));
            services.Configure<PasswordOptions>(configuration.GetSection(nameof(PasswordOptions)));
            return services;
        }
    }
}