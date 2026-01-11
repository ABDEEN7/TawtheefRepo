using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Rules;

public static class ProfileDistributionRules
{
    public static readonly UserProfileStatus[] StartStatuses =
        [UserProfileStatus.InCreation];
    
    public static readonly UserProfileStatus[] FinalStatuses =
        [UserProfileStatus.Approved];

    public static readonly UserProfileStatus[] AssignableStatuses =
        [UserProfileStatus.Submitted, UserProfileStatus.UnderReview];
}
