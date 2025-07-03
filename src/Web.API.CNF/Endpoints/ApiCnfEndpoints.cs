using Application.Contracts.AccountList;
using Application.Contracts.Accounts;
using Application.Contracts.Credits;
using Application.Contracts.Login;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.Credits;
using Application.Interfaces.Services.Login;
using Application.Services.Accounts;
using Web.API.CNF.Extensions;

namespace Web.API.CNF.Endpoints
{
    public static class ApiCnfEndpoints
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
        {
            /*EndPoints Minimal API*/
            endpoints.MapPost("/login", async (LoginRequest loginRequest, ILoginServices loginService) =>
            {
                var result = await loginService.LoginAsync(loginRequest);
                if (result.IsSuccess)
                    return Results.Ok(result.Value);
                else
                    return Results.Extensions.Failure(result.Error!);
            });

            endpoints.MapPost("/listAccounts", async (ListAccountsRequest listAccountsRequest, IListAccountsServices listAccountsService) =>
            {
                var result = await listAccountsService.ListAccountsAsync(listAccountsRequest);
                if (result.IsSuccess)
                    return Results.Ok(result.Value);
                else
                    return Results.Extensions.Failure(result.Error!);
            });

            endpoints.MapPost("/getDetailCredit", async (GetDetailCreditRequest getDetailCreditRequest, IGetDetailCreditServices getDetailCreditServices) =>
            {
                var result = await getDetailCreditServices.GetDetailCreditAsync(getDetailCreditRequest);
                if (result.IsSuccess)
                    return Results.Ok(result.Value);
                else
                    return Results.Extensions.Failure(result.Error!);
            });

            endpoints.MapPost("/accountTransactions", async (AccountTransactionsRequest accountTransactionsRequest, IAccountTransactionsServices accountTransactionsServices) =>
            {
                var result = await accountTransactionsServices.AccountTransactionsAsync(accountTransactionsRequest);
                if (result.IsSuccess)
                    return Results.Ok(result.Value);
                else
                    return Results.Extensions.Failure(result.Error!);
            });

            //endpoints.MapGet("/health", () => Results.Ok("API CNF is running."));

            /************************************************////
            return endpoints;
        }
    }
}
