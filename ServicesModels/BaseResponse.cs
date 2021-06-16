using System.Net;

namespace ServicesModels
{
    public class BaseResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}