using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Security.JwtToken.Options;
using ServicesModels.Security.JwtToken;

namespace Services.Security.JwtToken
{
    public class JwtTokenService
    {
        private readonly JwtAuthOptions _jwtOptions;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public JwtTokenService(IOptions<JwtAuthOptions> jwtOptions,
            UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public Task<GenerateTokensPairOutput> GenerateTokensPairAsync(User user)
        {
            return GenerateTokensPairAsync(user, _jwtOptions.AccessTokenLiveTimeSeconds,
                _jwtOptions.RefreshTokenLiveTimeSeconds);
        }

        public async Task<GenerateTokensPairOutput> GenerateTokensPairAsync(User user, int expirationAccessTokenSeconds,
            int expirationRefreshTokenSeconds)
        {
            var authParams = _jwtOptions;

            var secretKey = GetSymmetricSecurityKey(authParams.Secret);

            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var token = new JwtSecurityToken(
                authParams.Issuer,
                authParams.Audience, claims,
                expires: DateTime.UtcNow.AddSeconds(expirationAccessTokenSeconds),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddSeconds(expirationRefreshTokenSeconds),
                Token = GenerateRandomString()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);

            refreshToken.User = user;

            await _dbContext.SaveChangesAsync();

            return new GenerateTokensPairOutput()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<RefreshTokenOutput> RefreshToken(string refreshToken)
        {
            var refreshTokenDbRes = await _dbContext.RefreshTokens.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token.Equals(refreshToken));

            if (refreshTokenDbRes == null)
            {
                return new RefreshTokenOutput
                {
                    TokenInabilityReasons = RefreshTokenInabilityReasons.NotFound
                };
            }
            
            if (refreshTokenDbRes.IsUsed)
            {
                return new RefreshTokenOutput
                {
                    TokenInabilityReasons = RefreshTokenInabilityReasons.Used
                };
            }

            if (DateTime.UtcNow >= refreshTokenDbRes.ExpiryDate)
            {
                return new RefreshTokenOutput
                {
                    TokenInabilityReasons = RefreshTokenInabilityReasons.Expired
                };
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == refreshTokenDbRes.User.Id);

            if (user == null)
            {
                return new RefreshTokenOutput
                {
                    TokenInabilityReasons = RefreshTokenInabilityReasons.UnknownReason
                };
            }

            refreshTokenDbRes.IsUsed = true;

            await _dbContext.SaveChangesAsync();

            var tokenPair = await GenerateTokensPairAsync(user);

            return new RefreshTokenOutput
            {
                AccessToken = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                IsSuccessfully = true
            };
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }

        private string GenerateRandomString()
        {
            return (Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString()).Replace("-",
                String.Empty);
        }
    }
}