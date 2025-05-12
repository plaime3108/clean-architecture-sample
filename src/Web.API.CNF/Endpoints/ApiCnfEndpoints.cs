using Application.Contracts.Login;
using Application.Interfaces.Services.Login;
using Application.Responses;
using Web.API.CNF.Extensions;

namespace Web.API.CNF.Endpoints
{
    public static class ApiCnfEndpoints
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
        {
            /*EndPoints Minimal API*/
            endpoints.MapPost("/login", async (LoginRequest loginRequest, ILoginService loginService) =>
            {
                var result = await loginService.LoginAsync(loginRequest);
                if (result.IsSuccess)
                    return Results.Ok(result.Value);
                else
                    return Results.Extensions.Failure(result.Error!);
            });


            /************************************************////
            return endpoints;
        }
    }
}
