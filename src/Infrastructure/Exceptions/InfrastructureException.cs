using Application.Exceptions;
using Application.Common.Enums;

namespace Infrastructure.Exceptions
{
    public class InfrastructureException : BridgeApplicationException
    {
        public InfrastructureException(HttpStatusCode statusCode, string message, Exception? inner = null)
            : base(statusCode, message, $"Error Exception Infrastructure Layer: {inner?.Message}") { }
    }
}