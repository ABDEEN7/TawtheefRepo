using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Repositories.Base;

public interface IGenericRepository<T> where T : class, IBaseEntity
{
    DbSet<T> DbSet { get; }
 
    Task<IResult<T?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IResult<List<T>>> GetAllAsync(CancellationToken ct = default);
    Task<IResult<T>> AddAsync(T entity, CancellationToken ct = default);
    Task<IResult<IList<T>>> AddRangeAsync(IList<T> entity, CancellationToken ct = default);
    Task<IResult<T>> UpdateAsync(T entity, CancellationToken ct = default);
    Task<IResult<IList<T>>> UpdateRangeAsync(IList<T> entity, CancellationToken ct = default);
    Task<Result> DeleteAsync(T entity);
    Task<Result> DeleteRangeAsync(IEnumerable<T> entities);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
