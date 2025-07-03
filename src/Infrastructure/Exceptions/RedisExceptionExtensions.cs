using Application.Common.Enums;
using StackExchange.Redis;

namespace Infrastructure.Exceptions
{
    public static class RedisExceptionExtensions
    {
        public static InfrastructureException ToInfrastructureException(this Exception ex)
        {
            string message = "Ha ocurrido un error interno. Por favor, inténtalo nuevamente más tarde.";
            var statusCode = ex switch
            {
                RedisConnectionException => HttpStatusCode.ServiceUnavailable,
                RedisTimeoutException => HttpStatusCode.GatewayTimeOut,
                RedisServerException => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError,
            };
            return new InfrastructureException(statusCode, message, ex);
        }
    }
}
