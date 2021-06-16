using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Seeding.Helpers;
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
            await RefreshDbAsync();
            UserHelper.CreateAdmin(_server.Services);
            var loginModel = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/login");
                        
            request.Content = new StringContent(JsonSerializer.Serialize(loginModel, CamelCaseJsonSerializationOption), Encoding.Default, "application/json");
           
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.IsNotEmpty(loginResponse.AccessToken);
            Assert.IsNotEmpty(loginResponse.RefreshToken);
        }
    }
}