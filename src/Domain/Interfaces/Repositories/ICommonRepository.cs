using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ICommonRepository
    {
        Task<IEnumerable<ExchangeRates>> GetAllExchangeRateAsync();
        Task<IEnumerable<SystemModules>> GetAllSystemModulesAsync();
        Task<IEnumerable<ProductStatusCode>> GetAllProductStatusCodeAsync();
    }
}
