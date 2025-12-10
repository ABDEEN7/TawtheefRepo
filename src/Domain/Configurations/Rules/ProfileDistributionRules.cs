using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Rules;

public static class ProfileDistributionRules
{
    public static readonly UserProfileStatus[] StartStatuses =
        [UserProfileStatus.InCreation];
    
    public static readonly UserProfileStatus[] FinalStatuses =
        [UserProfileStatus.Approved, UserProfileStatus.Rejected, UserProfileStatus.Cancelled, UserProfileStatus.AdminCancelled];

    public static readonly UserProfileStatus[] AssignableStatuses =
        [UserProfileStatus.Submitted, UserProfileStatus.UnderReview, UserProfileStatus.RequiresUpdate];
}
