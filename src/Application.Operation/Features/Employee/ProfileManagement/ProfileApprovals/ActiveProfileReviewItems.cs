using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

internal static class ActiveProfileReviewItems
{
    public static List<ReviewItem> ForFullReview(UserProfile profile, IEnumerable<ReviewItem> items)
    {
        return items
            .Where(item =>
                item.ProfileChangeId == null &&
                !item.IsDeleted &&
                item.IsActiveInCurrentProfile(profile))
            .ToList();
    }
}
