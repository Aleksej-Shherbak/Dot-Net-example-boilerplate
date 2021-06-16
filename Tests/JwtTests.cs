using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApplication.Models.Auth;

namespace Tests
{
    public class JwtTests : BaseTest
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
            // Arrange
            RefreshDb();
            var request = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword,
            };

            // Act
            var response = await _client.PostAsync("/login", new StringContent(JsonSerializer.Serialize(request)));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseString);
            
            // Assert
            Assert.IsNotEmpty(loginResponse.AccessToken);
            Assert.IsNotEmpty(loginResponse.RefreshToken);
        }
    }
}