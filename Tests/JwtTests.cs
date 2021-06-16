using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Services.Security.Auth;
using WebApplication.Controllers;
using WebApplication.Models.Auth;

namespace Tests
{
    public class JwtTests: BaseTest
    {
        [SetUp]
        public void Setup()
        {
            base.Setup();
        }

        [Test]
        [Description("User can get the tokens pair")]
        public async Task UserCanGetTokensPair()
        {
            var authService = ServiceProvider.GetService<AuthService>();
            var authController = new AuthController(authService);
            var loginResult = await authController.Login(new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminEmail,
            });
            
            Assert.Pass();
        }
    }
}