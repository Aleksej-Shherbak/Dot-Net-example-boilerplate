using System;
using System.Threading.Tasks;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Seeding.Seeding.Abstract;
using Seeding.Settings;

namespace Seeding.Seeding
{
    public class SuperAdminSeeder: SeederBase
    {
        public override string SeederName => nameof(SuperAdminSeeder);

        public override async Task<bool> SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var superAdminOptions = serviceProvider.GetService<IOptions<SuperAdminSettings>>().Value;
            var userManager = serviceProvider.GetService<UserManager<User>>();

            var user = new User
            {
                Email = superAdminOptions.Email,
                UserName = superAdminOptions.Email,
            };
            
            var res = await userManager.CreateAsync(user, superAdminOptions.Password);
            if (res.Succeeded)
            {
                var currentUser = await userManager.FindByEmailAsync(user.Email);
                await userManager.AddToRoleAsync(currentUser, superAdminOptions.Role);
                return true;
            }

            return false;

        }
    }
}