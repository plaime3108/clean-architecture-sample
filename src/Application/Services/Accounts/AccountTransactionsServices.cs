using Application.Common.Enums;
using Application.Contracts.Accounts;
using Application.Interfaces.Services.Accounts;
using Application.Responses;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Repositories;

namespace Application.Services.Accounts
{
    public class AccountTransactionsServices : IAccountTransactionsServices
    {
        private readonly IAccountsRepository _accountsRepository;
        public AccountTransactionsServices(IAccountsRepository accountsRepository) 
        {
            _accountsRepository = accountsRepository;
        }
        public async Task<Result<AccountTransactionsResponse>> AccountTransactionsAsync(AccountTransactionsRequest request)
        {
            List<AccountTransactions> accountTransactions = new();
            AccountTransactionsResponse transactionsResponse = new();
            var formated = new AccountFormatHelper(request.AccountId);
            var accountId = await _accountsRepository.GetAccountId(formated.Cmp, formated.Brn, formated.Mod, formated.Ccy, formated.Doc, formated.Acc, formated.Opr, formated.Sop, formated.Opt);

            transactionsResponse.AccountNumber = AccountFormatHelper.BuildMaskedAccount(formated.Acc, formated.Sop, formated.Opr, "AHO");
            transactionsResponse.Balance = await GetBalanceAccount(accountId!.BTSIO00Guid, accountId!.BTSIO00Id);
            transactionsResponse.Currency = formated.Ccy;

            var dailyTrn = await _accountsRepository.GetDailyTransactions(formated.Cmp, formated.Brn, formated.Mod, formated.Ccy, formated.Doc, formated.Acc, formated.Opr, formated.Sop, formated.Opt);
            accountTransactions.AddRange(dailyTrn!.Select(x => new AccountTransactions
            {
                Date = x.Itfcon,
                Time = x.Ithora,
                Amount = x.Itimp1,
                TransactionType = x.Itdbha,
                TransactionDetail = x.Cmnom.Trim()
            }));

            if (accountTransactions.Count >= 10)
            {
                transactionsResponse.Transactions.AddRange(accountTransactions.OrderBy(x => x.Date).ThenBy(x => x.Time).Select(x => new Transaction
                {
                    Date = x.Date.ToString("dd/MM/yy"),
                    Time = x.Time,
                    Amount = Convert.ToInt32(x.Amount * 100),
                    TransactionType = x.TransactionType == 1 ? "D" : "C",
                    TransactionDetail = x.TransactionDetail
                }).TakeLast(10));

                return Result<AccountTransactionsResponse>.Success(transactionsResponse);
            }

            var endDate = DateTime.Today;
            var startDate = endDate.AddYears(-1);

            var historyTrn = await _accountsRepository.GetHistoryTransactions(formated.Cmp, formated.Brn, formated.Mod, formated.Ccy, formated.Doc, formated.Acc, formated.Opr, formated.Sop, formated.Opt, startDate, endDate);
            accountTransactions.AddRange(historyTrn!.Select(x => new AccountTransactions
            {
                Date = x.Hfcon,
                Time = x.Hhora,
                Amount = x.Hcimp1,
                TransactionType = x.Hcodmo,
                TransactionDetail = x.Cmnom.Trim()
            }));

            if (accountTransactions.Count == 0)
                return Result<AccountTransactionsResponse>.Failure("La caja de ahorro no tiene movimientos.", HttpStatusCode.NoContent);

            transactionsResponse.Transactions.AddRange(accountTransactions.OrderBy(x => x.Date).ThenBy(x => x.Time).Select(x => new Transaction
            {
                Date = x.Date.ToString("dd/MM/yy"),
                Time = x.Time,
                Amount = Convert.ToInt32(x.Amount * 100),
                TransactionType = x.TransactionType == 1 ? "D" : "C",
                TransactionDetail = x.TransactionDetail
            }).TakeLast(10));

            return Result<AccountTransactionsResponse>.Success(transactionsResponse);
        }
        private async Task<int> GetBalanceAccount(Guid guid, decimal AccoundId)
        {
            var AccountData = await _accountsRepository.GetBalanceAccount(guid, AccoundId);
            return Convert.ToInt32(AccountData!.availableBalance * 100);
        }
    }
}
