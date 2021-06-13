using System.Collections.Generic;
using ServicesModels.Security.Auth.Enums;

namespace ServicesModels.Security.Auth
{
    public class RegisterOutput
    {
        public string AccessToken { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public RegisterErrorReasons ErrorCode { get; set; }
        public string RefreshToken { get; set; }
    }
}