using System.Collections.Generic;
using Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seeding.Seeding;

namespace Seeding
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var services = new ServiceCollection();
            
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectionStringBlogDb"));
            });
            
            // Seeders go here 
            var seeders = new List<ISeeder>
            {
                
            };
            
            seeders.ForEach(x => x.Seed(services));
        }
    }
}