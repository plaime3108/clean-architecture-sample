using Application.Contracts.Credits;
using Application.Responses;

namespace Application.Interfaces.Services.Credits
{
    public interface IGetDetailCreditServices
    {
        Task<Result<GetDetailCreditResponse>> GetDetailCreditAsync(GetDetailCreditRequest request);
    }
}
