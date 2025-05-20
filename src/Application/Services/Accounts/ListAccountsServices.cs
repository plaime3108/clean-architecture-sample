using Application.Common.Enums;
using Application.Contracts.AccountList;
using Application.Interfaces.Services.Accounts;
using Domain.Interfaces.Repositories;
using Application.Responses;

namespace Application.Services.ListAccounts
{
    public class ListAccountsServices : IListAccountsServices
    {
        private readonly IListAccountsRepository _listAccountsRepository;
        private readonly ICommonRepository _commonRepository;
        public ListAccountsServices(IListAccountsRepository listAccountsRepository, ICommonRepository commonRepository)
        {
            _listAccountsRepository = listAccountsRepository;
            _commonRepository = commonRepository;
        }
        public async Task<Result<ListAccountsResponse>> ListAccountsAsync(ListAccountsRequest request)
        {
            ListAccountsResponse listAccounts = new ListAccountsResponse();

            string Name = string.Empty;
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var PersonData = await _listAccountsRepository.GetPersonDataAsync(request.CountryDocument, request.DocumentType, NumDoc);
            if (PersonData == null)
                return Result<ListAccountsResponse>.Failure("El número de documento no se encuentra registrado.", HttpStatusCode.NotFound);

            if (PersonData.Petipo == "F")
                Name = PersonData.Pfnom1.Trim() + " " + (PersonData!.Pfape1.Trim() + " " + PersonData!.Pfape2.Trim()).Trim();
            else
                Name = PersonData.Pjrazs.Trim();

            var Accounts = await _listAccountsRepository.GetAccountsClient(request.CountryDocument, request.DocumentType, NumDoc);

            if (Accounts == null | Accounts!.Count() == 0)
                return Result<ListAccountsResponse>.Failure("No existen cajas de ahorro asociadas a este número de documento", HttpStatusCode.NotFound);

            Accounts!.Where(x => x.BTSIO00Mod == 21 & x.BTSIO00Fac.Trim() != "CONJUNTA").ToList().ForEach(x => listAccounts.Accounts.Add(new Account
            {
                IdAccount = x.BTSIO00Id,
                IdAccountGuid = x.BTSIO00Guid,
                UnformattedAccount = BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                MaskedAccount = BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub),
                TextAccount = BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub),
                Currency = x.BTSIO00Mda,
            }));

            if (listAccounts.Accounts.Count == 0)
                return Result<ListAccountsResponse>.Failure("No existen cajas de ahorro para operar en este canal.", HttpStatusCode.NotFound);

            listAccounts.ClientName = Name;
            listAccounts.TypeDirection = "origen";
            return Result<ListAccountsResponse>.Success(listAccounts);
        }
        private string BuildUnformattedAccount(short Cmp, short Mod, short Brn, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            return (1000 + Cmp).ToString("D4").Substring(1, 3) + Brn.ToString("D3").Trim() + (1000 + Mod).ToString("D4").Substring(1, 3) + (10000 + Ccy).ToString("D5").Substring(1, 4) + (10000 + Doc).ToString("D5").Substring(1, 4) + 
                   (1000000000 + Acc).ToString("D10").Substring(1, 9) + (1000 + Sop).ToString("D4").Substring(1, 3) + (1000000000 + Opr).ToString("D10").Substring(1, 9) + (1000 + Opt).ToString("D4").Substring(1, 3);
        }

        private string BuildMaskedAccount(int Acc, short Sop)
        {
            string CharactersMask = "****************";
            return string.Concat(Acc.ToString().AsSpan(0, 3), CharactersMask.AsSpan(0, 3), Sop.ToString().PadLeft(4, '0').AsSpan(1, 3));
        }

        private string BuildTextFormatAccount(int Acc, short Ccy, short Sop)
        {
            string MaskedAccount = BuildMaskedAccount(Acc, Sop);
            var AcronymCurrency = _commonRepository.GetExchangeRateAllAsync().Result.FirstOrDefault(x => x.Moneda == Ccy);
            return MaskedAccount + " " + AcronymCurrency!.Mosign.Trim();
        }
    }
}