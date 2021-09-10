using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Data;
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
        protected TestServer Server { get; private set; }
        protected HttpClient Client { get; private set; }

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

            Server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseSetting("ConnectionStrings:ConnectionStringBlogDbTest", connectionString)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                ));
            Client = Server.CreateClient();
        }

        protected async Task RefreshDbAsync()
        {
            var applicationDbContext = Server.Services.GetService<ApplicationDbContext>();
            await applicationDbContext.Database.ExecuteSqlRawAsync(
                "DROP SCHEMA public CASCADE; CREATE SCHEMA public;");

            applicationDbContext.Database.Migrate();
        }
    }
}