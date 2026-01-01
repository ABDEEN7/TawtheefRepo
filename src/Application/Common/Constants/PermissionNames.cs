namespace Tawtheef.Application.Common.Constants;

public static class PermissionNames
{
    #region Admin Console Permissions
    // Dashboard
    public const string DashboardView = "dashboard.view";

    // Roles
    public const string RolesView = "roles.view";
    public const string RolesManage = "roles.manage";

    //User
    public const string UsersView = "users.view";
    public const string UsersManage = "users.manage";
    
    //Offices
    public const string OfficesView = "offices.view";
    public const string OfficesManage = "offices.manage";

    #endregion

    #region Employee Console Permissions
    // Profile
    public const string ProfileView = "profile.view";
    public const string ProfileManage = "profile.manage";
    public const string ProfileDistributionView = "profile.distribution.view";
    public const string ProfileDistributionManage = "profile.distribution.manage";
    public const string ProfileApprovalView = "profile.approval.view";
    public const string ProfileApprovalReview = "profile.approval.review";
    public const string ProfileApprovalChanges = "profile.approval.changes";

    // Jobs
    public const string JobsView = "jobs.view";
    public const string JobsManage = "jobs.manage";
    public const string JobsApprove = "jobs.approve";
    public const string JobsPointsManage = "jobs.points.manage";
    public const string JobsInvitationsView = "jobs.invitations.view";

    // Nominations
    public const string NominationsView = "nominations.view";
    public const string NominationsManage = "nominations.manage";

    // Kawader
    public const string KawaderManage = "kawader.manage";
    
    // Offices Users
    public const string OfficeUsersView = "office.users.view";
    public const string OfficeUsersManage = "office.users.manage";
    public const string MajorSkillsManage = "major-skill.management";
    #endregion
    
    
    public static List<string> GeneratePermissionsForModule(string module)
    {
        return
        [
            $"permissions.{module}.create",
            $"permissions.{module}.view",
            $"permissions.{module}.edit",
            $"permissions.{module}.delete"
        ];
    }
    
    public static readonly IReadOnlyList<string> All =
    [
        DashboardView,
        RolesView, RolesManage,
        UsersView, UsersManage,
        OfficesView, OfficesManage,
        ProfileView, ProfileManage,
        ProfileDistributionView, ProfileDistributionManage,
        ProfileApprovalView, ProfileApprovalReview, ProfileApprovalChanges,
        JobsView, JobsManage, JobsApprove, JobsPointsManage, JobsInvitationsView,
        NominationsView, NominationsManage,
        KawaderManage,
        OfficeUsersView, OfficeUsersManage,
        MajorSkillsManage
    ];

    public static readonly IReadOnlyList<string> SystemAdminPermissions =
    [
        DashboardView,
        RolesView,
        RolesManage,
        UsersView,
        UsersManage,
        OfficesView,
        OfficesManage,
        OfficeUsersView,
        OfficeUsersManage,
        KawaderManage,
        MajorSkillsManage
    ];
    
    public static readonly IReadOnlyList<string> EmployeePermissions =
    [
        DashboardView,
    ];
    
    public static readonly IReadOnlyList<string> OfficeAdminPermissions =
    [
        DashboardView,
        OfficeUsersView,
        OfficeUsersManage
    ];
    
    public static readonly IReadOnlyList<string> OfficeUserPermissions =
    [
        DashboardView,
    ];
    
    public static readonly IReadOnlyList<string> EmployeeSuperAdminPermissions =
    [
        DashboardView,
        ProfileView,
        ProfileManage,
        ProfileDistributionView,
        ProfileDistributionManage,
        ProfileApprovalView,
        ProfileApprovalReview,
        ProfileApprovalChanges,
        JobsView,
        JobsManage,
        JobsApprove,
        JobsPointsManage,
        JobsInvitationsView,
        NominationsView,
        NominationsManage,
        KawaderManage,
        OfficeUsersView,
        OfficeUsersManage,
        MajorSkillsManage
    ];
}
