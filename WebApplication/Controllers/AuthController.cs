using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Security.Auth;
using ServicesModels;
using ServicesModels.Security.Auth;
using WebApplication.Models.Auth;
using WebApplication.Models.Http;

namespace WebApplication.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var registrationResult = await _authService.RegisterAsync(new RegisterInput
            {
                Email = model.Email,
                Password = model.Password,
                UserName = model.UserName
            });

            if (registrationResult.IsSuccessful)
            {
                return Ok(new LoginResponse
                {
                    AccessToken = registrationResult.AccessToken,
                    RefreshToken = registrationResult.RefreshToken,
                });
            }

            return Unauthorized(new
            {
                registrationResult.Errors,
                ErrorCode = registrationResult.ErrorCode.ToString()
            });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var loginResult = await _authService.LoginAsync(new LoginInput
            {
                Email = model.Email,
                Password = model.Password
            });

            if (loginResult.IsSuccessful)
            {
                return Ok(new LoginResponse()
                {
                    AccessToken = loginResult.AccessToken,
                    RefreshToken = loginResult.RefreshToken
                });
            }

            return Unauthorized(new  ErrorResponseBase
            {
                Message = loginResult.Message,
                ErrorCode = loginResult.ErrorCode.ToString()
            });
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            var refreshResult = await _authService.Refresh(request.Token);

            if (refreshResult.IsSuccessfully)
            {
                return Ok(new LoginResponse
                {
                    AccessToken = refreshResult.AccessToken,
                    RefreshToken = refreshResult.RefreshToken
                });
            }

            return Unauthorized(new BaseResponse
            {
                Status = HttpStatusCode.Unauthorized,
                Message = refreshResult.TokenInabilityReasons.ToString()
            });
        }
        
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(HttpContext.User);

            return Ok();
        }
    }
}