using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ILoginRepository
    {
        Task<JSBN05?> GetPersonCodeAsync(short Country, short DocType, string DocNumber);
        Task<JSBY408?> GetRegisteredCnfAsync(int IdCnf, int PersonCode);
    }
}
