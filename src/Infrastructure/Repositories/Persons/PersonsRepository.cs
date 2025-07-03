using Dapper;
using System.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Configurations;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Application.Common.Enums;

namespace Infrastructure.Repositories.Persons
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseSettings _dbSettings;

        public PersonsRepository(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
        }
        public async Task<PersonData?> GetPersonDataAsync(short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<PersonData>(
                    @"ApiCoreCnfGetPersonData",
                    new { IPais = Country, ITdoc = DocType, INDoc = DocNumber },
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
        public async Task<IEnumerable<PersonCode>?> GetPersonCodeAsync(string DocumentRoot) //short Country, short DocType, string DocNumber)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryAsync<PersonCode>(
                    @"SELECT JSBN05Pais, JSBN05TDoc, JSBN05NDoc, JSBN05Pai2, JSBN05TDo2, JSBN05Raiz, JSBN05Comp, JSBN05Exte, JSBN05Regu, JSBN05AuC1, JSBN05AuC2, JSBN05AuN1, JSBN05AuI1, JSBN05AuF1, JSBN05CPer
                      FROM JSBN05 
                      WHERE JSBN05Raiz = @RDoc",//JSBN05Pais = @Pais and JSBN05TDoc = @TDoc AND JSBN05NDoc = @NDoc",
                    new { RDoc = DocumentRoot }); //Pais = Country, TDoc = DocType, NDoc = DocNumber });
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
        public async Task<AccountIntegration?> ValidatePersonAccountAsync(short Country, short DocType, string DocNumber, int Account)
        {
            try
            {
                short Cmp = 1;
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<AccountIntegration>(
                    @"SELECT Pgcod, Ctnro, Pepais, Petdoc, Pendoc, Ttcod, Cttfir 
                      FROM FSR008 
                      WHERE Pgcod = @Emp AND Ctnro = @IAcc AND Pepais = @IPais AND Petdoc = @ITdoc AND Pendoc = @INDoc",
                    new { Emp = Cmp, IAcc = Account, IPais = Country, ITdoc = DocType, INDoc = DocNumber });
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
