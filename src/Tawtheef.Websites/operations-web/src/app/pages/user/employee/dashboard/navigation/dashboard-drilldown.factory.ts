import { Permissions } from '../../../../../core/constants/permissions';
import { routes } from '../../../../../routes/routes';
import { IndicatorNavigation } from '../models/dashboard-ui.model';

const candidateUsers = (
  year: number,
  profileStatus?: 'Approved' | 'UnderReview'
): IndicatorNavigation => ({
  route: routes.portal.candidateUsersManagement,
  queryParams: {
    ...(profileStatus ? { profileStatus } : {}),
    scope: 'AccessibleProfiles',
    year
  },
  requiredPermission: Permissions.CandidateUsers.View
});

export const dashboardDrilldowns = {
  totalProfiles: (year: number): IndicatorNavigation => candidateUsers(year),
  approvedProfiles: (year: number): IndicatorNavigation => candidateUsers(year, 'Approved'),
  underReviewProfiles: (year: number): IndicatorNavigation => candidateUsers(year, 'UnderReview'),
  waitingDistribution: (year: number): IndicatorNavigation => ({
    route: routes.portal.profileDistribution,
    queryParams: { assignmentState: 'Unassigned', year },
    requiredPermission: Permissions.ProfileDistribution.View
  }),
  publishedJobs: (year: number): IndicatorNavigation => ({
    route: routes.portal.JobList,
    queryParams: { status: 'Published', year },
    requiredPermission: Permissions.Jobs.View
  }),
  totalInvitations: (year: number): IndicatorNavigation => ({
    route: routes.portal.jobInvitationSummary,
    queryParams: { year },
    requiredPermission: Permissions.JobInvitations.View
  })
};
