namespace Infrastructure.Options
{
    public class TokenParameters
    {
        public bool ValidateIssuer { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool RequireSignedTokens { get; set; }
        public bool RequireAudience { get; set; }
        public bool SaveSigninToken { get; set; }
        public bool TryAllIssuerSigningKeys { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateActor { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateTokenReplay { get; set; }
    }
}