using System.Linq;
using System.Threading.Tasks;
using Domains;
using Microsoft.AspNetCore.Identity;
using Services.Security.JwtToken;
using ServicesModels.Security.Auth;
using ServicesModels.Security.Auth.Enums;

namespace Services.Security.Auth
{
    public class AuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenService _jwtTokenService;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager,
            JwtTokenService jwtTokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginOutput> LoginAsync(LoginInput model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new LoginOutput
                {
                    ErrorCode = LoginErrorReasons.EmailNotFound,
                    Message = $"Email {model.Email} not found"
                };
            }

            var signInRes = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (signInRes.Succeeded == false)
            {
                return new LoginOutput
                {
                    ErrorCode = LoginErrorReasons.Unauthorized,
                    Message = "Unauthorized"
                };
            }

            var token = await _jwtTokenService.GenerateTokensPair(user);

            return new LoginOutput
            {
                IsSuccessful = true,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
            };
        }
        
        public async Task<RegisterOutput> RegisterAsync(RegisterInput model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(x => x.Description);
                return new RegisterOutput
                {
                    ErrorCode = RegisterErrorReasons.UnableToRegister,
                    Errors = errors
                };
            }

            var token = await _jwtTokenService.GenerateTokensPair(user);

            return new RegisterOutput
            {
                IsSuccessful = true,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }
    }
}