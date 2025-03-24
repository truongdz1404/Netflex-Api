using Netflex.Entities;


namespace Netflex.Database.Repositories.Abstractions
{
    public interface IActorRepository : IBaseRepository<Actor>
    {
        Task<List<Actor>> GetActorsByNameAsync(string name);
    }
}
