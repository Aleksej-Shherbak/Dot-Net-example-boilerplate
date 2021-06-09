namespace Services.Security.JwtToken.Options
{
    public class JwtAuthOptions
    {
        /// <summary>
        /// Who has generated the token. 
        /// </summary>
        public string Issuer { get; set; }
        
        /// <summary>
        /// Who requires the token
        /// </summary>
        public string Audience { get; set; }
        
        /// <summary>
        /// A secret string that will used for generating key of symmetric cipher
        /// </summary>
        public string Secret { get; set; }
        
        public int TokenLiveTimeSeconds { get; set; }

        public TokenParameters TokenParameters { get; set; }
    }
}