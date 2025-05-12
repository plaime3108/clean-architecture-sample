
using Application.Common.Enums;

namespace Application.Exceptions
{
    public class BridgeApplicationException : Exception
    {
        public int StatusCode { get; }
        public string? ErrorDetails { get; }

        public BridgeApplicationException(HttpStatusCode statusCode, string message, string? errorDetails = null) : base(message)
        {
            StatusCode = (int)statusCode;
            ErrorDetails = errorDetails;
        }
    }
}
