using Application.Interfaces.Cache;
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
        public async Task<IEnumerable<ExchangeRates>> GetAllExchangeRateAsync()
        {
            try
            {
                const string cacheKey = "exchange-rates";
                var cachedData = await _cacheService.GetAllAsync<ExchangeRates>(cacheKey);
                if (cachedData != null)
                    return cachedData;

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var exchangeRates = await conn.QueryAsync<ExchangeRates>(
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
        public async Task<IEnumerable<SystemModules>> GetAllSystemModulesAsync()
        {
            try
            {
                const string cacheKey = "system-modules";
                var cachedData = await _cacheService.GetAllAsync<SystemModules>(cacheKey);
                if (cachedData != null)
                    return cachedData;

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var systemModules = await conn.QueryAsync<SystemModules>(
                    @"SELECT Dscod, Modulo  
                      FROM FST111");
                await _cacheService.SetAsync(cacheKey, () => Task.FromResult(systemModules), TimeSpan.FromHours(24));
                return systemModules;
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
        public async Task<IEnumerable<ProductStatusCode>> GetAllProductStatusCodeAsync()
        {
            try
            {
                const string cacheKey = "status-code";
                var cachedData = await _cacheService.GetAllAsync<ProductStatusCode>(cacheKey);
                if (cachedData != null)
                    return cachedData;

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                var productStatusCode = await conn.QueryAsync<ProductStatusCode>(
                    @"SELECT Cecod, Cenom, Cenomr, Cepop 
                      FROM FST026");
                await _cacheService.SetAsync(cacheKey, () => Task.FromResult(productStatusCode), TimeSpan.FromHours(24));
                return productStatusCode;
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
