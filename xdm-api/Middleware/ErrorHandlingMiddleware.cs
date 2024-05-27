using System;
using System.Net;
using System.Text.Json;

namespace xdm_api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 调用下一个中间件
                await _next(context);
            }
            catch (Exception ex)
            {
                // 4. 捕获异常并记录错误
                _logger.LogError(ex, ex.StackTrace);

                // 5. 生成并返回错误响应
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 默认状态码为500
            var response = context.Response;

            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;

                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;

                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;

                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    break;
            }
            var errorResponse = new
            {
                Message = "An unexpected error occurred.",
                Details = exception.Message,
                StackTrace = exception.StackTrace,
                code = response.StatusCode
            };

            return context.Response.WriteAsJsonAsync(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true

            });
        }


    }
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}

