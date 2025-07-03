using Application.Common.Enums;
using Application.Contracts.AccountList;
using Application.Contracts.BtServices.Credits;
using Application.Contracts.BtServices.Token;
using Application.Interfaces.External;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Utils;
using Application.Responses;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Repositories;

namespace Application.Services.Accounts
{
    public class ListAccountsServices : IListAccountsServices
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IBtinreqProvider _btinreqProvider;
        private readonly IExternalApiClient _externalApiClient;
        private readonly IPersonsRepository _personsRepository;
        public ListAccountsServices(IAccountsRepository accountsRepository, IPersonsRepository personsRepository, ICommonRepository commonRepository, IBtinreqProvider btinreqProvider, IExternalApiClient externalApiClient)
        {
            _accountsRepository = accountsRepository;
            _commonRepository = commonRepository;
            _btinreqProvider = btinreqProvider;
            _externalApiClient = externalApiClient;
            _personsRepository = personsRepository;
        }
        public async Task<Result<ListAccountsResponse>> ListAccountsAsync(ListAccountsRequest request)
        {
            ListAccountsResponse listAccounts = new();
            string Name = string.Empty;
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var PersonData = await _personsRepository.GetPersonDataAsync(request.CountryDocument, request.DocumentType, NumDoc);
            if (PersonData == null)
                return Result<ListAccountsResponse>.Failure("El número de documento no se encuentra registrado.", HttpStatusCode.NotFound);

            if (PersonData.Petipo == "F")
                Name = PersonData.Pfnom1.Trim() + " " + (PersonData!.Pfape1.Trim() + " " + PersonData!.Pfape2.Trim()).Trim();
            else
                Name = PersonData.Pjrazs.Trim();

            var Accounts = await _accountsRepository.GetAccountsClient(request.CountryDocument, request.DocumentType, NumDoc);

            if (Accounts == null | Accounts!.Count() == 0)
                return Result<ListAccountsResponse>.Failure("No existen productos asociados a este número de documento.", HttpStatusCode.NotFound);

            List<AccountsNode> accountsList = [];
            switch (request.AccountType)
            {
                case "CSA": case "CM":
                    var accQueries = await BuildQueries(Accounts!, request.AccountType);
                    accountsList.Add(accQueries!);
                    break;
                case "CDP": case "PPE":
                    var credits = await BuildCredits(Accounts!, request.AccountType == "CDP" ? "origen" : "destino");
                    accountsList.Add(credits!);
                    break;
                case "PPD":
                    var originAcc = await BuildOriginAccount(Accounts!);
                    accountsList.Add(originAcc!);
                    var creditDest = await BuildCredits(Accounts!, "destino")!;
                    accountsList.Add(creditDest!);
                    break;
                case "TRCTA":
                    var originAccXfer = await BuildOriginAccount(Accounts!);
                    accountsList.Add(originAccXfer!);
                    var destAccXfer = await BuildDestAccount(Accounts!);
                    accountsList.Add(destAccXfer!);
                    break;
                case "TR3ROS": case "PS": case "PI":
                    var originAccTr3ros = await BuildOriginAccount(Accounts!);
                    accountsList.Add(originAccTr3ros!);
                    break;
                default:
                    return Result<ListAccountsResponse>.Failure("No es un tipo de transacción valida.", HttpStatusCode.BadRequest);
            }

            if (accountsList.Contains(item: null!))
                return Result<ListAccountsResponse>.Failure("No existen productos para operar en este canal.", HttpStatusCode.NotFound);

            listAccounts.ClientName = Name;
            listAccounts.Accounts = accountsList;
            return Result<ListAccountsResponse>.Success(listAccounts);
        }
        private async Task<AccountsNode?> BuildQueries(IEnumerable<Account> Accounts, string AccountType)
        {
            AccountsNode accountsNode = new();
            var filteredAccounts = Accounts!.Where(x => x.BTSIO00Mod == 21 && x.BTSIO00Est == 0 && x.BTSIO00Fac.Trim() != "CONJUNTA").ToList();
            var tasks = filteredAccounts.Select(async x =>
            {
                var accountFormat = await BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub, x.BTSIO00Ope, "AHO");
                var balanceAcc = await GetBalanceAccount(x.BTSIO00Guid, x.BTSIO00Id);
                var account = new AccountResponse
                {
                    IdAccount = x.BTSIO00Id,
                    IdAccountGuid = x.BTSIO00Guid,
                    UnformattedAccount = AccountFormatHelper.BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                    MaskedAccount = AccountFormatHelper.BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub, x.BTSIO00Ope, "AHO"),
                    TextAccount = accountFormat,
                    Currency = x.BTSIO00Mda,
                    Balance = AccountType == "CSA" ? balanceAcc : 0,
                };
                lock (accountsNode)
                    accountsNode.AccountNode.Add(account);
            });

            await Task.WhenAll(tasks);

            if (accountsNode.AccountNode.Count == 0)
                return null;

            accountsNode.TypeDirection = "origen";
            return accountsNode;
        }
        private async Task<AccountsNode?> BuildCredits(IEnumerable<Account> Accounts, string typeDirection) 
        {
            AccountsNode accountsNode = new();
            var modules = (await _commonRepository.GetAllSystemModulesAsync())
                .Where(x => x.Dscod == 50)
                .Select(x => x.Modulo).ToArray();
            int[] creditStatus = { 90, 99 };
            var filteredAccounts = Accounts!.Where(x => x.BTSIO00Mod != 21 && !creditStatus.Contains(x.BTSIO00Est) && modules.Contains(x.BTSIO00Mod)).ToList();
            var tasks = filteredAccounts.Select(async x =>
            {
                var amountsDebt = await GetCreditAmount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top);
                var accountFormat = await BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub, x.BTSIO00Ope, string.Empty);
                var account = new AccountResponse
                {
                    IdAccount = x.BTSIO00Id,
                    IdAccountGuid = x.BTSIO00Guid,
                    UnformattedAccount = AccountFormatHelper.BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                    MaskedAccount = AccountFormatHelper.BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub, x.BTSIO00Ope, string.Empty),
                    TextAccount = accountFormat,
                    Currency = x.BTSIO00Mda,
                    Balance = 0,
                    DebtAmount = amountsDebt.Amountcreditdebtbs,
                    DebtAmountConv = amountsDebt.Amountcreditdebtusd,
                    EndCurrency = amountsDebt.Endcurrency,
                };
                lock (accountsNode)
                    accountsNode.AccountNode.Add(account);
            });
            await Task.WhenAll(tasks);
            if (accountsNode.AccountNode.Count == 0)
                return null;

            accountsNode.TypeDirection = typeDirection;
            return accountsNode;
        }
        private async Task<AccountsNode?> BuildOriginAccount(IEnumerable<Account> Accounts)
        {
            AccountsNode accountsNode = new();
            string[] allowsEnable = { "S", "D" };
            var statusCode = (await _commonRepository.GetAllProductStatusCodeAsync())
                .Where(x => allowsEnable.Contains(x.Cepop))
                .Select(x => x.Cecod).ToArray();
            var filteredAccounts = Accounts!.Where(x => x.BTSIO00Mod == 21 && x.BTSIO00Fac.Trim() != "CONJUNTA" && statusCode.Contains(x.BTSIO00Est)).ToList();
            var tasks = filteredAccounts.Select(async x =>
            {
                var accountFormat = await BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub, x.BTSIO00Ope, "AHO");
                var account = new AccountResponse
                {
                    IdAccount = x.BTSIO00Id,
                    IdAccountGuid = x.BTSIO00Guid,
                    UnformattedAccount = AccountFormatHelper.BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                    MaskedAccount = AccountFormatHelper.BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub, x.BTSIO00Ope, "AHO"),
                    TextAccount = accountFormat,
                    Currency = x.BTSIO00Mda,
                };
                lock (accountsNode)
                    accountsNode.AccountNode.Add(account);
            });
            await Task.WhenAll(tasks);
            if (accountsNode.AccountNode.Count == 0)
                return null;

            accountsNode.TypeDirection = "origen";
            return accountsNode;
        }
        private async Task<AccountsNode?> BuildDestAccount(IEnumerable<Account> Accounts)
        {
            AccountsNode accountsNode = new();
            string[] allowsEnable = { "S", "C" };
            var statusCode = (await _commonRepository.GetAllProductStatusCodeAsync())
                .Where(x => allowsEnable.Contains(x.Cepop))
                .Select(x => x.Cecod).ToArray();
            var filteredAccounts = Accounts!.Where(x => x.BTSIO00Mod == 21 && statusCode.Contains(x.BTSIO00Est)).ToList();
            var tasks = filteredAccounts.Select(async x =>
            {
                var accountFormat = await BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub, x.BTSIO00Ope, "AHO");
                var account = new AccountResponse
                {
                    IdAccount = x.BTSIO00Id,
                    IdAccountGuid = x.BTSIO00Guid,
                    UnformattedAccount = AccountFormatHelper.BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                    MaskedAccount = AccountFormatHelper.BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub, x.BTSIO00Ope, "AHO"),
                    TextAccount = accountFormat,
                    Currency = x.BTSIO00Mda,
                };
                lock (accountsNode)
                    accountsNode.AccountNode.Add(account);
            });
                await Task.WhenAll(tasks);
            if (accountsNode.AccountNode.Count == 0)
                return null;

            accountsNode.TypeDirection = "destino";
            return accountsNode;
        }

        private async Task<string> BuildTextFormatAccount(int Acc, short Ccy, short Sop, int Opr, string type)
        {
            string MaskedAccount = AccountFormatHelper.BuildMaskedAccount(Acc, Sop, Opr, type);
            var AcronymCurrency = (await _commonRepository.GetAllExchangeRateAsync())
                .FirstOrDefault(x => x.Moneda == Ccy);
            return MaskedAccount + " " + AcronymCurrency!.Mosign.Trim();
        }

        private async Task<int> GetBalanceAccount(Guid guid, decimal AccoundId)
        {
            var AccountData = await _accountsRepository.GetBalanceAccount(guid, AccoundId);
            return Convert.ToInt32(AccountData!.availableBalance * 100);
        }
        private async Task<GetCreditAmountResponse> GetCreditAmount(short Cmp, short Mod, short Brn, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            var token = await _externalApiClient.SendAsync<GetTokenResponse>();
            return await _externalApiClient.SendAsync<GetCreditAmountRequest, GetCreditAmountResponse>(new GetCreditAmountRequest
            {
                Btinreq = _btinreqProvider.GetBtinreqAsync(token.SessionToken),
                Company = Cmp,
                Branch = Brn,
                Module = Mod,
                Currency = Ccy,
                Paper = Doc,
                Account = Acc,
                Operation = Opr,
                Suboperation = Sop,
                Operationtype = Opt
            }, "GetCreditAmountPath");
        }
    }
}