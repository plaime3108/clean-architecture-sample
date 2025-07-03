using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ILoginRepository
    {
        Task<RegisterCnf?> GetRegisteredCnfAsync(int IdCnf, int PersonCode);
    }
}
