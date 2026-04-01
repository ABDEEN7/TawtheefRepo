using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class UserRepository(TawtheefDbContext dbContext, IGenericRepository<User> repository) : BaseRepository<User>(repository), IUserRepository
{
    public async Task<IResult<User>> GetByEmailAsync(string email, params Expression<Func<User, object>>[]? includes)
    {
        if (string.IsNullOrEmpty(email))
            return Result.Fail<User>(ErrorsCodes.EmailRequired);

        var query = Repository.DbSet.AsQueryable();

        if (includes is null || includes.Length <= 0)
        {
            var user = await query.FirstOrDefaultAsync(u => u.Email == email);
            return user != null ? Result.Ok(user) : Result.Fail<User>(ErrorsCodes.UserNotFound);
        }
        query = includes.Aggregate(query, (current, include) => current.Include(include));
        
        var result = await query.FirstOrDefaultAsync(u => u.Email == email);
        return result != null ? Result.Ok(result) : Result.Fail<User>(ErrorsCodes.UserNotFound);
    }

    public async Task<IResult<User>> GetByIdAsync(Guid id, params Expression<Func<User, object>>[]? includes)
    {
        var query = Repository.DbSet.AsQueryable();

        if (includes is null || includes.Length <= 0) 
        {
            var user = await query.FirstOrDefaultAsync(u => u.Id == id);
            return user != null ? Result.Ok(user) : Result.Fail<User>(ErrorsCodes.UserNotFound);
        }
        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var result = await query.FirstOrDefaultAsync(u => u.Id == id);
        return result != null ? Result.Ok(result) : Result.Fail<User>(ErrorsCodes.UserNotFound);
    }

    public async Task<IResult<bool>> IsEmailUniqueAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return Result.Fail<bool>(ErrorsCodes.EmailRequired);

        return Result.Ok(!await Repository.DbSet.AnyAsync(u => u.Email == email));
    }

    public async Task<IResult<bool>> IsUserNameUniqueAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return Result.Fail<bool>(ErrorsCodes.UserNameRequired);

        return Result.Ok(!await Repository.DbSet.AnyAsync(u => u.UserName == userName));
    }

    public async Task<IResult<bool>> IsPhoneNumberUniqueAsync(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return Result.Fail<bool>(ErrorsCodes.PhoneNumberRequired);

        return Result.Ok(!await Repository.DbSet.AnyAsync(u => u.PhoneNumber == phoneNumber));
    }

    public async Task<IReadOnlyList<User>> GetUsersByPermissionAsync(string permission, CancellationToken ct)
    {
        var roleIds = await dbContext.RoleClaims
            .AsNoTracking()
            .Where(rc => rc.ClaimType == RoleClaimTypes.Permission && rc.ClaimValue == permission)
            .Select(rc => rc.RoleId)
            .ToListAsync(ct);

        if (roleIds.Count == 0) return [];

        var userIds = await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => roleIds.Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(ct);

        if (userIds.Count == 0) return [];

        return await Repository.DbSet
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(ct);
    }
}
