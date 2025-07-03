using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<IEnumerable<Account>> GetAccountsClient(short Country, short DocType, string DocNumber);
        Task<AccountId?> GetAccountId(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt);
        Task<string?> ValidateAccountClient(int AccountClient);
        Task<AccountData?> GetBalanceAccount(Guid guid, decimal AccountId);
        Task<IEnumerable<DailyTransactions>?> GetDailyTransactions(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt);
        Task<IEnumerable<HistoryTransactions>?> GetHistoryTransactions(short Cmp, short Brn, short Mod, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt, DateTime StartDate, DateTime EndDate);
        Task<AccountCnf?> GetAccountCnf(int IdCnf);
    }
}
