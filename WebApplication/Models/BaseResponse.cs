using System.Net;

namespace WebApplication.Models
{
    public class BaseResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}