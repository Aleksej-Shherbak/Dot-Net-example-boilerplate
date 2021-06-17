using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Seeding.Helpers;
using ServicesModels.Security.Auth.Enums;
using WebApplication.Models.Auth;
using WebApplication.Models.Http;

namespace Tests
{
    public class AuthTests : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            base.Setup();
        }

        [Test]
        [Description("User can login")]
        public async Task UserCanLogin()
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

        [Test]
        [Description("User get 401 if invalid credentials given")]
        public async Task UserCantLoginWithInvalidCredentials()
        {
            // Arrange
            await RefreshDbAsync();
            var loginModel = new LoginRequest
            {
                Email = "error@error.com",
                Password = "kek"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/login");
                        
            request.Content = new StringContent(JsonSerializer.Serialize(loginModel, CamelCaseJsonSerializationOption), Encoding.Default, "application/json");
           
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<ErrorResponseBase>(responseString, CamelCaseJsonSerializationOption);
            
            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(loginResponse.ErrorCode, LoginErrorReasons.EmailNotFound.ToString());
        }

        [Test]
        [Description("User get Unauthorized if access token is expired")]
        public async Task UserGetUnauthorizedIfAccessTokenExpired()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        [Description("User can refresh token")]
        public async Task UserCanRefreshToken()
        {
            throw new NotImplementedException();
        }

        [Test] [Description("User can user refresh token only once")]
        public async Task UserCanRefreshTokenOnlyOnce()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        [Description("Refresh tokens become invalid after logout")]
        public async Task RefreshTokensBecomeInvalidAfterLogout()
        {
            // TODO after logout store access token in Redis (like a black list). Expiration should be equals access token live time 
            throw new NotImplementedException();
        }
        
        [Test]
        [Description("Access tokens become invalid after logout")]
        public async Task AccessTokensBecomeInvalidAfterLogout()
        {
            // TODO after logout store access token in Redis (like a black list). Expiration should be equals access token live time 
            throw new NotImplementedException();
        }
        
        [Test]
        [Description("It's possible to use multiple refresh tokens")]
        public async Task PossibleToLoginFromSeveralPlacesSimultaneously()
        {
            throw new NotImplementedException();
        }
    }
}