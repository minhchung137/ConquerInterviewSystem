using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                await HandleExceptionAsync(context, ex.ErrorCode, HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context, AppErrorCode.InternalError, HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, AppErrorCode errorCode, HttpStatusCode statusCode)
        {
            var response = APIResponse<string>.Fail(errorCode, (ResponseStatus)(int)statusCode);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
