namespace Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;

/// <summary>
/// Attribute-safe permission keys (compile-time constants).
/// Use ONLY in attributes and places that require const.
/// </summary>
public static class PermissionKeys
{
    public static class Dashboard
    {
        public const string View = "dashboard.view";
    }

    public static class Roles
    {
        public const string View = "roles.view";
        public const string Manage = "roles.manage";
    }

    public static class Users
    {
        public const string View = "users.view";
        public const string Manage = "users.manage";
    }

    public static class Offices
    {
        public const string View = "offices.view";
        public const string Manage = "offices.manage";
    }

    public static class Languages
    {
        public const string View = "languages.view";
        public const string Manage = "languages.manage";
    }

    public static class Countries
    {
        public const string View = "countries.view";
        public const string Manage = "countries.manage";
    }

    public static class Universities
    {
        public const string View = "universities.view";
        public const string Manage = "universities.manage";
    }

    public static class ProfileLogs
    {
        public const string View = "profile.logs.view";
    }

    public static class Profile
    {
        public const string View = "profile.view";
        public const string Manage = "profile.manage";
    }

    public static class ProfileDistribution
    {
        public const string View = "profile.distribution.view";
        public const string Manage = "profile.distribution.manage";
    }

    public static class ProfileApproval
    {
        public const string View = "profile.approval.view";
        public const string Review = "profile.approval.review";
        public const string Changes = "profile.approval.changes";
    }

    public static class Jobs
    {
        public const string View = "jobs.view";
        public const string Manage = "jobs.manage";
        public const string Approve = "jobs.approve";
    }

    public static class JobsPoints
    {
        public const string Manage = "jobs.points.manage";
    }

    public static class JobsInvitations
    {
        public const string View = "jobs.invitations.view";
    }

    public static class Nominations
    {
        public const string View = "nominations.view";
        public const string Manage = "nominations.manage";
    }

    public static class Kawader
    {
        public const string Manage = "kawader.manage";
    }

    public static class OfficeUsers
    {
        public const string View = "office.users.view";
        public const string Manage = "office.users.manage";
    }

    public static class MajorSkills
    {
        public const string Manage = "major-skill.management";
    }
}