using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement;

public static class UserProfileLoader
{
    public static async Task<UserProfile?> GetFullProfileByUserId(IUnitOfWork uow, 
        Guid userId,bool tracking = false, CancellationToken ct = default)
    {
        var query = UserProfileQueryFactory.CreateFullQuery(uow, tracking);
        var profile =  await query.OrderBy(p => p.Id).FirstOrDefaultAsync(p => p.UserId == userId, ct);
        return profile;
    }
    public static async Task<UserProfile?> GetFullProfileByProfileId(IUnitOfWork uow, 
        Guid profileId,bool tracking = false, CancellationToken ct = default)
    {
        var query = UserProfileQueryFactory.CreateFullQuery(uow, tracking);
        var profile =  await query.OrderBy(p => p.Id).FirstOrDefaultAsync(p => p.Id == profileId, ct);
        return profile;
    }
}
