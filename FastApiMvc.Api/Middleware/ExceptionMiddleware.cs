using FastApiMvc.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace FastApiMvc.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case BusinessException e:
                        response.StatusCode = e.StatusCode;
                        break;
                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        _logger.LogError(error, error.Message);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new
                {
                    statusCode = response.StatusCode,
                    message = error.Message
                });

                await response.WriteAsync(result);
            }
        }
    }
}
