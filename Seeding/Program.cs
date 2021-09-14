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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;


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
            
            services.Configure<SuperAdminSettings>(configuration.GetSection(nameof(SuperAdminSettings)));
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ConnectionStringBlogDb"));
            });
            services.AddProjectIdentity(configuration);
            services.AddScoped<SeedersRunner>();
            services.AddSingleton(configuration);
            
            await services.BuildServiceProvider().GetService<SeedersRunner>().RunAsync();
        }
    }
}