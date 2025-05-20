using Application.Exceptions;
using Web.API.CNF.Models.Responses;

namespace Web.API.CNF.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            exception = UnwrapException(exception);
            var (statusCode, errorMessage) = exception switch
            {
                BridgeApplicationException apiEx => (apiEx.StatusCode, apiEx.Message),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado."),
                ArgumentException => (StatusCodes.Status400BadRequest, "Solicitud inválida."),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado."),
                _ => (StatusCodes.Status500InternalServerError, "Ha ocurrido un error inesperado. Contacte al administrador.")
            };
            
            var response = new ErrorResponse
            {
                Error = errorMessage,
                Details = exception.Message
            };

            if (exception is BridgeApplicationException exWithDetails && !string.IsNullOrEmpty(exWithDetails.ErrorDetails))
            {
                response.Details = exWithDetails.ErrorDetails;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            
            return context.Response.WriteAsJsonAsync(response);
        }

        private static Exception UnwrapException(Exception ex)
        {
            if (ex is AggregateException aggEx && aggEx.InnerExceptions.Count == 1)
                return UnwrapException(aggEx.InnerExceptions[0]);
            
            return ex;
        }
    }
}
