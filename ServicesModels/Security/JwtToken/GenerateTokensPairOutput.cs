namespace ServicesModels.Security.JwtToken
{
    public class GenerateTokensPairOutput
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}