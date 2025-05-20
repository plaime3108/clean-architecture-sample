using Application.Contracts.Login;
using Application.Responses;

namespace Application.Interfaces.Services.Login
{
    public interface ILoginServices
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
