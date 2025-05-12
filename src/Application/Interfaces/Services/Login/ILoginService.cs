using Application.Contracts.Login;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Login
{
    public interface ILoginService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
