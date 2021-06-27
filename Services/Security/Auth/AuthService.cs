using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Security.JwtToken;
using ServicesModels.Security.Auth;
using ServicesModels.Security.Auth.Enums;
using ServicesModels.Security.JwtToken;

namespace Services.Security.Auth
{
    public class AuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _dbContext;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager,
            JwtTokenService jwtTokenService, ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _dbContext = dbContext;
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

            var token = await _jwtTokenService.GenerateTokensPairAsync(user);

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

            var token = await _jwtTokenService.GenerateTokensPairAsync(user);

            return new RegisterOutput
            {
                IsSuccessful = true,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }

        public Task<RefreshTokenOutput> Refresh(string token)
        {
            return _jwtTokenService.RefreshToken(token);
        }

        public async Task LogoutAsync(ClaimsPrincipal userClaimsPrincipal)
        {
            if (userClaimsPrincipal == null)
            {
                return;
            }

            var user = await _userManager.GetUserAsync(userClaimsPrincipal);

            if (user == null)
            {
                return;
            }

            var refreshTokens = await _dbContext.RefreshTokens.Where(x => x.User.Id == user.Id).ToArrayAsync();

            _dbContext.RefreshTokens.RemoveRange(refreshTokens);

            await _dbContext.SaveChangesAsync();
            
            // TODO add access token to Redis black list (as invalid) 
        }
    }
}