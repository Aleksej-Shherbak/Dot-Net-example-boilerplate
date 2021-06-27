namespace ServicesModels.Security.JwtToken
{
    public class RefreshTokenOutput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSuccessfully { get; set; }
        public RefreshTokenInabilityReasons TokenInabilityReasons { get; set; }
    }

    public enum RefreshTokenInabilityReasons
    {
        Expired,
        NotFound,
        Used,
        UnknownReason
    }
}