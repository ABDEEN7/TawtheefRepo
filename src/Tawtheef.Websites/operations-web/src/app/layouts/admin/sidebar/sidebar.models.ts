import { InterviewDashboardPermissions, Permissions } from '../../../core/constants/permissions';
import { routes } from '../../../routes/routes';

export interface MenuItem {
  key: string;
  label: string;
  icon: string;
  route: string;
  permission: string | string[];
}

export interface MenuGroup {
  key: string;
  label: string;
  icon: string;
  children: MenuItem[];
}

export type SidebarItem = MenuItem | MenuGroup;

export function isMenuGroup(item: SidebarItem): item is MenuGroup {
  return 'children' in item;
}

export class Sidebar {
  static menuItems: MenuItem[] = [
    {
      key: 'home',
      label: 'internal.sidebar.home',
      icon: 'hgi-home-05',
      route: routes.portal.dashboard,
      permission: Permissions.Dashboard.View,
    },

    {
      key: 'roles',
      label: 'admin.sidebar.roles',
      icon: 'hgi-security-validation',
      route: routes.portal.roleManagement,
      permission: Permissions.Roles.Manage,
    },
    {
      key: 'users',
      label: 'admin.sidebar.users',
      icon: ' hgi-file-star',
      route: routes.portal.usersManagement,
      permission: Permissions.Users.Manage,
    },

    {
      key: 'distribution',
      label: 'internal.sidebar.distribution',
      icon: 'hgi-mail-send-02',
      route: routes.portal.profileDistribution,
      permission: Permissions.ProfileDistribution.View,
    },
    {
      key: 'approve-profile',
      label: 'internal.sidebar.approve-profile',
      icon: 'hgi-task-done-01',
      route: routes.portal.approvalProfile,
      permission: Permissions.ProfileApproval.View,
    },
    {
      key: 'job',
      label: 'internal.sidebar.job',
      icon: 'hgi-ai-beautify',
      route: routes.portal.JobList,
      permission: Permissions.Jobs.View,
    },
    {
      key: 'invitation-summary',
      label: 'internal.sidebar.invitation-summary',
      icon: 'hgi-ai-setting',
      route: routes.portal.jobInvitationSummary,
      permission: Permissions.JobInvitations.View,
    },
    {
      key: 'exceptions',
      label: 'internal.sidebar.exceptions',
      icon: 'hgi-file-validation',
      route: routes.portal.exceptions,
      permission: Permissions.Exceptions.View,
    },
    {
      key: 'candidate-users',
      label: 'internal.sidebar.candidate-users',
      icon: 'hgi-user-switch',
      route: routes.portal.candidateUsersManagement,
      permission: Permissions.CandidateUsers.View,
    },
    {
      key: 'office-users',
      label: 'internal.sidebar.office-users',
      icon: 'hgi-file-star',
      route: routes.portal.officeUsersManagement,
      permission: Permissions.OfficeUsers.View,
    },
    {
      key: 'organization-structures',
      label: 'internal.sidebar.organization-structures',
      icon: 'hgi-structure-03',
      route: routes.portal.organizationStructures,
      permission: Permissions.OrganizationStructures.Manage,
    },
    {
      key: 'major-skill',
      label: 'internal.sidebar.major-skill',
      icon: 'hgi-list-setting',
      route: routes.portal.majorsSkillsManagement,
      permission: [Permissions.MajorSkills.View, Permissions.MajorSkills.Manage],
    },
    {
      key: 'kawader',
      label: 'internal.sidebar.kawader',
      icon: ' hgi-user-multiple',
      route: routes.portal.kawader,
      permission: Permissions.Kawader.Manage,
    },
    {
      key: 'minister-office',
      label: 'internal.sidebar.ministerOffice',
      icon: 'hgi-office',
      route: routes.portal.ministerOfficeManagement,
      permission: Permissions.MinisterOffice.View,
    },
    {
      key: 'locations',
      label: 'internal.sidebar.locations',
      icon: 'hgi-location-01',
      route: routes.portal.locationsManagement,
      permission: Permissions.Locations.View,
    },
    {
      key: 'rooms',
      label: 'internal.sidebar.rooms',
      icon: 'hgi-building-03',
      route: routes.portal.roomsManagement,
      permission: Permissions.Rooms.View,
    },
    {
      key: 'exams',
      label: 'internal.sidebar.exams',
      icon: 'hgi-task-01',
      route: routes.portal.examsManagement,
      permission: Permissions.Exams.View,
    },
    {
      key: 'exam-results',
      label: 'internal.sidebar.exam-results',
      icon: 'hgi-task-done-01',
      route: routes.portal.examResults,
      permission: Permissions.Exams.View,
    },
    {
      key: 'question-bank-assignments',
      label: 'internal.sidebar.question-bank-assignments',
      icon: 'hgi-task-daily-01',
      route: routes.portal.questionBankAssignments,
      permission: Permissions.QuestionBankAssignments.Manage,
    },
    {
      key: 'question-banks',
      label: 'internal.sidebar.question-banks',
      icon: 'hgi-file-validation',
      route: routes.portal.questionBanks,
      permission: Permissions.QuestionBanks.View,
    },
    {
      key: 'question-bank-requests',
      label: 'internal.sidebar.question-bank-requests',
      icon: 'hgi-task-01',
      route: routes.portal.questionBankRequests,
      permission: Permissions.QuestionBankRequests.View,
    },

    {
      key: 'test-slots',
      label: 'internal.sidebar.testSlots',
      icon: 'hgi-calendar-03',
      route: routes.portal.testSlotsManagement,
      permission: Permissions.TestSlots.View,
    },
    {
      key: 'interview-dashboard',
      label: 'internal.sidebar.interview-dashboard',
      icon: 'hgi-dashboard-square-02',
      route: routes.portal.interviewDashboard,
      permission: InterviewDashboardPermissions,
    },
    {
      key: 'test-sessions',
      label: 'internal.sidebar.testSessions',
      icon: 'hgi-calendar-03',
      route: routes.portal.testSessionsManagement,
      permission: Permissions.TestSessions.View,
    },

    {
      key: 'interview-axes-criteria',
      label: 'internal.sidebar.interview-axes-criteria',
      icon: 'hgi-chart-line-data-01',
      route: routes.portal.interviewAxesCriteria,
      permission: [
        Permissions.InterviewEvaluationBank.View,
        Permissions.InterviewEvaluationBank.Manage,
      ],
    },
    {
      key: 'interview-template',
      label: 'internal.sidebar.interview-template',
      icon: 'hgi-doc-02',
      route: routes.portal.interviewTemplates,
      permission: [
        Permissions.InterviewEvaluationTemplate.View,
        Permissions.InterviewEvaluationTemplate.Manage,
      ],
    },
    {
      key: 'interview-committee',
      label: 'internal.sidebar.interview-committee',
      icon: 'hgi-user-group',
      route: routes.portal.interviewCommittees,
      permission: [Permissions.InterviewCommittee.View, Permissions.InterviewCommittee.Manage],
    },
    {
      key: 'interviews',
      label: 'internal.sidebar.interviews',
      icon: 'hgi-user-question-02',
      route: routes.portal.interviews,
      permission: [Permissions.InterviewSchedule.View, Permissions.InterviewSchedule.Manage],
    },
    {
      key: 'start-interview',
      label: 'internal.sidebar.start-interview',
      icon: 'hgi-task-edit-01',
      route: routes.portal.startInterview,
      permission: [Permissions.InterviewEvaluation.View, Permissions.InterviewEvaluation.Manage],
    },
    {
      key: 'approve-interview',
      label: 'internal.sidebar.approve-interview',
      icon: 'hgi-task-done-01',
      route: routes.portal.approveInterview,
      permission: [
        Permissions.InterviewResultReport.View,
        Permissions.InterviewResultReport.Manage,
      ],
    },

    {
      key: 'offices',
      label: 'admin.sidebar.offices',
      icon: 'hgi-mail-send-02',
      route: routes.portal.officesManagement,
      permission: Permissions.Offices.Manage,
    },
    {
      key: 'countries',
      label: 'admin.sidebar.countries',
      icon: 'hgi-globe-02',
      route: routes.portal.countriesManagement,
      permission: Permissions.Countries.Manage,
    },
    {
      key: 'languages',
      label: 'admin.sidebar.languages',
      icon: 'hgi-arrow-data-transfer-horizontal',
      route: routes.portal.languagesManagement,
      permission: Permissions.Languages.Manage,
    },
    {
      key: 'target-entities',
      label: 'admin.sidebar.targetEntities',
      icon: 'hgi-arrow-data-transfer-horizontal',
      route: routes.portal.targetEntitiesManagement,
      permission: Permissions.TargetEntities.Manage,
    },
    {
      key: 'religions',
      label: 'admin.sidebar.religions',
      icon: 'hgi-structure-03',
      route: routes.portal.religionsManagement,
      permission: Permissions.Religions.Manage,
    },
    {
      key: 'universities',
      label: 'admin.sidebar.universities',
      icon: ' hgi-university',
      route: routes.portal.universitiesManagement,
      permission: Permissions.Universities.View,
    },
    {
      key: 'job-points-configuration',
      label: 'admin.sidebar.jobPointsConfig',
      icon: 'hgi-ai-beautify',
      route: routes.portal.jobPointsConfiguration,
      permission: Permissions.JobPointsConfiguration.View,
    },
    {
      key: 'job-category-candidate-settings',
      label: 'admin.sidebar.jobCategoryCandidateSettings',
      icon: 'hgi-settings-02',
      route: routes.portal.jobCategoryCandidateSettings,
      permission: Permissions.JobCategoryCandidateSettings.View,
    },
    {
      key: 'invitation-expiry-configuration',
      label: 'admin.sidebar.invitationExpiryConfiguration',
      icon: 'hgi-calendar-03',
      route: routes.portal.invitationExpiryConfiguration,
      permission: Permissions.InvitationExpiryConfiguration.View,
    },
    {
      key: 'job-titles',
      label: 'admin.sidebar.jobTitles',
      icon: 'hgi-license-draft',
      route: routes.portal.jobTitlesManagement,
      permission: Permissions.JobTitles.View,
    },

    {
      key: 'home-content',
      label: 'admin.sidebar.homeContent',
      icon: 'hgi-settings-02',
      route: routes.portal.homeContentManagement,
      permission: Permissions.HomeContent.Manage,
    },

    {
      key: 'profileLogs',
      label: 'admin.sidebar.profileLogs',
      icon: 'hgi-mail-send-02',
      route: routes.portal.profileLogs,
      permission: Permissions.ProfileLogs.View,
    },
    {
      key: 'systemAdminLogs',
      label: 'admin.sidebar.systemAdminLogs',
      icon: 'hgi-audit-01',
      route: routes.portal.systemAdminLogs,
      permission: Permissions.ProfileLogs.View,
    },
  ];

