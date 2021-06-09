using ServicesModels.Security.Auth.Enums;

namespace ServicesModels.Security.Auth
{
    public class LoginOutput
    {
        public bool IsSuccessful { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public LoginErrorReasons ErrorCode { get; set; }
    }
}