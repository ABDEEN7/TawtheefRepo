using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IGenericRepository<T> where T : class, IBaseEntity
{
    DbSet<T> DbSet { get; }
 
    Task<Result<T?>> GetByIdAsync(Guid id);
    Task<Result<List<T>>> GetAllAsync();
    Task<Result<T>> AddAsync(T entity);
    Task<Result<IList<T>>> AddRangeAsync(IList<T> entity);
    Task<Result<T>> UpdateAsync(T entity);
    Task<Result<IList<T>>> UpdateRangeAsync(IList<T> entity);
    Task<Result> DeleteAsync(T entity);
    Task<Result> DeleteAsync(Guid id);
}