  // Keep the existing page definitions and their order as the single source for
  // routes, permissions, and the default landing page after login.
  static groupedMenuItems: SidebarItem[] = Sidebar.groupMenuItems();

  private static groupMenuItems(): SidebarItem[] {
    const children = (keys: string[]) =>
      keys.map(key => Sidebar.menuItems.find(item => item.key === key)!);
    const groups: MenuGroup[] = [
      {
        key: 'user-access',
        label: 'internal.sidebar.groups.user-access',
        icon: 'hgi-user-group',
        children: children([
          'roles',
          'users',
          'candidate-users',
          'office-users',
        ]),
      },
      {
        key: 'profiles',
        label: 'internal.sidebar.groups.profiles',
        icon: 'hgi-user-multiple',
        children: children([
          'distribution',
          'approve-profile',
          'kawader',
        ]),
      },
      {
        key: 'jobs',
        label: 'internal.sidebar.groups.jobs',
        icon: 'hgi-ai-beautify',
        children: children([
          'job',
          'invitation-summary',
          'exceptions',
          'job-titles',
          'job-points-configuration',
          'job-category-candidate-settings',
          'invitation-expiry-configuration',
        ]),
      },
      {
        key: 'organization',
        label: 'internal.sidebar.groups.organization',
        icon: 'hgi-structure-03',
        children: children([
          'organization-structures',
          'offices',
          'target-entities',
        ]),
      },
      {
        key: 'examinations',
        label: 'internal.sidebar.groups.examinations',
        icon: 'hgi-task-01',
        children: children([
          'exams',
          'exam-results',
          'test-slots',
          'test-sessions',
          'locations',
          'rooms',
        ]),
      },
      {
        key: 'question-banks-group',
        label: 'internal.sidebar.groups.question-banks-group',
        icon: 'hgi-file-validation',
        children: children([
          'question-banks',
          'question-bank-requests',
          'question-bank-assignments',
        ]),
      },
      {
        key: 'interviews-group',
        label: 'internal.sidebar.groups.interviews-group',
        icon: 'hgi-user-question-02',
        children: children([
          'interview-dashboard',
          'interview-axes-criteria',
          'interview-template',
          'interview-committee',
          'interviews',
          'start-interview',
          'approve-interview',
        ]),
      },
      {
        key: 'reference-data',
        label: 'internal.sidebar.groups.reference-data',
        icon: 'hgi-list-setting',
        children: children([
          'major-skill',
          'countries',
          'languages',
          'religions',
          'universities',
        ]),
      },
      {
        key: 'audit-logs',
        label: 'internal.sidebar.groups.audit-logs',
        icon: 'hgi-audit-01',
        children: children([
          'profileLogs',
          'systemAdminLogs',
        ]),
      },
    ];
    const includedGroups = new Set<string>();
    return Sidebar.menuItems.flatMap((item): SidebarItem[] => {
      const group = groups.find(group => group.children.some(child => child.key === item.key));
      if (!group) return [item];
      if (includedGroups.has(group.key)) return [];
      includedGroups.add(group.key);
      return [group];
    });
  }
}
