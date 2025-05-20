using Application.Common.Enums;
using StackExchange.Redis;

namespace Infrastructure.Exceptions
{
    public static class RedisExceptionExtensions
    {
        public static InfrastructureException ToInfrastructureException(this Exception ex)
        {
            string message = "Ha ocurrido un error interno. Por favor, inténtalo nuevamente más tarde.";
            HttpStatusCode statusCode;
            switch (ex)
            {
                case RedisConnectionException:
                    statusCode = HttpStatusCode.ServiceUnavailable;
                        break;
                case RedisTimeoutException:
                    statusCode = HttpStatusCode.GatewayTimeOut;
                    break;
                case RedisServerException:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            return new InfrastructureException(statusCode, message, ex);
        }
    }
}
