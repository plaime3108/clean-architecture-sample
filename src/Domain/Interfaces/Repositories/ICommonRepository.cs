using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ICommonRepository
    {
        Task<IEnumerable<FST005>> GetExchangeRateAllAsync();
    }
}
