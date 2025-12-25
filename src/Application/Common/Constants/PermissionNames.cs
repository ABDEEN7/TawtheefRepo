namespace Tawtheef.Application.Common.Constants;

public static class PermissionNames
{
    //User
    public const string UsersView = "users.view";
    public const string UsersManage = "users.manage";
    
    // Profile
    public const string ProfileView = "profile.view";
    public const string ProfileManage = "profile.manage";
    
    // Jobs
    public const string JobsView = "jobs.view";
    public const string JobsManage = "jobs.manage";
    
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
        UsersView, UsersManage,
        ProfileView, ProfileManage,
        JobsView, JobsManage
    ];
}
