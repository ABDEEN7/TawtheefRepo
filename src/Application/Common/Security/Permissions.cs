using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
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
        int order)
        => new(new PermissionKey(key), module, action, nameEn, nameAr, order);

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
                2);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Roles.Manage,
                nameof(PermissionKeys.Roles),
                PermissionAction.Manage,
                "Roles - Manage",
                "الأدوار - إدارة",
                3);
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
                4);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Users.Manage,
                nameof(PermissionKeys.Users),
                PermissionAction.Manage,
                "Users - Manage",
                "المستخدمون - إدارة",
                5);
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

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Jobs.Manage,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Manage,
                "Jobs - Manage",
                "الوظائف - إدارة",
                31);

        public static readonly PermissionDefinition Approve =
            Def(
                PermissionKeys.Jobs.Approve,
                nameof(PermissionKeys.Jobs),
                PermissionAction.Approve,
                "Jobs - Approve",
                "الوظائف - اعتماد",
                32);
    }

    public static class JobsPoints
    {
        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.JobsPoints.Manage,
                nameof(PermissionKeys.JobsPoints),
                PermissionAction.Manage,
                "Jobs Points - Manage",
                "نقاط الوظائف - إدارة",
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
    }

    public static class Nominations
    {
        public static readonly PermissionDefinition View =
            Def(
                PermissionKeys.Nominations.View,
                nameof(PermissionKeys.Nominations),
                PermissionAction.View,
                "Nominations - View",
                "الترشيحات - عرض",
                40);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.Nominations.Manage,
                nameof(PermissionKeys.Nominations),
                PermissionAction.Manage,
                "Nominations - Manage",
                "الترشيحات - إدارة",
                41);
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
                60);

        public static readonly PermissionDefinition Manage =
            Def(
                PermissionKeys.OfficeUsers.Manage,
                nameof(PermissionKeys.OfficeUsers),
                PermissionAction.Manage,
                "Office Users - Manage",
                "مستخدمو المكتب - إدارة",
                61);
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
}
