using System.Text;
using Microsoft.IdentityModel.Tokens;
using Services.Security.JwtToken.Options;

namespace Services.Security.JwtToken.Mapping
{
    public static class JwtOptionsMapper
    {
        public static TokenValidationParameters MapToTokenValidationParameters(this JwtAuthOptions source)
        {
            return new()
            {
                ValidateIssuer = source.TokenParameters.ValidateIssuer,
                ValidIssuer = source.Issuer,
                
                RequireAudience = source.TokenParameters.RequireAudience,
                ValidateAudience = source.TokenParameters.ValidateAudience,
                ValidAudience =  source.Audience,
                
                ValidateLifetime = source.TokenParameters.ValidateLifetime,
               
                ValidateIssuerSigningKey = source.TokenParameters.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(source.Secret)),
                
                RequireExpirationTime = source.TokenParameters.RequireExpirationTime,
                RequireSignedTokens = source.TokenParameters.RequireSignedTokens,
                SaveSigninToken = source.TokenParameters.SaveSigninToken,
                TryAllIssuerSigningKeys = source.TokenParameters.TryAllIssuerSigningKeys,
                ValidateActor = source.TokenParameters.ValidateActor,
                ValidateTokenReplay = source.TokenParameters.ValidateTokenReplay,
            };
        }
    }
}