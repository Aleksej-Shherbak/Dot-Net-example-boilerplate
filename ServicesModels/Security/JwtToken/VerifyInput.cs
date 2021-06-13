namespace ServicesModels.Security.JwtToken
{
    public class VerifyInput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}