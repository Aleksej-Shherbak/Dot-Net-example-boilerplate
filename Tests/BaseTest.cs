using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Data;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WebApplication;

namespace Tests
{
    public class BaseTest
    {
        protected const string AdminEmail = "admin@admin.com";
        protected const string AdminPassword = "password";
        protected JsonSerializerOptions CamelCaseJsonSerializationOption { get; set; }

        protected TestServer _server;
        protected HttpClient _client;

        [SetUp]
        public void Setup()
        {
            CamelCaseJsonSerializationOption = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("ConnectionStringBlogDbTest");
            
            // TODO make shared appsettings or just figure out how to set path to particular appsettings
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseSetting("ConnectionStrings:ConnectionStringBlogDbTest", connectionString)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                ));
            _client = _server.CreateClient();
        }

        protected async Task RefreshDbAsync()
        {
            // TODO move database name to appsettings or retrieve from the connection string 
            var applicationDbContext = _server.Services.GetService<ApplicationDbContext>();
            var res = await applicationDbContext.Database.ExecuteSqlRawAsync(
                $"DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
            
            applicationDbContext.Database.Migrate();
        }
    }
}