using Application.Exceptions;
using Application.Common.Enums;

namespace Infrastructure.Exceptions
{
    public class InfrastructureException(HttpStatusCode statusCode, string message, Exception? inner = null) :
        BridgeApplicationException(statusCode, message, $"Error Exception Infrastructure Layer: {inner?.Message}")
    {
    }
}