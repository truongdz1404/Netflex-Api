using Microsoft.EntityFrameworkCore;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Entities;

namespace Netflex.Database.Repositories.Implements
{
    public class CountryRepository(ApplicationDbContext dbContext)
        : BaseRepository<Country>(dbContext), ICountryRepository
    {
        public async Task<List<Country>> GetCountriesByNameAsync(string name)
        {
            return await Entities
                .Where(c => c.Name.Contains(name))
                .ToListAsync();
        }
    }
}
