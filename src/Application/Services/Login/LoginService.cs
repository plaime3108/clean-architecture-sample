using Application.Common.Enums;
using Application.Contracts.Login;
using Application.Interfaces.Services.Login;
using Application.Responses;
using Domain.Interfaces.Repositories;

namespace Application.Services.Login
{
    public class LoginService : ILoginServices
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var CodePerson = await _loginRepository.GetPersonCodeAsync(request.CountryDocument, request.DocumentType, NumDoc);

            if (CodePerson == null)
                return Result<LoginResponse>.Failure("Codigo de persona no encontrado.", HttpStatusCode.NotFound);

            int _codPerson = CodePerson!.JSBN05CPer;
            var CnfRegistered = await _loginRepository.GetRegisteredCnfAsync(request.IdCnf, _codPerson);

            if (CnfRegistered == null)
                return Result<LoginResponse>.Failure("La persona no se encuentra registrada como corresponsal.", HttpStatusCode.NotFound);

            if (CnfRegistered!.JSBY408es == 3)
            {
                if (CnfRegistered!.JSBY419est == 3)
                    return Result<LoginResponse>.Success(new LoginResponse { Status = "A", Name = CnfRegistered!.JSBY408nc.Trim() });
                else
                    return Result<LoginResponse>.Failure("Persona no habilitada para trabajar con corresponsal.", HttpStatusCode.Unauthorized);
            }
            else
                return Result<LoginResponse>.Failure("Persona no afiliada a corresponsal.", HttpStatusCode.Forbidden);
        }
    }
}
