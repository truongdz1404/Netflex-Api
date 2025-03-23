using Netflex.Entities;

namespace Netflex.Database.Repositories.Abstractions
{
    public interface ICountryRepository : IBaseRepository<Country>
    {
        Task<List<Country>> GetCountriesByNameAsync(string name);
    }
}
