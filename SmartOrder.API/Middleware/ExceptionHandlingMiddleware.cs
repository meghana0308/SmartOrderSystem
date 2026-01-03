using SmartOrder.API.Services;
using System.Net;
using System.Text.Json;
using SmartOrder.API.Helpers;

namespace SmartOrder.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;
                var result = JsonSerializer.Serialize(new { message = ex.Message });
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };



            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = new
                {
                    message = exception.Message,
                    type = exception.GetType().Name,
                    status = context.Response.StatusCode
                }
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
