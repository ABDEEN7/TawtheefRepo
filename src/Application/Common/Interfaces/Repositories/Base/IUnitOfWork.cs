using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetEntityRepository<T>() where T : EventEntity;
    IGenericRepository<T> GetUserRepository<T>() where T : User;
    void Remove<T>(T? entity) where T : EventEntity;
    void RemoveRange<T>(IList<T>? entities) where T : EventEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);
    Task Rollback();
    
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    bool HasActiveTransaction { get; }
}
