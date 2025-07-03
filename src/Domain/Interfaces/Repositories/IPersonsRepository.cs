using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IPersonsRepository
    {
        Task<IEnumerable<PersonCode>?> GetPersonCodeAsync(string DocumentRoot); //short Country, short DocType, string DocNumber);
        Task<PersonData?> GetPersonDataAsync(short Country, short DocType, string DocNumber);
        Task<AccountIntegration?> ValidatePersonAccountAsync(short Country, short DocType, string DocNumber, int Account);
    }
}
