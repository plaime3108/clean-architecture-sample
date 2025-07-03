using Application.Contracts.Accounts;
using Application.Responses;

namespace Application.Interfaces.Services.Accounts
{
    public interface IAccountTransactionsServices
    {
        Task<Result<AccountTransactionsResponse>> AccountTransactionsAsync(AccountTransactionsRequest request);
    }
}
