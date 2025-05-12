using Application.Contracts.AccountList;
using Application.Responses;

namespace Application.Interfaces.Services.Accounts
{
    public interface IListAccountsServices
    {
        Task<Result<ListAccountsResponse>> ListAccountsAsync(ListAccountsRequest request);
    }
}
