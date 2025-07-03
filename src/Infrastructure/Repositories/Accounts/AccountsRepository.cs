using Application.Common.Enums;
using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Infrastructure.Repositories.Accounts
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseSettings _dbSettings;

        public AccountsRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<IEnumerable<Account>> GetAccountsClient(short Country, short DocType, string DocNumber)
        {
            short _Emp = 1;
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<Account>(
                    @"ApiCoreCnfGetAccountsClient",
                    new { @IEmp = _Emp, @IPais = Country, @ITdoc = DocType, @INDoc = DocNumber },
                    commandType: CommandType.StoredProcedure);
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
        public async Task<AccountId?> GetAccountId(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<AccountId>(
                    @"SELECT BTSIO00Id, BTSIO00Guid 
                      FROM BTSIO00 
                      WHERE BTSIO00Emp = @Iemp AND BTSIO00Mod = @Imod AND BTSIO00Suc = @Isuc AND BTSIO00Mda = @Imda AND BTSIO00Pap = @Ipap AND BTSIO00Cta = @Icta AND BTSIO00Ope = @Ioper AND BTSIO00Sub = @Isub AND BTSIO00Top = @Itope",
                    new { @Iemp = Cmp, @Isuc = Brn, @Imod = Mod, @Imda = Ccy, @Ipap = Doc, @Icta = Acc, @Ioper = Opr, @Isub = Sop, @Itope = Opt });
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
                    @"dbo.FBT_TipoFacultad",
                    new { cuenta = AccountClient },
                    commandType: CommandType.StoredProcedure);
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
        public async Task<AccountData?> GetBalanceAccount(Guid guid, decimal AccountId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<AccountData>(
                    @"ApiProductGetProductDataByAccountGuid",
                    new { @AccountGuid = guid, AccountId },
                    commandType: CommandType.StoredProcedure);
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
        public async Task<IEnumerable<DailyTransactions>?> GetDailyTransactions(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<DailyTransactions>(
                    @"ApiCoreCnfGetDailyTransactions",
                    new { @IEmp = Cmp, @ISuc = Brn, @IMod = Mod, @IMda = Ccy, @IPap = Doc, @ICta = Acc, @IOper = Opr, @ISubOp = Sop, @ITope = Opt },
                    commandType: CommandType.StoredProcedure);
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
        public async Task<IEnumerable<HistoryTransactions>?> GetHistoryTransactions(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<HistoryTransactions>(
                    @"ApiCoreCnfGetHistoryTransactions",
                    new { @IEmp = Cmp, @ISuc = Brn, @IMod = Mod, @IMda = Ccy, @IPap = Doc, @ICta = Acc, @IOper = Opr, @ISubOp = Sop, @ITope = Opt, @IFini = StartDate, @IFfin = EndDate },
                    commandType: CommandType.StoredProcedure);
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

        public async Task<AccountCnf?> GetAccountCnf(int IdCnf)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<AccountCnf>(
                    @"SELECT JSBY408id, JSBY409md, JSBY409fr, JSBY409hr, JSBY409pg, JSBY409ml, JSBY409ag, JSBY409ct, JSBY409pl, JSBY409op, JSBY409sb, JSBY409to, JSBY409so, JSBY409us, JSBY409su 
                      FROM JSBY409
                      WHERE JSBY408id = @Id",
                    new { @Id = IdCnf });
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
