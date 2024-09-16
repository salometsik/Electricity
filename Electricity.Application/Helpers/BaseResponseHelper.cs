using Electricity.Application.Models.Base;
using System.Net;

namespace Electricity.Application.Helpers
{
    public static class BaseResponseHelper
    {
        public static T HandleException<T>(this T response) where T : BaseResponse
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Success = false;
            response.Message = "Error saving changes";
            return response;
        }

        public static T HandleNoContent<T>(this T response) where T : BaseResponse
        {
            response.StatusCode = HttpStatusCode.NoContent;
            response.Success = true;
            response.Message = "Objects not found";
            return response;
        }
    }
}
