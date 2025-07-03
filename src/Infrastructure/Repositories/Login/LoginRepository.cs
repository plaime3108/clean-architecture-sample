using Application.Common.Enums;
using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DatabaseSettings _dbSettings;
        private readonly string _connectionString;
        public LoginRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<RegisterCnf?> GetRegisteredCnfAsync(int IdCnf, int PersonCode)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<RegisterCnf>(
                    @"SELECT a.JSBY408id, JSBY408cp, JSBY408fr, JSBY408nc, JSBY408ar, JSBY408es, JSBY408dc, JSBY419cp, JSBY419est 
                    FROM JSBY408 a
                    INNER JOIN JSBY419 b ON a.JSBY408id = b.JSBY408id AND b.JSBY419cp = @PCode
                    WHERE a.JSBY408id = @Id",
                    new { Id = IdCnf, PCode = PersonCode });
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
