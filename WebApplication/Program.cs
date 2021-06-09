using System;
using System.Linq;
using Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CreateAdmin(host);
            host.Run();
        }

        private static void CreateAdmin(IHost host)
        {
            var adminRoleName = "admin";
            using var scope = host.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = roleManager.Roles.FirstOrDefault(x => x.Name == adminRoleName);

            if (adminRole == null)
            {
                roleManager.CreateAsync(new IdentityRole(adminRoleName)).GetAwaiter().GetResult();
            }

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var adminsList = userMgr.GetUsersInRoleAsync(adminRoleName).GetAwaiter().GetResult();

            if (adminsList.Any() == false)
            {
                var admin = new User
                {
                    EmailConfirmed = true,
                    UserName = "admin",
                    Email = "admin@admin.com"
                };

                userMgr.CreateAsync(admin, "password").GetAwaiter().GetResult();
                userMgr.AddToRoleAsync(admin, adminRoleName).GetAwaiter().GetResult();

                Console.WriteLine("Super admin user has bean created!");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}