using System.Collections.ObjectModel;

namespace Tawtheef.Application.Common.Security;

public static class PermissionCatalog
{
    public static readonly ReadOnlyCollection<PermissionDefinition> All = new([
        Permissions.Dashboard.View,
        Permissions.Dashboard.Export,
        Permissions.Dashboard.Manage,

        Permissions.Roles.View,
        Permissions.Roles.Manage,

        Permissions.Users.View,
        Permissions.Users.Manage,

        Permissions.CandidateUsers.View,
        Permissions.CandidateUsers.Manage,

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

        Permissions.TargetEntities.View,
        Permissions.TargetEntities.Manage,

        Permissions.HomeContent.View,
        Permissions.HomeContent.Manage,

        Permissions.ProfileLogs.View,

        Permissions.Profile.View,
        Permissions.Profile.Manage,

        Permissions.ProfileDistribution.View,
        Permissions.ProfileDistribution.Manage,

        Permissions.ProfileApproval.View,
        Permissions.ProfileApproval.Review,
        Permissions.ProfileApproval.Changes,

        Permissions.Jobs.View,
        Permissions.Jobs.Edit,
        Permissions.Jobs.Approve,
        Permissions.Jobs.SendInvitation,
        Permissions.Jobs.Cancel,
        Permissions.Jobs.Create,
        Permissions.Jobs.Publish,
        Permissions.Jobs.Delete,
        Permissions.Jobs.Clone,

        Permissions.JobsPoints.Edit,
        Permissions.JobsPoints.View,
        Permissions.JobsPoints.Approve,


        Permissions.JobsInvitations.View,
        Permissions.JobsInvitations.ManageAttachment,

        Permissions.Kawader.Manage,

        Permissions.OfficeUsers.View,
        Permissions.OfficeUsers.Manage,

        Permissions.MajorSkills.View,
        Permissions.MajorSkills.Manage,
        Permissions.OrganizationStructures.Manage,
        
        Permissions.MinisterOffice.View,
        Permissions.MinisterOffice.Manage,
        
        Permissions.Cities.View,
        Permissions.Cities.Manage,

        Permissions.JobTitles.View,
        Permissions.JobTitles.Manage,

        Permissions.JobCategoryCandidateSettings.View,
        Permissions.JobCategoryCandidateSettings.Manage,

        Permissions.JobPointsConfiguration.View,
        Permissions.JobPointsConfiguration.Manage,

        Permissions.InvitationExpiryConfiguration.View,
        Permissions.InvitationExpiryConfiguration.Manage,

        Permissions.Exceptions.View,
        Permissions.Exceptions.Create,
        Permissions.Exceptions.SendInvitation,
        Permissions.Exceptions.Cancel,
    ]);

    public static readonly ISet<string> Keys =
        All.Select(x => x.Key.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
}
