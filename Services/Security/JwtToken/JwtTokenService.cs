using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Security.JwtToken.Options;

namespace Services.Security.JwtToken
{
    public class JwtTokenService
    {
        private readonly IOptions<JwtAuthOptions> _authOptions;
        private readonly UserManager<User> _userManager;

        public JwtTokenService(IOptions<JwtAuthOptions> authOptions, UserManager<User> userManager)
        {
            _authOptions = authOptions;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtAsync(User user)
        {
            var authParams = _authOptions.Value;

            var secretKey = GetSymmetricSecurityKey(authParams.Secret);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var token = new JwtSecurityToken(authParams.Issuer, authParams.Audience, claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLiveTimeSeconds), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}