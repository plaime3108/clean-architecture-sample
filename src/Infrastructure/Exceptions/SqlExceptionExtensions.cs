using Application.Common.Enums;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Exceptions
{
    public static class SqlExceptionExtensions
    {
        private static readonly Dictionary<int, HttpStatusCode> ErrorCodeMappings = new() {
            // Errores de conexión
            { 2, HttpStatusCode.ServiceUnavailable },
            { 53, HttpStatusCode.GatewayTimeOut },
            { 1326, HttpStatusCode.Unauthorized },
            { 17053, HttpStatusCode.InternalServerError },
            { 18456, HttpStatusCode.Unauthorized },

            // Base de datos
            { 4060, HttpStatusCode.Forbidden },
            { 9002, HttpStatusCode.InsufficientStorage },

            // Sintaxis y objetos
            { 102, HttpStatusCode.BadRequest },
            { 156, HttpStatusCode.BadRequest },
            { 207, HttpStatusCode.BadRequest },
            { 208, HttpStatusCode.BadRequest },

            // Restricciones
            { 547, HttpStatusCode.Conflict },
            { 2601, HttpStatusCode.Conflict },
            { 2627, HttpStatusCode.Conflict },
            { 8114, HttpStatusCode.BadRequest },

            // Transacciones
            { 3906, HttpStatusCode.InternalServerError },
            { 3930, HttpStatusCode.InternalServerError },
            { 1205, HttpStatusCode.Conflict },

            // Otros
            { 8152, HttpStatusCode.BadRequest },
            { 8156, HttpStatusCode.InternalServerError }
        };
        public static InfrastructureException ToInfrastructureException(this SqlException ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Ha ocurrido un error interno. Por favor, inténtalo nuevamente más tarde.";

            foreach (SqlError error in ex.Errors)
                if (ErrorCodeMappings.TryGetValue(error.Number, out statusCode))
                    break;
                else
                    statusCode = HttpStatusCode.InternalServerError;
            
            return new InfrastructureException(statusCode, message, ex);
        }
    }
}
