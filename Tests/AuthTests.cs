using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Seeding.Helpers;
using Services.Security.JwtToken;
using ServicesModels;
using ServicesModels.Security.Auth.Enums;
using ServicesModels.Security.JwtToken;
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
            UserHelper.CreateAdmin(Server.Services);
            var loginModel = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/login");

            request.Content = new StringContent(JsonSerializer.Serialize(loginModel, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await Client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var loginResponse =
                JsonSerializer.Deserialize<LoginResponse>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.IsNotEmpty(loginResponse.AccessToken);
            Assert.IsNotEmpty(loginResponse.RefreshToken);
        }

        [Test]
        [Description("User gets 401 if invalid credentials given")]
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

            request.Content = new StringContent(JsonSerializer.Serialize(loginModel, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await Client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var loginResponse =
                JsonSerializer.Deserialize<ErrorResponseBase>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(loginResponse.ErrorCode, LoginErrorReasons.EmailNotFound.ToString());
        }

        [Test]
        [Description("User can't refresh token if the refresh token is expired")]
        public async Task UserCantRefreshWithExpiredRefresh()
        {
            // Arrange
            await RefreshDbAsync();
            var user = UserHelper.CreateAdmin(Server.Services);
            var jwtService = Server.Services.GetRequiredService<JwtTokenService>();
            var tokensPair = await jwtService.GenerateTokensPairAsync(user, 0, 0);

            var model = new RefreshRequest()
            {
                Token = tokensPair.RefreshToken
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/refresh");

            request.Content = new StringContent(JsonSerializer.Serialize(model, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var httpResponseMessage = await Client.SendAsync(request);
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var response =
                JsonSerializer.Deserialize<ErrorResponseBase>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreEqual(response.Message, RefreshTokenInabilityReasons.Expired.ToString());
        }

        [Test]
        [Description("User can refresh token")]
        public async Task UserCanRefreshToken()
        {
            // Arrange
            await RefreshDbAsync();
            var user = UserHelper.CreateAdmin(Server.Services);
            var jwtService = Server.Services.GetRequiredService<JwtTokenService>();
            var tokensPair = await jwtService.GenerateTokensPairAsync(user);

            var model = new RefreshRequest()
            {
                Token = tokensPair.RefreshToken
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/refresh");

            request.Content = new StringContent(JsonSerializer.Serialize(model, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var httpResponseMessage = await Client.SendAsync(request);
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<LoginResponse>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreNotEqual(tokensPair.RefreshToken, response.RefreshToken);
        }

        [Test]
        [Description("User can refresh token only once")]
        public async Task UserCanRefreshTokenOnlyOnce()
        {
            // Arrange
            await RefreshDbAsync();
            var user = UserHelper.CreateAdmin(Server.Services);
            var jwtService = Server.Services.GetRequiredService<JwtTokenService>();
            var tokensPair = await jwtService.GenerateTokensPairAsync(user);

            var model = new RefreshRequest()
            {
                Token = tokensPair.RefreshToken
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/refresh");
            var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/refresh");

            request.Content = new StringContent(JsonSerializer.Serialize(model, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");
            secondRequest.Content = new StringContent(JsonSerializer.Serialize(model, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            await Client.SendAsync(request);
            var httpResponseMessage = await Client.SendAsync(secondRequest);
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<BaseResponse>(responseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreEqual(response.Message, RefreshTokenInabilityReasons.Used.ToString());
        }

        [Test]
        [Description("Refresh tokens become invalid after logout")]
        public async Task RefreshTokensBecomeInvalidAfterLogout()
        {
            // Arrange
            await RefreshDbAsync();
            UserHelper.CreateAdmin(Server.Services);
            var loginModel = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/login");

            request.Content = new StringContent(JsonSerializer.Serialize(loginModel, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await Client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var loginResponse =
                JsonSerializer.Deserialize<LoginResponse>(responseString, CamelCaseJsonSerializationOption);
            var refreshToken = loginResponse.RefreshToken;
            var accessToken = loginResponse.AccessToken;
            var requestLogout = new HttpRequestMessage(HttpMethod.Get, "/logout");
            Client.DefaultRequestHeaders.Clear();
            requestLogout.Headers.Authorization =  new AuthenticationHeaderValue("Bearer", accessToken);
            var logoutRes = await Client.SendAsync(requestLogout);
            
            // Act
            var model = new RefreshRequest()
            {
                Token = refreshToken
            };
            var requestRefresh = new HttpRequestMessage(HttpMethod.Post, "/refresh");

            requestRefresh.Content = new StringContent(JsonSerializer.Serialize(model, CamelCaseJsonSerializationOption),
                Encoding.Default, "application/json");

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var httpResponseMessage = await Client.SendAsync(requestRefresh);
            var responseStringRefresh = await httpResponseMessage.Content.ReadAsStringAsync();
            var responseRefresh = JsonSerializer.Deserialize<BaseResponse>(responseStringRefresh, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreEqual(responseRefresh.Status, HttpStatusCode.Unauthorized);
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
            // Arrange
            await RefreshDbAsync();
            UserHelper.CreateAdmin(Server.Services);
            var firstLoginModel = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword
            };

            var secondLoginModel = new LoginRequest
            {
                Email = AdminEmail,
                Password = AdminPassword
            };

            var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
            {
                Content = new StringContent(JsonSerializer.Serialize(firstLoginModel, CamelCaseJsonSerializationOption),
                    Encoding.Default, "application/json")
            };

            var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(secondLoginModel, CamelCaseJsonSerializationOption),
                    Encoding.Default, "application/json")
            };

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var firstResponse = await Client.SendAsync(firstRequest);
            var firstResponseString = await firstResponse.Content.ReadAsStringAsync();
            var firstLoginResponse =
                JsonSerializer.Deserialize<LoginResponse>(firstResponseString, CamelCaseJsonSerializationOption);

            var secondResponse = await Client.SendAsync(secondRequest);
            var secondResponseString = await secondResponse.Content.ReadAsStringAsync();
            var secondLoginResponse =
                JsonSerializer.Deserialize<LoginResponse>(secondResponseString, CamelCaseJsonSerializationOption);

            // Assert
            Assert.AreNotEqual(firstLoginResponse.RefreshToken, secondLoginResponse.RefreshToken);
        }
    }
}