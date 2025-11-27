using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IGenericRepository<T> where T : class, IBaseEntity
{
    DbSet<T> DbSet { get; }
 
    Task<IResult<T?>> GetByIdAsync(Guid id);
    Task<IResult<List<T>>> GetAllAsync();
    Task<IResult<T>> AddAsync(T entity);
    Task<IResult<IList<T>>> AddRangeAsync(IList<T> entity);
    Task<IResult<T>> UpdateAsync(T entity);
    Task<IResult<IList<T>>> UpdateRangeAsync(IList<T> entity);
    Task<Result> DeleteAsync(T entity);
    Task<Result> DeleteRangeAsync(IEnumerable<T> entities);
    Task<Result> DeleteAsync(Guid id);
}
