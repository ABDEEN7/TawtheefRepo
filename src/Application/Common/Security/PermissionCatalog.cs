using System.Collections.ObjectModel;

namespace Tawtheef.Application.Common.Security;

public static class PermissionCatalog
{
    public static readonly ReadOnlyCollection<PermissionDefinition> All = new(
    [
        Permissions.Dashboard.View,

        Permissions.Roles.View,
        Permissions.Roles.Manage,

        Permissions.Users.View,
        Permissions.Users.Manage,

        Permissions.Offices.View,
        Permissions.Offices.Manage,

        Permissions.Languages.View,
        Permissions.Languages.Manage,
        Permissions.Religions.View,
        Permissions.Religions.Manage,

        Permissions.Countries.View,
        Permissions.Countries.Manage,

        Permissions.Universities.View,
        Permissions.Universities.Manage,

        Permissions.Profile.View,
        Permissions.Profile.Manage,

        Permissions.ProfileDistribution.View,
        Permissions.ProfileDistribution.Manage,

        Permissions.ProfileApproval.View,
        Permissions.ProfileApproval.Review,
        Permissions.ProfileApproval.Changes,

        Permissions.Jobs.View,
        Permissions.Jobs.Manage,
        Permissions.Jobs.Approve,

        Permissions.JobsPoints.Manage,
        Permissions.JobsInvitations.View,

        Permissions.Nominations.View,
        Permissions.Nominations.Manage,

        Permissions.Kawader.Manage,

        Permissions.OfficeUsers.View,
        Permissions.OfficeUsers.Manage,

        Permissions.MajorSkills.Manage,
    ]);

    public static readonly ISet<string> Keys =
        All.Select(x => x.Key.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
}
