using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetEntityRepository<T>() where T : EventEntity;
    void Remove<T>(T? entity) where T : EventEntity;
    void RemoveRange<T>(IList<T>? entities) where T : EventEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);
    Task Rollback();
    
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    bool HasActiveTransaction { get; }
    
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation, 
        CancellationToken cancellationToken = default);
}
