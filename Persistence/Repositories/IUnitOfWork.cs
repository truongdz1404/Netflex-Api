using Netflex.Entities.Abstractions;

namespace Netflex.Persistence.Repositories;

public interface IUnitOfWork
{
    IBaseRepository<T> Repository<T>() where T : Entity;
    Task<int> Save(CancellationToken cancellationToken);
    Task Rollback();
}