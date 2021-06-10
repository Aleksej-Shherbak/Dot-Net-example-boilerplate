namespace WebApplication.Models
{
    public class Response<T>: BaseResponse
    {
        public T Data { get; set; }
    }
}