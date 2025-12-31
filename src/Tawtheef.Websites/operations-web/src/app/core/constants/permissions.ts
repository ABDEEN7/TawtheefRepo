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
  Offices: {
    View: 'offices.view',
    Manage: 'offices.manage',
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
    PointsManage: 'jobs.points.manage',
  },
  JobInvitations: {
    View: 'jobs.invitations.view',
  },
  Nominations: {
    View: 'nominations.view',
    Manage: 'nominations.manage',
  },
  Kawader: {
    Manage: 'kawader.manage',
  },
} as const;
