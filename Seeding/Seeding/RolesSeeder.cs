using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seeding.Seeding.Abstract;

namespace Seeding.Seeding
{
    public class RolesSeeder: SeederBase
    {
        public string SeederName => nameof(RolesSeeder);
      
        public override async Task<bool> SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole<int>>>();

            var roles = configuration.GetSection("Roles")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray();
            
            foreach (var role in roles)
            {
                var res = await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = role,
                });

                if (!res.Succeeded)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}