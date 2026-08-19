using SupplierServiceNet.Application.Dtos.Response;
using SupplierServiceNet.CrossCutting.Exceptions;
using System.Net;
using System.Text.Json;

namespace SupplierServiceNet.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment hostEnvironment)
        {
            _next = next;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = exception.Message;

            switch (exception)
            {
                case AuthenticationException:
                    statusCode = HttpStatusCode.Unauthorized;
                    _logger.LogWarning("Authentication failed: {Message}", message);
                    break;

                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    _logger.LogWarning("Unauthorized access: {Message}", message);
                    break;

                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    _logger.LogWarning("Validation error: {Message}", message);
                    break;

                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    _logger.LogWarning("Not found: {Message}", message);
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception: {Message}", message);
                    message = _hostEnvironment.IsDevelopment() ? message : "An error occurred processing your request.";
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;

            var response = new ApiException(
                (int)statusCode,
                message,
                _hostEnvironment.IsDevelopment() ? exception.StackTrace : null);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return httpContext.Response.WriteAsync(json);
        }
    }
}
