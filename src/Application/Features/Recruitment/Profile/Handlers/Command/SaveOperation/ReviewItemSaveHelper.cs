using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

internal static class ReviewItemSaveHelper
{
    public static async Task UpdateSectionStatusAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return;

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var items = await reviewRepo.DbSet
            .Where(item => item.UserProfileId == profile.Id && item.Status != ReviewStatus.Solved)
            .ToListAsync(ct);

        if (items.Count == 0)
        {
            profile.Status = UserProfileStatus.Submitted;
            return;
        }

        foreach (var item in items.Where(item => item.Section == section))
        {
            item.Status = ReviewStatus.Solved;
            item.IsOutdated = false;
        }

        if (items.All(item => item.Status == ReviewStatus.Solved))
        {
            profile.Status = UserProfileStatus.Submitted;
        }
    }
}
