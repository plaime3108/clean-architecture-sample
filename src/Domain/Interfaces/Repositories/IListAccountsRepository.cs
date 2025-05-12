using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IListAccountsRepository
    {
        Task<FSD001?> GetTypePersonAsync(short Country, short DocType, string DocNumber);
        Task<FSD002?> GetPersonDataAsync(short Country, short DocType, string DocNumber);
        Task<FSD003?> GetDataLegalPersonAsync(short Country, short DocType, string DocNumber);
        Task<IEnumerable<BTSIO00>> GetAccountsClient(short Country, short DocType, string DocNumber);
        Task<string?> ValidateAccountClient(int AccountClient);
    }
}
