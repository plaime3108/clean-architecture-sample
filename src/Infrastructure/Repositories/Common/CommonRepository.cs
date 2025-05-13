using Application.Common.Enums;
using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories.Common
{
    public class CommonRepository : ICommonRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseSettings _dbSettings;

        public CommonRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<IEnumerable<FST005>> GetExchangeRateAllAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<FST005>(
                    @"SELECT Moneda, Mosign, Monom, Momdiv, Mocpra, Movta, Moarb, Moarbc 
                      FROM FST005");
            }
            catch (SqlException ex) 
            {
                throw ex.ToInfrastructureException();
            }
            catch (Exception ex)
            {
                throw new InfrastructureException(HttpStatusCode.InternalServerError, "Ha ocurrido un error interno. Por favor, inténtalo nuevamente más tarde.", ex);
            }
        }
    }
}
