using Dapper;
using System.Data;
using Domain.Entities;
using Application.Common.Enums;
using Microsoft.Data.SqlClient;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;

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
                var resp_ = await conn.QueryAsync<BTSIO00>("ApiCoreCnfGetAccountsClient",
                    new { @IEmp = _Emp, @IPais = Country, @ITdoc = DocType, @INDoc = DocNumber },
                    commandType: CommandType.StoredProcedure);
                return resp_;
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
        public async Task<FSD001?> GetPersonDataAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<FSD001>(
                    @"ApiCoreCnfGetPersonData",
                    new { IPais = Country, ITdoc = DocType, INDoc = DocNumber });
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
                    @"dbo.FBT_TipoFacultad", new { cuenta = AccountClient }, commandType: CommandType.StoredProcedure);
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
