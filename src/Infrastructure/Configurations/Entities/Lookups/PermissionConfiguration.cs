using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class PermissionConfiguration : LookupBaseConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new Permission
            {
                Id = PermissionIds.DashboardView,
                BackendName = PermissionNames.DashboardView,
                NameEn = "Dashboard - View",
                NameAr = "Dashboard - View",
                DisplayOrder = 1
            },
            new Permission
            {
                Id = PermissionIds.RolesView,
                BackendName = PermissionNames.RolesView,
                NameEn = "Roles - View",
                NameAr = "Roles - View",
                DisplayOrder = 2
            },
            new Permission
            {
                Id = PermissionIds.RolesManage,
                BackendName = PermissionNames.RolesManage,
                NameEn = "Roles - Manage",
                NameAr = "Roles - Manage",
                DisplayOrder = 3
            },
            new Permission
            {
                Id = PermissionIds.UsersView,
                BackendName = PermissionNames.UsersView,
                NameEn = "Users - View",
                NameAr = "Users - View",
                DisplayOrder = 4
            },
            new Permission
            {
                Id = PermissionIds.UsersManage,
                BackendName = PermissionNames.UsersManage,
                NameEn = "Users - Manage",
                NameAr = "Users - Manage",
                DisplayOrder = 5
            },
            new Permission
            {
                Id = PermissionIds.OfficesView,
                BackendName = PermissionNames.OfficesView,
                NameEn = "Offices - View",
                NameAr = "Offices - View",
                DisplayOrder = 6
            },
            new Permission
            {
                Id = PermissionIds.OfficesManage,
                BackendName = PermissionNames.OfficesManage,
                NameEn = "Offices - Manage",
                NameAr = "Offices - Manage",
                DisplayOrder = 7
            },
            new Permission
            {
                Id = PermissionIds.ProfileView,
                BackendName = PermissionNames.ProfileView,
                NameEn = "Profile - View",
                NameAr = "Profile - View",
                DisplayOrder = 8
            },
            new Permission
            {
                Id = PermissionIds.ProfileManage,
                BackendName = PermissionNames.ProfileManage,
                NameEn = "Profile - Manage",
                NameAr = "Profile - Manage",
                DisplayOrder = 9
            },
            new Permission
            {
                Id = PermissionIds.ProfileDistributionView,
                BackendName = PermissionNames.ProfileDistributionView,
                NameEn = "Profile Distribution - View",
                NameAr = "Profile Distribution - View",
                DisplayOrder = 10
            },
            new Permission
            {
                Id = PermissionIds.ProfileDistributionManage,
                BackendName = PermissionNames.ProfileDistributionManage,
                NameEn = "Profile Distribution - Manage",
                NameAr = "Profile Distribution - Manage",
                DisplayOrder = 11
            },
            new Permission
            {
                Id = PermissionIds.ProfileApprovalView,
                BackendName = PermissionNames.ProfileApprovalView,
                NameEn = "Profile Approval - View",
                NameAr = "Profile Approval - View",
                DisplayOrder = 12
            },
            new Permission
            {
                Id = PermissionIds.ProfileApprovalReview,
                BackendName = PermissionNames.ProfileApprovalReview,
                NameEn = "Profile Approval - Review",
                NameAr = "Profile Approval - Review",
                DisplayOrder = 13
            },
            new Permission
            {
                Id = PermissionIds.ProfileApprovalChanges,
                BackendName = PermissionNames.ProfileApprovalChanges,
                NameEn = "Profile Approval - Changes",
                NameAr = "Profile Approval - Changes",
                DisplayOrder = 14
            },
            new Permission
            {
                Id = PermissionIds.JobsView,
                BackendName = PermissionNames.JobsView,
                NameEn = "Jobs - View",
                NameAr = "Jobs - View",
                DisplayOrder = 15
            },
            new Permission
            {
                Id = PermissionIds.JobsManage,
                BackendName = PermissionNames.JobsManage,
                NameEn = "Jobs - Manage",
                NameAr = "Jobs - Manage",
                DisplayOrder = 16
            },
            new Permission
            {
                Id = PermissionIds.JobsApprove,
                BackendName = PermissionNames.JobsApprove,
                NameEn = "Jobs - Approve",
                NameAr = "Jobs - Approve",
                DisplayOrder = 17
            },
            new Permission
            {
                Id = PermissionIds.JobsPointsManage,
                BackendName = PermissionNames.JobsPointsManage,
                NameEn = "Jobs - Points Manage",
                NameAr = "Jobs - Points Manage",
                DisplayOrder = 18
            },
            new Permission
            {
                Id = PermissionIds.JobsInvitationsView,
                BackendName = PermissionNames.JobsInvitationsView,
                NameEn = "Jobs - Invitations View",
                NameAr = "Jobs - Invitations View",
                DisplayOrder = 19
            },
            new Permission
            {
                Id = PermissionIds.NominationsView,
                BackendName = PermissionNames.NominationsView,
                NameEn = "Nominations - View",
                NameAr = "Nominations - View",
                DisplayOrder = 20
            },
            new Permission
            {
                Id = PermissionIds.NominationsManage,
                BackendName = PermissionNames.NominationsManage,
                NameEn = "Nominations - Manage",
                NameAr = "Nominations - Manage",
                DisplayOrder = 21
            },
            new Permission
            {
                Id = PermissionIds.KawaderManage,
                BackendName = PermissionNames.KawaderManage,
                NameEn = "Kawader - Manage",
                NameAr = "Kawader - Manage",
                DisplayOrder = 22
            },
            new Permission
            {
                Id = PermissionIds.OfficeUsersView,
                BackendName = PermissionNames.OfficeUsersView,
                NameEn = "Office Users - View",
                NameAr = "Office Users - View",
                DisplayOrder = 23
            },
            new Permission
            {
                Id = PermissionIds.OfficeUsersManage,
                BackendName = PermissionNames.OfficeUsersManage,
                NameEn = "Office Users - Manage",
                NameAr = "Office Users - Manage",
                DisplayOrder = 24
            },
            new Permission
            {
                Id = PermissionIds.MajorSkillsManage,
                BackendName = PermissionNames.MajorSkillsManage,
                NameEn = "Major Skills - Manage",
                NameAr = "Major Skills - Manage",
                DisplayOrder = 25
            }
        );
    }
}
