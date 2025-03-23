using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflex.Database.Repositories
{
    public interface IFollowRepository
    {
        Task<Follow?> GetByUserIdAndFilmIdAsync(string userId, Guid filmId);
        Task<Follow?> GetByUserIdAndSerieIdAsync(string userId, Guid serieId);
        Task<List<Follow>> GetByUserIdAsync(string userId);
        Task<List<Follow>> GetAllAsync();
        Task<Follow> AddAsync(Follow follow);
        Task DeleteAsync(Follow follow);
        Task<int> Save(CancellationToken cancellationToken);
    }
}
