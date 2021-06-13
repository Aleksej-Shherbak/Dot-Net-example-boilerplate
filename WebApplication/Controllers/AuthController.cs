using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Security.Auth;
using ServicesModels.Security.Auth;
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

            return registrationResult.IsSuccessful
                ? Ok(new
                {
                    Token = registrationResult.AccessToken
                })
                : BadRequest(new
                {
                    registrationResult.Errors,
                    ErrorCode = registrationResult.ErrorCode.ToString()
                });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var loginResult = await _authService.LoginAsync(new LoginInput
            {
                Email = model.Email,
                Password = model.Password
            });

            return loginResult.IsSuccessful
                ? Ok(new
                {
                    Token = loginResult.AccessToken
                })
                : BadRequest(new
                {
                    Message = loginResult.Message,
                    ErrorCode = loginResult.ErrorCode.ToString()
                });
        }
    }
}