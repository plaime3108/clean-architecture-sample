using Application.Common.Enums;
using Application.Contracts.Cache;
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
        private readonly ICacheService _cacheService;

        public CommonRepository(IOptions<DatabaseSettings> dbSettings, ICacheService cacheService)
        {
            _dbSettings = dbSettings.Value;
            _connectionString = _dbSettings.GetConnectionString();
            _cacheService = cacheService;
        }
        public async Task<IEnumerable<FST005>> GetExchangeRateAllAsync()
        {
            try
            {
                const string cacheKey = "exchange-rates";
                var cachedData = await _cacheService.GetAsync<FST005>(cacheKey);
                if (cachedData != null)
                    return cachedData;

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var exchangeRates = await conn.QueryAsync<FST005>(
                    @"SELECT Moneda, Mosign, Monom, Momdiv, Mocpra, Movta, Moarb, Moarbc 
                      FROM FST005");
                await _cacheService.SetAsync(cacheKey, () => Task.FromResult(exchangeRates), TimeSpan.FromHours(24));
                return exchangeRates;
            }
            catch (SqlException ex) 
            {
                throw ex.ToInfrastructureException();
            }
            catch (Exception ex)
            {
                throw ex.ToInfrastructureException();
            }
        }
    }
}
