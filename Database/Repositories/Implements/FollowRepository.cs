using Microsoft.EntityFrameworkCore;
namespace Netflex.Database.Repositories.Implements
{
    public class FollowRepository : IFollowRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FollowRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Follow?> GetByUserIdAndFilmIdAsync(string userId, Guid filmId)
        {
            return await _dbContext.Set<Follow>()
                .FirstOrDefaultAsync(f => f.FollowerId == userId && f.FilmId == filmId);
        }

        public async Task<Follow?> GetByUserIdAndSerieIdAsync(string userId, Guid serieId)
        {
            return await _dbContext.Set<Follow>()
                .FirstOrDefaultAsync(f => f.FollowerId == userId && f.SerieId == serieId);
        }

        public async Task<List<Follow>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Set<Follow>()
                .Where(f => f.FollowerId == userId)
                .ToListAsync();
        }

        public async Task<Follow> AddAsync(Follow follow)
        {
            await _dbContext.Set<Follow>().AddAsync(follow);
            return follow;
        }

        public Task DeleteAsync(Follow follow)
        {
            _dbContext.Set<Follow>().Remove(follow);
            return Task.CompletedTask;
        }

        public async Task<List<Follow>> GetAllAsync()
        {
            return await _dbContext
                .Set<Follow>()
                .ToListAsync();
        }

        public async Task<int> Save(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
