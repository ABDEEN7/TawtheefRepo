export const Permissions = {
  Dashboard: {
    View: 'dashboard.view',
  },
  Roles: {
    View: 'roles.view',
    Manage: 'roles.manage',
  },
  Users: {
    View: 'users.view',
    Manage: 'users.manage',
  },
  CandidateUsers: {
    View: 'candidate.users.view',
    Manage: 'candidate.users.manage',
  },
  OfficeUsers: {
    View: 'office.users.view',
    Manage: 'office.users.manage',
  },
  Languages: {
    View: 'languages.view',
    Manage: 'languages.manage',
  },
  Religions: {
    View: 'religions.view',
    Manage: 'religions.manage',
  },
  Offices: {
    View: 'offices.view',
    Manage: 'offices.manage',
  },
  Countries: {
    View: 'countries.view',
    Manage: 'countries.manage',
  },
  Universities: {
    View: 'universities.view',
    Manage: 'universities.manage',
  },
  ProfileLogs: {
    View: 'profile.logs.view',
  },
  TargetEntities: {
    View: 'targetentities.view',
    Manage: 'targetentities.manage',
  },
  HomeContent: {
    View: 'home.content.view',
    Manage: 'home.content.manage',
  },
  Profile: {
    View: 'profile.view',
    Manage: 'profile.manage',
  },
  ProfileDistribution: {
    View: 'profile.distribution.view',
    Manage: 'profile.distribution.manage',
  },
  ProfileApproval: {
    View: 'profile.approval.view',
    Review: 'profile.approval.review',
    Changes: 'profile.approval.changes',
  },
  Jobs: {
    View: 'jobs.view',
    Manage: 'jobs.manage',
    Approve: 'jobs.approve',
  },
  JobPoints: {
    View: 'jobs.points.view',
    Manage: 'jobs.points.manage',
    Approve: 'jobs.points.approve',
  },
  JobInvitations: {
    View: 'jobs.invitations.view',
  },
  MajorSkills: {
    Manage: 'major-skill.management',
  },
  OrganizationStructures: {
    Manage: 'organization-structures.manage',
  },
  Nominations: {
    View: 'nominations.view',
    Manage: 'nominations.manage',
  },
  Kawader: {
    Manage: 'kawader.manage',
  },
  MinisterOffice: {
    View: 'minister-office.view',
    Manage: 'minister-office.manage',
  }
} as const;
