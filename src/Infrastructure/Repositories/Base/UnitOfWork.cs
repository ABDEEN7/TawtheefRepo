using System.Collections;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Common;
using Tawtheef.Infrastructure.Data;

namespace Tawtheef.Infrastructure.Repositories.Base;

public class UnitOfWork(TawtheefDbContext dbContext) : IUnitOfWork, IAsyncDisposable
{
    private readonly TawtheefDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private Hashtable? _repositories;
    private IDbContextTransaction? _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction is not null;

    public IGenericRepository<T> GetEntityRepository<T>() where T : EventEntity
    {
        _repositories ??= new Hashtable();
 
        var type = typeof(T).Name;

        if (_repositories.ContainsKey(type)) return (IGenericRepository<T>)_repositories[type]!;
        var repositoryType = typeof(GenericRepository<>);
 
        var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);
 
        _repositories.Add(type, repositoryInstance);

        return (IGenericRepository<T>) _repositories[type]!;
    }

    public void Remove<T>(T? entity) where T : EventEntity
    {
        if (entity is null) return;
        _dbContext.Remove(entity);
    }
    public void RemoveRange<T>(IList<T>? entities) where T : EventEntity
    {
        if (entities is null or {Count: <= 0}) return;
        _dbContext.RemoveRange(entities);
    }

    public Task Rollback()
    {
        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }
 
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No-op if a transaction is already active (supports nested usage)
        if (_currentTransaction is not null) return;

        // Pick the isolation level you prefer (default ReadCommitted is fine for most cases)
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Best-effort rollback on commit failure
            try { await _currentTransaction.RollbackAsync(cancellationToken); }
            catch { /* swallow rollback errors here */ }
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return;

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
    
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation, 
        CancellationToken cancellationToken = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation(cancellationToken);

                if (result is Result { IsFailed: true })
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
    // ----------------------------------------------

    public void Dispose()
    {
        // Defensive: rollback any dangling transaction
        if (_currentTransaction is not null)
        {
            _currentTransaction.Rollback();
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        _dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        await _dbContext.DisposeAsync();
    }
}
