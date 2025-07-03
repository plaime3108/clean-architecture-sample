using Application.Common.Enums;
using Application.Contracts.BtServices.Credits;
using Application.Contracts.BtServices.Token;
using Application.Contracts.Credits;
using Application.Interfaces.External;
using Application.Interfaces.Services.Credits;
using Application.Interfaces.Utils;
using Application.Responses;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Repositories;
using System.Text.Json;

namespace Application.Services.Credits
{
    public class GetDetailCreditServices : IGetDetailCreditServices
    {
        private readonly IExternalApiClient _externalApiClient;
        private readonly IBtinreqProvider _btinreqProvider;
        private readonly IPersonsRepository _personsRepository;
        public GetDetailCreditServices(IExternalApiClient externalApiClient, IBtinreqProvider btinreqProvider, IPersonsRepository personsRepository)
        {
            _externalApiClient = externalApiClient;
            _btinreqProvider = btinreqProvider;
            _personsRepository = personsRepository;
        }
        public async Task<Result<GetDetailCreditResponse>> GetDetailCreditAsync(GetDetailCreditRequest request)
        {
            PersonCode getPerson;
            GetDetailCreditResponse getDetailCredit = new();
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var formated = new AccountFormatHelper(request.AccountId);
            var persons = await _personsRepository.GetPersonCodeAsync(request.DocumentNumber);
            getPerson = persons!.Where(x => x.JSBN05Pais == request.CountryDocument && x.JSBN05TDoc == request.DocumentType && x.JSBN05NDoc!.Trim() == NumDoc).FirstOrDefault()!;

            if (getPerson == null)
            {
                getPerson = persons!.Where(x => x.JSBN05Pais == request.CountryDocument && x.JSBN05TDoc == request.DocumentType && x.JSBN05NDoc!.Trim() == request.DocumentNumber).FirstOrDefault()!;
                if (getPerson == null)
                    return Result<GetDetailCreditResponse>.Failure("Código de persona no encontrado.", HttpStatusCode.NotFound);
            }

            var valpersonAccount = await _personsRepository.ValidatePersonAccountAsync(request.CountryDocument, request.DocumentType, getPerson.JSBN05NDoc!.Trim(), formated.Acc);
            
            if (valpersonAccount == null)
                return Result<GetDetailCreditResponse>.Failure("El número de cuenta no corresponde al número de documento proporcionado.", HttpStatusCode.UnprocessableEntity);

            var pendingInstallment = await GetPendingInstallment(formated.Cmp, formated.Mod, formated.Brn, formated.Ccy, formated.Doc, formated.Acc, formated.Opr, formated.Sop, formated.Opt);

            if (pendingInstallment.Error > 0)
                return Result<GetDetailCreditResponse>.Failure("No existe la operación o no tiene cuotas en mora.", HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(pendingInstallment.Pendinginst))
                return Result<GetDetailCreditResponse>.Failure("La operación no tiene cuotas pendientes.", HttpStatusCode.NoContent);

            var creditAmount = await GetCreditAmount(formated.Cmp, formated.Mod, formated.Brn, formated.Ccy, formated.Doc, formated.Acc, formated.Opr, formated.Sop, formated.Opt);
            
            return Result<GetDetailCreditResponse>.Success(new GetDetailCreditResponse
            {
                TotalInstallment = pendingInstallment.Totalinst,
                Principalbalance = pendingInstallment.Principalbalance,
                Currency = formated.Ccy,
                LegalExpenses = pendingInstallment.Legalexpenses,
                AccountId = request.AccountId,
                AmountBs = creditAmount.Amountcreditdebtbs,
                AmountUsd = creditAmount.Amountcreditdebtusd,
                Endcurrency = creditAmount.Endcurrency,
                Installments = JsonSerializer.Deserialize<IEnumerable<InstallmentNode>>(pendingInstallment.Pendinginst)!,
            });
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
        private async Task<GetPendingInstResponse> GetPendingInstallment(short Cmp, short Mod, short Brn, short Ccy, short Doc, int Acc, int Opr, short Sop, short Opt)
        {
            var token = await _externalApiClient.SendAsync<GetTokenResponse>();

            return await _externalApiClient.SendAsync<GetPendingInstRequest, GetPendingInstResponse>(new GetPendingInstRequest
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
            }, "GetPendingInstallmentPath");
        }
    }
}
