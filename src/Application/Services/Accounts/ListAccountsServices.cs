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
        public ListAccountsServices(IListAccountsRepository listAccountsRepository)
        {
            _listAccountsRepository = listAccountsRepository;
        }
        public async Task<Result<ListAccountsResponse>> ListAccountsAsync(ListAccountsRequest request)
        {
            ListAccountsResponse listAccounts = new ListAccountsResponse();

            string Name = string.Empty;
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var TypePerson = await _listAccountsRepository.GetTypePersonAsync(request.CountryDocument, request.DocumentType, NumDoc);
            if (TypePerson == null)
                return Result<ListAccountsResponse>.Failure("El número de documento no se encuentra registrado.", HttpStatusCode.NotFound);

            if (TypePerson!.Petipo == "F")
            {
                var PersonData = await _listAccountsRepository.GetPersonDataAsync(request.CountryDocument, request.DocumentType, NumDoc);
                if (PersonData == null)
                    return Result<ListAccountsResponse>.Failure("El número de documento de la persona no es válido.", HttpStatusCode.NotFound);
                
                Name = PersonData!.Pfnom1.Trim() + " " + (PersonData!.Pfape1.Trim() + " " + PersonData!.Pfape2.Trim()).Trim();
            }
            else
            {
                var DataLegalPerson = await _listAccountsRepository.GetDataLegalPersonAsync(request.CountryDocument, request.DocumentType, NumDoc.Trim());
                if (DataLegalPerson == null)
                    return Result<ListAccountsResponse>.Failure("El número de documento de la persona jurídica no es válido.", HttpStatusCode.NotFound);

                Name = DataLegalPerson!.Pjrazs.Trim();
            }

            var Accounts = await _listAccountsRepository.GetAccountsClient(request.CountryDocument, request.DocumentType, NumDoc);

            if (Accounts == null)
                return Result<ListAccountsResponse>.Failure("No existen cajas de ahorro asociadas a este número de documento", HttpStatusCode.NotFound);

            Accounts.ToList().ForEach(x => listAccounts.Accounts.Add(new Account
            {
                IdAccount = x.BTSIO00Id,
                IdAccountGuid = x.BTSIO00Guid,
                UnformattedAccount = BuildUnformattedAccount(x.BTSIO00Emp, x.BTSIO00Mod, x.BTSIO00Suc, x.BTSIO00Mda, x.BTSIO00Pap, x.BTSIO00Cta, x.BTSIO00Ope, x.BTSIO00Sub, x.BTSIO00Top),
                MaskedAccount = BuildMaskedAccount(x.BTSIO00Cta, x.BTSIO00Sub),
                TextAccount = BuildTextFormatAccount(x.BTSIO00Cta, x.BTSIO00Mda, x.BTSIO00Sub),
                Currency = x.BTSIO00Mda,
            }));

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
            return string.Concat(Acc.ToString().AsSpan(1, 3), CharactersMask.AsSpan(1, 3), Sop.ToString().PadLeft(4, '0').AsSpan(2, 3));
        }

        private string BuildTextFormatAccount(int Acc, short Ccy, short Sop)
        {
            string MaskedAccount = BuildMaskedAccount(Acc, Sop);
            return Ccy == 0 ? MaskedAccount + " Bs" : MaskedAccount + " USD";
        }
    }
}