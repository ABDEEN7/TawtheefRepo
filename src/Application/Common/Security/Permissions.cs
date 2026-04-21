using Tawtheef.Domain.Entities.Security;

namespace Tawtheef.Application.Common.Security;

public static class Permissions
{
    private static PermissionDefinition Def(
        string key,
        string module,
        PermissionAction action,
        string nameEn,
        string nameAr,
        int order,
        bool canBeAssignedToRole = true)
        => new(new PermissionKey(key), module, action, nameEn, nameAr, order, canBeAssignedToRole);

    // =========================
    // Admin Console
    // =========================

    public static class Dashboard
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Dashboard.View,
                nameof(PermissionKeys.Dashboard),
                PermissionAction.View,
                "Dashboard - View",
                "لوحة التحكم - عرض",
                1);
    }

    public static class Roles
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Roles.View,
                nameof(PermissionKeys.Roles),
                PermissionAction.View,
                "Roles - View",
                "الأدوار - عرض",
                2,
                false);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Roles.Manage,
                nameof(PermissionKeys.Roles),
                PermissionAction.Manage,
                "Roles - Manage",
                "الأدوار - إدارة",
                3,
                false);
    }

    public static class Users
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Users.View,
                nameof(PermissionKeys.Users),
                PermissionAction.View,
                "Users - View",
                "المستخدمون - عرض",
                4,
                false);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Users.Manage,
                nameof(PermissionKeys.Users),
                PermissionAction.Manage,
                "Users - Manage",
                "المستخدمون - إدارة",
                5,
                false);
    }

    public static class CandidateUsers
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.CandidateUsers.View,
                nameof(PermissionKeys.CandidateUsers),
                PermissionAction.View,
                "Candidate Users - View",
                "مستخدمو المرشحين - عرض",
                18);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.CandidateUsers.Manage,
                nameof(PermissionKeys.CandidateUsers),
                PermissionAction.Manage,
                "Candidate Users - Manage",
                "مستخدمو المرشحين - إدارة",
                19);
    }

    public static class Offices
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Offices.View,
                nameof(PermissionKeys.Offices),
                PermissionAction.View,
                "Offices - View",
                "المكاتب - عرض",
                6);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Offices.Manage,
                nameof(PermissionKeys.Offices),
                PermissionAction.Manage,
                "Offices - Manage",
                "المكاتب - إدارة",
                7);
    }

    public static class Languages
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Languages.View,
                nameof(PermissionKeys.Languages),
                PermissionAction.View,
                "Languages - View",
                "اللغات - عرض",
                8);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Languages.Manage,
                nameof(PermissionKeys.Languages),
                PermissionAction.Manage,
                "Languages - Manage",
                "اللغات - إدارة",
                9);
    }

    public static class Religions
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Religions.View,
                nameof(PermissionKeys.Religions),
                PermissionAction.View,
                "Religions - View",
                "الديانات - عرض",
                10);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Religions.Manage,
                nameof(PermissionKeys.Religions),
                PermissionAction.Manage,
                "Religions - Manage",
                "الديانات - إدارة",
                11);
    }

    public static class Countries
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Countries.View,
                nameof(PermissionKeys.Countries),
                PermissionAction.View,
                "Countries - View",
                "الدول - عرض",
                12);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Countries.Manage,
                nameof(PermissionKeys.Countries),
                PermissionAction.Manage,
                "Countries - Manage",
                "الدول - إدارة",
                13);
    }

    public static class Universities
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Universities.View,
                nameof(PermissionKeys.Universities),
                PermissionAction.View,
                "Universities - View",
                "الجامعات - عرض",
                14);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Universities.Manage,
                nameof(PermissionKeys.Universities),
                PermissionAction.Manage,
                "Universities - Manage",
                "الجامعات - إدارة",
                15);
    }

    public static class TargetEntities
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.TargetEntities.View,
                nameof(PermissionKeys.TargetEntities),
                PermissionAction.View,
                "Target Entities - View",
                "الجهات المستهدفة - عرض",
                16);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.TargetEntities.Manage,
                nameof(PermissionKeys.TargetEntities),
                PermissionAction.Manage,
                "Target Entities - Manage",
                "الجهات المستهدفة - إدارة",
                17);
    }

    public static class HomeContent
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.HomeContent.View,
                nameof(PermissionKeys.HomeContent),
                PermissionAction.View,
                "Home Content - View",
                "محتوى الصفحة الرئيسية - عرض",
                20,
                false);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.HomeContent.Manage,
                nameof(PermissionKeys.HomeContent),
                PermissionAction.Manage,
                "Home Content - Manage",
                "محتوى الصفحة الرئيسية - إدارة",
                21,
                false);
    }

    public static class ProfileLogs
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.ProfileLogs.View,
                nameof(PermissionKeys.ProfileLogs),
                PermissionAction.View,
                "Profile Logs - View",
                "سجل الملفات - عرض",
                14);
    }

    // =========================
    // Employee Console
    // =========================

    public static class Profile
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Profile.View,
                nameof(PermissionKeys.Profile),
                PermissionAction.View,
                "Profile - View",
                "الملف الشخصي - عرض",
                20);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Profile.Manage,
                nameof(PermissionKeys.Profile),
                PermissionAction.Manage,
                "Profile - Manage",
                "الملف الشخصي - إدارة",
                21);
    }

    public static class ProfileDistribution
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.ProfileDistribution.View,
                nameof(PermissionKeys.ProfileDistribution),
                PermissionAction.View,
                "Profile Distribution - View",
                "توزيع الملفات - عرض",
                22);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.ProfileDistribution.Manage,
                nameof(PermissionKeys.ProfileDistribution),
                PermissionAction.Manage,
                "Profile Distribution - Manage",
                "توزيع الملفات - إدارة",
                23);
    }

    public static class ProfileApproval
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.ProfileApproval.View,
                nameof(PermissionKeys.ProfileApproval),
                PermissionAction.View,
                "Profile Approval - View",
                "اعتماد الملفات - عرض",
                24);

        public static readonly PermissionDefinition Review =
            Def(
                PermissionKeys.ProfileApproval.Review,
                nameof(PermissionKeys.ProfileApproval),
                PermissionAction.Review,
                "Profile Approval - Review",
                "اعتماد الملفات - مراجعة",
                25);

        public static readonly PermissionDefinition Changes =
            Def(
                PermissionKeys.ProfileApproval.Changes,
                nameof(PermissionKeys.ProfileApproval),
                PermissionAction.Changes,
                "Profile Approval - Changes",
                "اعتماد الملفات - تعديلات",
                26);
    }

    public static class Jobs
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Jobs.View,
                nameof(PermissionKeys.Jobs),
                PermissionAction.View,
                "Jobs - View",
                "الوظائف - عرض",
                30);

        public static readonly PermissionDefinition Edit =
            Def(
                PermissionKeys.Jobs.Edit,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Manage,
                "Jobs - Edit",
                "الوظائف - تعديل",
                31);

        public static readonly PermissionDefinition Approve =
            Def(
                PermissionKeys.Jobs.Approve,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Approve,
                "Jobs - Approve",
                "الوظائف - اعتماد",
                32);

        public static readonly PermissionDefinition SendInvitation =
            Def(
                PermissionKeys.Jobs.SendInvitation,
                nameof(PermissionKeys.Jobs),
                PermissionAction.SendInvitation,
                "Jobs - Send Invitation",
                "الوظائف - ارسال الدعوات",
                32);

        public static readonly PermissionDefinition Cancel =
            Def(
                PermissionKeys.Jobs.Cancel,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Cancel,
                "Jobs - Cancel",
                "الوظائف - إلغاء",
                33);

        public static readonly PermissionDefinition Create =
            Def(
                PermissionKeys.Jobs.Create,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Create,
                "Jobs - Create",
                "الوظائف - إنشاء",
                34);

        public static readonly PermissionDefinition Publish =
            Def(
                PermissionKeys.Jobs.Publish,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Publish,
                "Jobs - Publish",
                "الوظائف - نشر",
                35);

        public static readonly PermissionDefinition Delete =
            Def(
                PermissionKeys.Jobs.Delete,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Delete,
                "Jobs - Delete",
                "الوظائف - حذف",
                36);

        public static readonly PermissionDefinition Clone =
            Def(
                PermissionKeys.Jobs.Clone,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Clone,
                "Jobs - Clone",
                "الوظائف - استنساخ",
                37);
    }

    public static class JobsPoints
    {
        public static readonly PermissionDefinition Edit =
            Def(
                PermissionKeys.JobsPoints.Edit,
                nameof(PermissionKeys.JobsPoints),
                PermissionAction.Manage,
                "Jobs - Points Edit",
                "نقاط الوظائف - تعديل",
                41);

        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.JobsPoints.View,
                nameof(PermissionKeys.JobsPoints),
                PermissionAction.View,
                "Jobs Points - View",
                "نقاط الوظائف - عرض",
                33);

        public static readonly PermissionDefinition Approve =
            Def(
                PermissionKeys.JobsPoints.Approve,
                nameof(PermissionKeys.JobsPoints),
                PermissionAction.Approve,
                "Jobs Points - Approve",
                "نقاط الوظائف - إعتماد",
                33);
    }

    public static class JobsInvitations
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.JobsInvitations.View,
                nameof(PermissionKeys.JobsInvitations),
                PermissionAction.View,
                "Jobs Invitations - View",
                "دعوات الوظائف - عرض",
                34);

        public static readonly PermissionDefinition ManageAttachment =
            Def(
                PermissionKeys.JobsInvitations.ManageAttachment,
                nameof(PermissionKeys.JobsInvitations),
                PermissionAction.Manage,
                "Jobs Invitations - Manage Attachment",
                "دعوات الوظائف - إدارة المرفقات",
                34);
    }

    public static class Kawader
    {
        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Kawader.Manage,
                nameof(PermissionKeys.Kawader),
                PermissionAction.Manage,
                "Kawader - Manage",
                "الكوادر - إدارة",
                50);
    }

    public static class OfficeUsers
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.OfficeUsers.View,
                nameof(PermissionKeys.OfficeUsers),
                PermissionAction.View,
                "Office Users - View",
                "مستخدمو المكتب - عرض",
                60,
                false);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.OfficeUsers.Manage,
                nameof(PermissionKeys.OfficeUsers),
                PermissionAction.Manage,
                "Office Users - Manage",
                "مستخدمو المكتب - إدارة",
                61,
                false);
    }

    public static class MajorSkills
    {
        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.MajorSkills.Manage,
                nameof(PermissionKeys.MajorSkills),
                PermissionAction.Manage,
                "Major Skills - Manage",
                "مهارات التخصص - إدارة",
                70);
    }

    public static class OrganizationStructures
    {
        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.OrganizationStructures.Manage,
                nameof(PermissionKeys.OrganizationStructures),
                PermissionAction.Manage,
                "Organization Structures - Manage",
                "الهياكل التنظيمية - إدارة",
                80);
    }

    public static class MinisterOffice
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.MinisterOffice.View,
                nameof(PermissionKeys.MinisterOffice),
                PermissionAction.View,
                "Minister Office - View",
                "مكتب سعادة الوزير - عرض",
                90);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.MinisterOffice.Manage,
                nameof(PermissionKeys.MinisterOffice),
                PermissionAction.Manage,
                "Minister Office - Manage",
                "مكتب سعادة الوزير - إدارة",
                91);
    }

    public static class Cities
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Cities.View,
                nameof(PermissionKeys.Cities),
                PermissionAction.View,
                "Cities - View",
                "المدن - عرض",
                92);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Cities.Manage,
                nameof(PermissionKeys.Cities),
                PermissionAction.Manage,
                "Cities - Manage",
                "المدن - إدارة",
                93);
    }

    public static class JobTitles
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.JobTitles.View,
                nameof(PermissionKeys.JobTitles),
                PermissionAction.View,
                "Job Titles - View",
                "المسميات الوظيفية - عرض",
                94);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.JobTitles.Manage,
                nameof(PermissionKeys.JobTitles),
                PermissionAction.Manage,
                "Job Titles - Manage",
                "المسميات الوظيفية - إدارة",
                95);
    }

    public static class JobCategoryCandidateSettings
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.JobCategoryCandidateSettings.View,
                nameof(PermissionKeys.JobCategoryCandidateSettings),
                PermissionAction.View,
                "Job Category Candidate Settings - View",
                "إعدادات المرشحين حسب تصنيف الوظيفة - عرض",
                96);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.JobCategoryCandidateSettings.Manage,
                nameof(PermissionKeys.JobCategoryCandidateSettings),
                PermissionAction.Manage,
                "Job Category Candidate Settings - Manage",
                "إعدادات المرشحين حسب تصنيف الوظيفة - إدارة",
                97);

    }

    public static class JobPointsConfiguration
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.JobPointsConfiguration.View,
                nameof(PermissionKeys.JobPointsConfiguration),
                PermissionAction.View,
                "Job Points Configuration - View",
                "إعدادات نقاط الوظيفة - عرض",
                98);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.JobPointsConfiguration.Manage,
                nameof(PermissionKeys.JobPointsConfiguration),
                PermissionAction.Manage,
                "Job Points Configuration - Manage",
                "إعدادات نقاط الوظيفة - إدارة",
                99);
    }
}
