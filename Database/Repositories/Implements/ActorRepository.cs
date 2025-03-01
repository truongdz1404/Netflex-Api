
using Microsoft.EntityFrameworkCore;
using Netflex.Database.Repositories.Abstractions;

namespace Netflex.Database.Repositories.Implements
{
    public class ActorRepository(ApplicationDbContext dbContext)
        : BaseRepository<Actor>(dbContext), IActorRepository
    {
        public async Task<List<Actor>> GetActorsByNameAsync(string name)
        {
            return await Entities
                .Where(a => a.Name.Contains(name))
                .ToListAsync();
        }
    }
}
