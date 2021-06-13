using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<GenerateTokensPairOutput> GenerateTokensPair(User user)
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
                expires: DateTime.UtcNow.AddSeconds(authParams.TokenLiveTimeSeconds),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var prevRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.User.Id == user.Id);

            if (prevRefreshToken != null)
            {
                _dbContext.Remove(prevRefreshToken);
            }

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRandomString()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            
            user.RefreshToken = refreshToken;
            
            await _dbContext.SaveChangesAsync();

            return new GenerateTokensPairOutput()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
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