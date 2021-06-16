using System.Threading.Tasks;
using Data;
using Domains;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.Security.Auth;
using Services.Security.JwtToken;
using Services.Security.JwtToken.Mapping;
using Services.Security.JwtToken.Options;

namespace Tests
{
    public class BaseTest
    {
        protected ServiceProvider ServiceProvider { get; set; }
        protected const string AdminEmail = "admin@admin.com";
        protected const string AdminPassword = "password";
        

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var authSection = configuration.GetSection("Auth");
            
            serviceCollection.Configure<JwtAuthOptions>(authSection);

            var passwordSection = configuration.GetSection("PasswordOptions");
            serviceCollection.Configure<AuthPasswordOptions>(passwordSection);

            var databaseConnectionString = configuration.GetConnectionString("ConnectionStringBlogDbTest");
            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(databaseConnectionString);
            });
            
            var authPasswordOptions = passwordSection.Get<AuthPasswordOptions>();
            serviceCollection
                .AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = authPasswordOptions.RequireDigit;
                options.Password.RequiredLength = authPasswordOptions.RequiredLength;
                options.Password.RequireLowercase = authPasswordOptions.RequireLowercase;
                options.Password.RequireUppercase = authPasswordOptions.RequireUppercase;
                options.Password.RequireNonAlphanumeric = authPasswordOptions.RequireNonAlphanumeric;
                options.User.RequireUniqueEmail = true;
            }).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<ApplicationDbContext>();

            var jwtOptions = authSection.Get<JwtAuthOptions>();
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = jwtOptions.MapToTokenValidationParameters();
                    options.Events = JwtEventsFactory.Create();
                });

            serviceCollection.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            
            serviceCollection.AddScoped<JwtTokenService>();
            serviceCollection.AddScoped<AuthService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected void ClearDatabase()
        {
            var applicationDbContext = ServiceProvider.GetService<ApplicationDbContext>();

            // applicationDbContext.Database.ExecuteSqlRaw("DROP DATABASE ")
        }

        protected void RunMigrations()
        {
        }


    }
}