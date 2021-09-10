using Data;
using Domains;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Security.Auth;
using Services.Security.JwtToken;
using Services.Security.JwtToken.Mapping;
using Services.Security.JwtToken.Options;
using PasswordOptions = Services.Security.JwtToken.Options.PasswordOptions;

namespace WebApplication.Infrastructure.Extensions.Configuration
{
    public static class SecurityExt
    {
        public static IServiceCollection AddSecurityExt(this IServiceCollection services, IConfiguration configuration)
        {
            var authPasswordOptions = configuration.GetSection(nameof(PasswordOptions)).Get<PasswordOptions>();
            services
                .AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.Password.RequireDigit = authPasswordOptions.RequireDigit;
                    options.Password.RequiredLength = authPasswordOptions.RequiredLength;
                    options.Password.RequireLowercase = authPasswordOptions.RequireLowercase;
                    options.Password.RequireUppercase = authPasswordOptions.RequireUppercase;
                    options.Password.RequireNonAlphanumeric = authPasswordOptions.RequireNonAlphanumeric;
                    options.User.RequireUniqueEmail = true;
                }).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<ApplicationDbContext>();

            var jwtOptions = configuration.GetSection(nameof(JwtAuthOptions)).Get<JwtAuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = jwtOptions.MapToTokenValidationParameters();
                    options.Events = JwtEventsFactory.Create();
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddScoped<JwtTokenService>();
            services.AddScoped<AuthService>();
            
            return services;
        }
    }
}