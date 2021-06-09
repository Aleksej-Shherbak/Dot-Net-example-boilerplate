namespace ServicesModels.Security.Auth.Enums
{
    /// <summary>
    /// Short login error information for frontend
    /// </summary>
    public enum LoginErrorReasons
    {
        EmailNotFound,
        
        /// <summary>
        /// Unable to authorize by some reason that is not important for frontend.
        /// Just unauthorized. That's all.
        /// </summary>
        Unauthorized
    }
}