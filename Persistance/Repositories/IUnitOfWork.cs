namespace Netflex.Persistance.Repositories;

public interface IUnitOfWork
{
    IBaseRepository<T> Repository<T>() where T : Entity;
    Task<int> Save(CancellationToken cancellationToken);
    Task Rollback();
}