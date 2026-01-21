using Tawtheef.Domain.Entities.Security;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Security;

public static class RolePermissionCatalog
{
    public static readonly IReadOnlyDictionary<Guid, IReadOnlyCollection<PermissionKey>> ByRoleId =
        new Dictionary<Guid, IReadOnlyCollection<PermissionKey>>
        {
            [SystemRoleIds.SystemAdmin] = PermissionCatalog.All.Select(x => x.Key).ToArray(),
            [SystemRoleIds.HrManager] = [
                Permissions.Dashboard.View.Key
            ],
            [SystemRoleIds.DepartmentManager] = [
                Permissions.Dashboard.View.Key
            ],
            [SystemRoleIds.Employee] = [
                Permissions.Dashboard.View.Key
            ],

            [SystemRoleIds.OfficeAdmin] = [
                Permissions.Dashboard.View.Key,
                Permissions.OfficeUsers.View.Key,
                Permissions.OfficeUsers.Manage.Key
            ],
            [SystemRoleIds.OfficeUser] = [
                Permissions.Dashboard.View.Key
            ],

            [SystemRoleIds.EmployeeSuperAdmin] = [
                Permissions.Dashboard.View.Key,
                Permissions.Profile.View.Key,
                Permissions.Profile.Manage.Key,
                Permissions.ProfileDistribution.View.Key,
                Permissions.ProfileDistribution.Manage.Key,
                Permissions.ProfileApproval.View.Key,
                Permissions.ProfileApproval.Review.Key,
                Permissions.ProfileApproval.Changes.Key,

                Permissions.Jobs.View.Key,
                Permissions.Jobs.Manage.Key,
                Permissions.Jobs.Approve.Key,
                Permissions.JobsPoints.Manage.Key,
                Permissions.JobsPoints.View.Key,
                Permissions.JobsPoints.Approve.Key,

                Permissions.JobsInvitations.View.Key,

                Permissions.Nominations.View.Key,
                Permissions.Nominations.Manage.Key,

                Permissions.Kawader.Manage.Key,

                Permissions.OfficeUsers.View.Key,
                Permissions.OfficeUsers.Manage.Key,

                Permissions.CandidateUsers.View.Key,
                Permissions.CandidateUsers.Manage.Key,

                Permissions.OrganizationStructures.Manage.Key,

                Permissions.MajorSkills.Manage.Key
            ]
        };
}
