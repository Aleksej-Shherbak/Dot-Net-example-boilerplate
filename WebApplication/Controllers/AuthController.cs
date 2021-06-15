using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Security.Auth;
using ServicesModels.Security.Auth;
using ServicesModels.Security.JwtToken;
using WebApplication.Models;
using WebApplication.Models.Auth;

namespace WebApplication.Controllers
{
    [ApiController]
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

            return BadRequest(new
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

            return BadRequest(new
            {
                Message = loginResult.Message,
                ErrorCode = loginResult.ErrorCode.ToString()
            });
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([Required] string token)
        {
            var refreshResult = await _authService.Refresh(token);

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
    }
}