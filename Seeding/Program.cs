using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seeding.Seeding;
using Seeding.Settings;
using Infrastructure.Ext;


namespace Seeding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            var services = new ServiceCollection();
            
            services.Configure<SuperAdmin>(configuration.GetSection(nameof(SuperAdmin)));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ConnectionStringBlogDb"));
            });
            services.AddProjectIdentity(configuration);
            
            // Seeders go here 
            var seedingTasks = new List<ISeeder>
            {
                new SuperAdminSeeder(),
            }.Select(x => x.SeedAsync(services.BuildServiceProvider(), configuration));

            await Task.WhenAll(seedingTasks);
        }
    }
}