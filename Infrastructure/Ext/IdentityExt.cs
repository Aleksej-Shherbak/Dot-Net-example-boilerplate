using Data;
using Domains;
using Infrastructure.Mapping;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Ext
{
    public static class IdentityExt
    {
        public static IServiceCollection AddProjectIdentity(this IServiceCollection services, IConfiguration configuration)
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
                   // options.Events = JwtEventsFactory.Create();
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            
            
            return services;
        }
    }
}