using Application.Common.Enums;
using Application.Contracts.Login;
using Application.Interfaces.Services.Login;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services.Login
{
    public class LoginService : ILoginServices
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IPersonsRepository _personsRepository;
        public LoginService(ILoginRepository loginRepository, IPersonsRepository personsRepository)
        {
            _loginRepository = loginRepository;
            _personsRepository = personsRepository;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            PersonCode getPerson;
            string NumDoc = request.Complement != string.Empty ? request.DocumentNumber + "-" + request.Complement : request.DocumentNumber + request.Issue;
            var persons = await _personsRepository.GetPersonCodeAsync(request.DocumentNumber);
            getPerson = persons!.Where(x => x.JSBN05Pais == request.CountryDocument && x.JSBN05TDoc == request.DocumentType && x.JSBN05NDoc!.Trim() == NumDoc).FirstOrDefault()!;
            
            if (getPerson == null)
            {
                getPerson = persons!.Where(x => x.JSBN05Pais == request.CountryDocument && x.JSBN05TDoc == request.DocumentType && x.JSBN05NDoc!.Trim() == request.DocumentNumber).FirstOrDefault()!;
                if (getPerson == null)
                    return Result<LoginResponse>.Failure("Código de persona no encontrado.", HttpStatusCode.NotFound);
            }
            
            int _codPerson = getPerson.JSBN05CPer;
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
