using Dapper;
using System.Data;
using Domain.Entities;
using Application.Common.Enums;
using Microsoft.Data.SqlClient;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Repositories.ListAccounts
{
    public class ListAccountsRepository : IListAccountsRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseSettings _dbSettings;

        public ListAccountsRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<IEnumerable<BTSIO00>> GetAccountsClient(short Country, short DocType, string DocNumber)
        {
            short _Emp = 1;
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<BTSIO00>(
                    @"SELECT BTSIO00Id, BTSIO00Emp, BTSIO00Mod, BTSIO00Suc, BTSIO00Mda, BTSIO00Pap, BTSIO00Cta, BTSIO00Ope, BTSIO00Sub, BTSIO00Top, BTSIO00Guid 
                      FROM FSR008 
                      INNER JOIN BTSIO00 ON BTSIO00Emp = Pgcod AND BTSIO00Cta = Ctnro 
                      WHERE Pgcod = @Emp AND Pepais = @Pais AND Petdoc = @Tdoc AND Pendoc = @NDoc",
                    new { Emp = _Emp, Pais = Country, TDoc = DocType, NDoc = DocNumber });
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
        public async Task<FSD001?> GetTypePersonAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<FSD001>(
                    @"SELECT Pepais, Petdoc, Pendoc, Petipo, Penom 
                      FROM FSD001 
                      WHERE Pepais = @Pais AND Petdoc = @Tdoc AND Pendoc = @NDoc",
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
        public async Task<FSD002?> GetPersonDataAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<FSD002>(
                    @"SELECT Pfpais, Pftdoc, Pfndoc, Pfape1, Pfape2, Pfnom1, Pfnom2, Pfebco, Pfcant, Pffnac 
                      FROM FSD002 
                      WHERE Pfpais = @Pais AND Pftdoc = @Tdoc AND Pfndoc = @NDoc",
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
        public async Task<FSD003?> GetDataLegalPersonAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<FSD003>(
                    @"SELECT Pjpais, Pjtdoc, Pjndoc, Pjrazs, Njcod, Pjfcon 
                      FROM FSD003 
                      WHERE Pjpais = @Pais AND Pjtdoc = @Tdoc AND Pjndoc = @NDoc",
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
        public async Task<string?> ValidateAccountClient(int AccountClient)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.ExecuteScalarAsync<string>(
                    @"dbo.FBT_TipoFacultad", new { @cuenta = AccountClient }, commandType: CommandType.StoredProcedure);
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
