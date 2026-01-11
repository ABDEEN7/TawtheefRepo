using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile;

public static class UserProfileLoader
{
    public static async Task<IResult<UserProfile?>> GetSummaryAsync(IUnitOfWork uow, Guid userId, CancellationToken ct) {
        var repo = uow.GetEntityRepository<UserProfile>();
        var profile = await repo.DbSet.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        return Result.Ok(profile);
    }

    public static async Task<UserProfile?> GetFullProfileByUserId(IUnitOfWork uow, 
        Guid userId,bool tracking = false, CancellationToken ct = default)
    {
        var query = UserProfileQueryFactory.CreateFullQuery(uow, tracking);
        var profile =  await query.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        return profile;
    }
    public static async Task<UserProfile?> GetFullProfileByProfileId(IUnitOfWork uow, 
        Guid profileId,bool tracking = false, CancellationToken ct = default)
    {
        var query = UserProfileQueryFactory.CreateFullQuery(uow, tracking);
        var profile =  await query.FirstOrDefaultAsync(p => p.Id == profileId, ct);
        return profile;
    }
}
