using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IListAccountsRepository
    {
        Task<FSD001?> GetPersonDataAsync(short Country, short DocType, string DocNumber);
        Task<IEnumerable<BTSIO00>> GetAccountsClient(short Country, short DocType, string DocNumber);
        Task<string?> ValidateAccountClient(int AccountClient);
    }
}
