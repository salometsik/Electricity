using System.Net;

namespace Electricity.Application.Models.Base
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = true;
            StatusCode = HttpStatusCode.OK;
        }
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
