using System.Net.Http;
using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WebApplication;

namespace Tests
{
    public class BaseTest
    {
        protected const string AdminEmail = "admin@admin.com";
        protected const string AdminPassword = "password";

        protected TestServer _server;
        protected HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>().UseEnvironment("Testing"));
            _client = _server.CreateClient();
        }
        
        protected void RefreshDb()
        {
            var applicationDbContext = _server.Services.GetService<ApplicationDbContext>();
            applicationDbContext.Database.ExecuteSqlRaw(
                $"DROP DATABASE IF EXISTS {applicationDbContext.Database.ProviderName}");
            
            applicationDbContext.Database.ExecuteSqlRaw(
                $"CREATE DATABSE {applicationDbContext.Database.ProviderName}");
            applicationDbContext.Database.Migrate();
        }
    }
}