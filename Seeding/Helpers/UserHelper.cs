using System;
using System.Linq;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Seeding.Helpers
{
    public static class UserHelper
    {
        public static User CreateAdmin(IServiceProvider serviceProvider, string adminRoleName = "admin",
            string adminEmail = "admin@admin.com", string adminPassword = "password", string adminUserName = "admin")
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var adminRole = roleManager.Roles.FirstOrDefault(x => x.Name == adminRoleName);

            if (adminRole == null)
            {
                roleManager.CreateAsync(new IdentityRole<int>(adminRoleName)).GetAwaiter().GetResult();
            }

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var adminsList = userMgr.GetUsersInRoleAsync(adminRoleName).GetAwaiter().GetResult();

            User admin = null;
            if (adminsList.Any() == false)
            {
                admin = new User
                {
                    EmailConfirmed = true,
                    UserName = adminUserName,
                    Email = adminEmail
                };

                userMgr.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
                userMgr.AddToRoleAsync(admin, adminRoleName).GetAwaiter().GetResult();

                Console.WriteLine("Super admin user has bean created!");
            }
            return admin;
        }
    }
}