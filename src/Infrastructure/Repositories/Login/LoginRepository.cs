using Application.Common.Enums;
using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories.Login
{
    public class LoginRepository : ILoginRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseSettings _dbSettings;

        public LoginRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<JSBN05?> GetPersonCodeAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<JSBN05>(
                    @"SELECT JSBN05Pais, JSBN05TDoc, JSBN05NDoc, JSBN05Pai2, JSBN05TDo2, JSBN05Raiz, JSBN05Comp, JSBN05Exte, JSBN05Regu, JSBN05AuC1, JSBN05AuC2, JSBN05AuN1, JSBN05AuI1, JSBN05AuF1, JSBN05CPer
                      FROM JSBN05 
                      WHERE JSBN05Pais = @Pais and JSBN05TDoc = @TDoc AND JSBN05NDoc = @NDoc",
                    new { Pais = Country, TDoc = DocType, NDoc = DocNumber });
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
        public async Task<JSBY408?> GetRegisteredCnfAsync(int IdCnf, int PersonCode)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<JSBY408>(
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
