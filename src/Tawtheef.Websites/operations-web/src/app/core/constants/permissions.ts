export const Permissions = {
  Dashboard: {
    View: 'dashboard.view',
    Export: 'dashboard.export',
    Manage: 'dashboard.manage',
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
    Edit: 'jobs.edit',
    Approve: 'jobs.approve',
    SendInvitation: 'jobs.send-invitation',
    Cancel: 'jobs.cancel',
    Create: 'jobs.create',
    Publish: 'jobs.publish',
    Delete: 'jobs.delete',
    Clone: 'jobs.clone',
  },
  JobPoints: {
    View: 'jobs.points.view',
    Edit: 'jobs.points.edit',
    Approve: 'jobs.points.approve',
  },
  JobInvitations: {
    View: 'jobs.invitations.view',
  },
  MajorSkills: {
    View: 'major-skill.view',
    Manage: 'major-skill.management',
  },
  OrganizationStructures: {
    Manage: 'organization-structures.manage',
  },
  Kawader: {
    Manage: 'kawader.manage',
  },
  MinisterOffice: {
    View: 'minister-office.view',
    Manage: 'minister-office.manage',
  },
  Cities: {
    View: 'cities.view',
    Manage: 'cities.manage',
  },
  JobTitles: {
    View: 'job-titles.view',
    Manage: 'job-titles.manage',
  },
  JobCategoryCandidateSettings: {
    View: 'job-category-candidate-settings.view',
    Manage: 'job-category-candidate-settings.manage',
  },
  JobPointsConfiguration: {
    View: 'job-points-configuration.view',
    Manage: 'job-points-configuration.manage',
  },
  InvitationExpiryConfiguration: {
    View: 'invitation-expiry-configuration.view',
    Manage: 'invitation-expiry-configuration.manage',
  },
  Rooms: {
    View: 'rooms.view',
    Manage: 'rooms.manage',
  },
} as const;
