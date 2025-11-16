using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Infrastructure.Data;

namespace Tawtheef.Infrastructure.Repositories.Base;

public class GenericRepository<T>(TawtheefDbContext dbContext) : IGenericRepository<T>
    where T : class, IBaseEntity
{
    public DbSet<T> DbSet => dbContext.Set<T>();
 
    public async Task<Result<T>> AddAsync(T entity)
    {
        try
        {
            await dbContext.Set<T>().AddAsync(entity);
            return Result.Success(entity);
        }
        catch(Exception e)
        {
            return Result.Failure<T>($"An error occurred while adding the entity: {e.Message}");
        }
    }
    public async Task<Result<IList<T>>> AddRangeAsync(IList<T> entity)
    {
        try
        {
            await dbContext.Set<T>().AddRangeAsync(entity);
            return Result.Success(entity);
        }
        catch(Exception e)
        {
            return Result.Failure<IList<T>>($"An error occurred while adding the entity: {e.Message}");
        }
    }
 
    public async Task<Result<T>> UpdateAsync(T entity)
    {
        try
        {

            var exist = await dbContext.Set<T>().FindAsync(entity.Id);
            if(exist is null) 
                return Result.Failure<T>($"Entity with ID {entity.Id} does not exist.");
            dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return Result.Success(entity);
        }
        catch (Exception e)
        {
            return Result.Failure<T>($"An error occurred while updating the entity: {e.Message}");
        }
    }
    
    public async Task<Result<IList<T>>> UpdateRangeAsync(IList<T> entity)
    {
        try
        {
            foreach (var item in entity)
            {
                var exist = await dbContext.Set<T>().FindAsync(item.Id);
                if (exist is null) 
                    return Result.Failure<IList<T>>($"Entity with ID {item.Id} does not exist.");
                dbContext.Entry(exist).CurrentValues.SetValues(item);
            }
            return Result.Success(entity);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<T>>($"An error occurred while updating the entities: {e.Message}");
        }
    }
 
    public Task<Result> DeleteAsync(T entity)
    {
        try
        {
            dbContext.Set<T>().Remove(entity);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure($"An error occurred while deleting the entity: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await dbContext.Set<T>().FindAsync(id);
            if (entity == null)
                return Result.Failure($"Entity with ID {id} does not exist.");
            
            dbContext.Set<T>().Remove(entity);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while deleting the entity with ID {id}: {ex.Message}");
        }
    }
 
    public async Task<Result<List<T>>> GetAllAsync()
    {
        try
        {
            return Result.Success(await dbContext.Set<T>().AsNoTracking().ToListAsync());
        }
        catch (Exception ex)
        {
            return Result.Failure<List<T>>($"An error occurred while retrieving entities: {ex.Message}");
        }
    }
 
    public async Task<Result<T?>> GetByIdAsync(Guid id)
    {
        try
        {
            return Result.Success(await dbContext.Set<T>().FindAsync(id));
        }
        catch (Exception ex)
        {
            return Result.Failure<T?>($"An error occurred while retrieving the entity with ID {id}: {ex.Message}");
        }
    }
}
