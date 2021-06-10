using System.Text.Json;
using Data;
using Domains;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services.Security.Auth;
using Services.Security.JwtToken;
using Services.Security.JwtToken.Mapping;
using Services.Security.JwtToken.Options;
using WebApplication.Infrastructure;

namespace WebApplication
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var authSection = _configuration.GetSection("Auth");
            services.Configure<JwtAuthOptions>(authSection);

            var passwordSection = _configuration.GetSection("PasswordOptions");
            services.Configure<AuthPasswordOptions>(passwordSection);

            var databaseConnectionString = _configuration.GetConnectionString("ConnectionStringBlogDb");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(databaseConnectionString, b
                    => b.MigrationsAssembly("WebApplication"));
            });

            services.AddConnections();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TodoApp", Version = "v1"});
                c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                // TODO понять, для чего это
                //   c.OperationFilter<AuthResponsesOperationFilter>();
            });

            var authPasswordOptions = passwordSection.Get<AuthPasswordOptions>();
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = authPasswordOptions.RequireDigit;
                options.Password.RequiredLength = authPasswordOptions.RequiredLength;
                options.Password.RequireLowercase = authPasswordOptions.RequireLowercase;
                options.Password.RequireUppercase = authPasswordOptions.RequireUppercase;
                options.Password.RequireNonAlphanumeric = authPasswordOptions.RequireNonAlphanumeric;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            var jwtOptions = authSection.Get<JwtAuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = jwtOptions.MapToTokenValidationParameters();
                    options.Events = JwtEventsFactory.Create();
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddControllers().ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = ErrorResponseGenerator.ErrorResponse;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            
            services.AddScoped<JwtTokenService>();
            services.AddScoped<AuthService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}