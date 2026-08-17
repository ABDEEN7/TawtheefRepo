namespace Tawtheef.Application.Common.Security;

/// <summary>
/// Attribute-safe permission keys (compile-time constants).
/// Use ONLY in attributes and places that require const.
/// </summary>
public static class PermissionKeys
{
    public static class Dashboard
    {
        public const string View = "dashboard.view";
        public const string Export = "dashboard.export";
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

    public static class CandidateUsers
    {
        public const string View = "candidate.users.view";
        public const string Manage = "candidate.users.manage";
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

    public static class Religions
    {
        public const string View = "religions.view";
        public const string Manage = "religions.manage";
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

    public static class Cities
    {
        public const string View = "cities.view";
        public const string Manage = "cities.manage";
    }

    public static class ProfileLogs
    {
        public const string View = "profile.logs.view";
    }

    public static class TargetEntities
    {
        public const string View = "targetentities.view";
        public const string Manage = "targetentities.manage";
    }

    public static class HomeContent
    {
        public const string View = "home.content.view";
        public const string Manage = "home.content.manage";
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
        public const string Edit = "jobs.edit";
        public const string Approve = "jobs.approve";
        public const string SendInvitation = "jobs.send-invitation";
        public const string Cancel = "jobs.cancel";
        public const string Create = "jobs.create";
        public const string Publish = "jobs.publish";
        public const string Delete = "jobs.delete";
        public const string Clone = "jobs.clone";
    }

    public static class JobsPoints
    {
        public const string View = "jobs.points.view";
        public const string Edit = "jobs.points.edit";
        public const string Approve = "jobs.points.approve";
    }

    public static class JobsInvitations
    {
        public const string View = "jobs.invitations.view";
        public const string ManageAttachment = "jobs.invitations.manage-attachment";
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
        public const string View = "major-skill.view";
        public const string Manage = "major-skill.management";
    }

    public static class OrganizationStructures
    {
        public const string Manage = "organization-structures.manage";
    }

    public static class MinisterOffice
    {
        public const string View = "minister-office.view";
        public const string Manage = "minister-office.manage";
    }

    public static class JobTitles
    {
        public const string View = "job-titles.view";
        public const string Manage = "job-titles.manage";
    }

    public static class JobCategoryCandidateSettings
    {
        public const string View = "job-category-candidate-settings.view";
        public const string Manage = "job-category-candidate-settings.manage";
    }

    public static class JobPointsConfiguration
    {
        public const string View = "job-points-configuration.view";
        public const string Manage = "job-points-configuration.manage";
    }

    public static class InvitationExpiryConfiguration
    {
        public const string View = "invitation-expiry-configuration.view";
        public const string Manage = "invitation-expiry-configuration.manage";
    }
}
