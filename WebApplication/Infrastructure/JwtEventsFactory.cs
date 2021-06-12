using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using WebApplication.Models;

namespace WebApplication.Infrastructure
{
    public static class JwtEventsFactory
    {
        public static JwtBearerEvents Create()
        {
            return new ()
            {
                OnChallenge = async context =>
                {
                    // Call this to skip the default logic and avoid using the default response
                    context.HandleResponse();

                    // Write to the response in any way you wish here
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Headers.Append("Content-type", "application/json");
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new BaseResponse
                    {
                        Message = "Unauthorized",
                        Status = HttpStatusCode.Unauthorized
                    }, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }));
                }
            };
        }
    }
}