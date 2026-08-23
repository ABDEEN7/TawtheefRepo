import { Permissions } from "../../../core/constants/permissions";
import { routes } from "../../../routes/routes";

export interface MenuItem {
  key: string;
  label: string;
  icon: string;
  route: string;
  permission: string | string[];
}

export class Sidebar {
  static menuItems: MenuItem[] = [
    { key: 'home', label: 'internal.sidebar.home', icon: 'hgi-home-05', route: routes.portal.dashboard, permission: Permissions.Dashboard.View },

    { key: 'roles', label: 'admin.sidebar.roles', icon: 'hgi-security-validation', route: routes.portal.roleManagement, permission: Permissions.Roles.Manage },
    { key: 'users', label: 'admin.sidebar.users', icon: ' hgi-file-star', route: routes.portal.usersManagement, permission: Permissions.Users.Manage },

    { key: 'distribution', label: 'internal.sidebar.distribution', icon: 'hgi-mail-send-02', route: routes.portal.profileDistribution, permission: Permissions.ProfileDistribution.View },
    { key: 'approve-profile', label: 'internal.sidebar.approve-profile', icon: 'hgi-task-done-01', route: routes.portal.approvalProfile, permission: Permissions.ProfileApproval.View },
    { key: 'job', label: 'internal.sidebar.job', icon: 'hgi-ai-beautify', route: routes.portal.JobList, permission: Permissions.Jobs.View },
    { key: 'invitation-summary', label: 'internal.sidebar.invitation-summary', icon: 'hgi-ai-setting', route: routes.portal.jobInvitationSummary, permission: Permissions.JobInvitations.View },
    { key: 'exceptions', label: 'internal.sidebar.exceptions', icon: 'hgi-file-validation', route: routes.portal.exceptions, permission: Permissions.Exceptions.View },
    { key: 'candidate-users', label: 'internal.sidebar.candidate-users', icon: 'hgi-user-switch', route: routes.portal.candidateUsersManagement, permission: Permissions.CandidateUsers.View },
    { key: 'office-users', label: 'internal.sidebar.office-users', icon: 'hgi-file-star', route: routes.portal.officeUsersManagement, permission: Permissions.OfficeUsers.View },
    { key: 'organization-structures', label: 'internal.sidebar.organization-structures', icon: 'hgi-structure-03', route: routes.portal.organizationStructures, permission: Permissions.OrganizationStructures.Manage },
    { key: 'major-skill', label: 'internal.sidebar.major-skill', icon: 'hgi-list-setting', route: routes.portal.majorsSkillsManagement, permission: [Permissions.MajorSkills.View, Permissions.MajorSkills.Manage] },
    { key: 'kawader', label: 'internal.sidebar.kawader', icon: ' hgi-user-multiple', route: routes.portal.kawader, permission: Permissions.Kawader.Manage },
    { key: 'minister-office', label: 'internal.sidebar.ministerOffice', icon: 'hgi-office', route: routes.portal.ministerOfficeManagement, permission: Permissions.MinisterOffice.View },

    { key: 'offices', label: 'admin.sidebar.offices', icon: 'hgi-mail-send-02', route: routes.portal.officesManagement, permission: Permissions.Offices.Manage },
    { key: 'countries', label: 'admin.sidebar.countries', icon: 'hgi-globe-02', route: routes.portal.countriesManagement, permission: Permissions.Countries.Manage },
    { key: 'languages', label: 'admin.sidebar.languages', icon: 'hgi-arrow-data-transfer-horizontal', route: routes.portal.languagesManagement, permission: Permissions.Languages.Manage },
    { key: 'target-entities', label: 'admin.sidebar.targetEntities', icon: 'hgi-arrow-data-transfer-horizontal', route: routes.portal.targetEntitiesManagement, permission: Permissions.TargetEntities.Manage },
    { key: 'religions', label: 'admin.sidebar.religions', icon: 'hgi-structure-03', route: routes.portal.religionsManagement, permission: Permissions.Religions.Manage },
    { key: 'universities', label: 'admin.sidebar.universities', icon: ' hgi-university', route: routes.portal.universitiesManagement, permission: Permissions.Universities.View },
    { key: 'job-points-configuration', label: 'admin.sidebar.jobPointsConfig', icon: 'hgi-ai-beautify', route: routes.portal.jobPointsConfiguration, permission: Permissions.JobPointsConfiguration.View },
    { key: 'job-category-candidate-settings', label: 'admin.sidebar.jobCategoryCandidateSettings', icon: 'hgi-settings-02', route: routes.portal.jobCategoryCandidateSettings, permission: Permissions.JobCategoryCandidateSettings.View },
    { key: 'invitation-expiry-configuration', label: 'admin.sidebar.invitationExpiryConfiguration', icon: 'hgi-calendar-03', route: routes.portal.invitationExpiryConfiguration, permission: Permissions.InvitationExpiryConfiguration.View },
    { key: 'job-titles', label: 'admin.sidebar.jobTitles', icon: 'hgi-license-draft', route: routes.portal.jobTitlesManagement, permission: Permissions.JobTitles.View },

    { key: 'home-content', label: 'admin.sidebar.homeContent', icon: 'hgi-settings-02', route: routes.portal.homeContentManagement, permission: Permissions.HomeContent.Manage },

    { key: 'profileLogs', label: 'admin.sidebar.profileLogs', icon: 'hgi-mail-send-02', route: routes.portal.profileLogs, permission: Permissions.ProfileLogs.View },
    { key: 'systemAdminLogs', label: 'admin.sidebar.systemAdminLogs', icon: 'hgi-audit-01', route: routes.portal.systemAdminLogs, permission: Permissions.ProfileLogs.View },
  ];
}
