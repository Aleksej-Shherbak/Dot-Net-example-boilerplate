using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Infrastructure.Extensions.Configuration
{
    public static class WebExt
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = ErrorResponseGenerator.ErrorResponse;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            return services;
        }
    }
}