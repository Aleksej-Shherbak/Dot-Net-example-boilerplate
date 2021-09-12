using System.Threading.Tasks;
using Data;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Seeding.Settings;

namespace Seeding.Seeding
{
    public class SuperAdminSeeder: ISeeder
    {
        
        public async Task<bool> SeedAsync(ServiceProvider serviceProvider, IConfiguration configuration)
        {
            var superAdminOptions = serviceProvider.GetService<IOptions<SuperAdmin>>().Value;
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
            else
            {
                return false;
            }
            
        }
    }
}