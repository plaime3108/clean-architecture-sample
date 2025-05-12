using Application.Common.Enums;

namespace Application.Responses
{
    public class ErrorResponse
    {
        public string? Message { get; }
        public string? ErrorDetails { get; }
        public HttpStatusCode StatusCode { get; }
        public ErrorResponse(string message, HttpStatusCode statusCode, string? errorDetails = null)
        {
            Message = message;
            StatusCode = statusCode;
            ErrorDetails = errorDetails;
        }
    }
}
