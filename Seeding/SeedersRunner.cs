using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Seeding.Seeding;
using Seeding.Seeding.Abstract;

namespace Seeding
{
    public class SeedersRunner
    {
        private IServiceProvider ServiceProvider { get; set; }
        private IConfiguration Configuration { get; set; }
        private ILogger<SeedersRunner> Logger { get; set; }

        public SeedersRunner(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<SeedersRunner> logger)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            Logger = logger;
        }

        public async Task RunAsync()
        {
            // Seeders go here 
            var seeders = new List<ISeeder>
            {
                new RolesSeeder(),
                new SuperAdminSeeder(),
            };

            foreach (var seeder in seeders)
            {
                Logger.LogInformation($"Running {seeder.SeederName}");
                await seeder.SeedAsync(ServiceProvider, Configuration);
            }
        }
    }
}