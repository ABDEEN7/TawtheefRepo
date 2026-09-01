import { GUID } from "../shared/types/guid.type";

export const portalRoutes = {
  portal: '/portal',
  get dashboard() {
    return this.portal + '/dashboard';
  },
  get adminDashboard() {
    return this.portal + '/admin-dashboard';
  },
  get profileDistribution() {
    return this.portal + '/profile-distribution';
  },
  approvalJob(jobId: GUID) {
    return this.portal + '/jobs/approval-job/' + jobId;
  },
  get approvalProfile() {
    return this.portal + '/approval-profile';
  },
  approvalProfileReview(id: string) {
    return this.approvalProfile + `/${id}/wizard`;
  },
  approvalProfileChanges(id: string) {
    return this.approvalProfile + `/${id}/changes`;
  },
  get jobInvitationSummary() {
    return this.portal + '/job-invitation-summary';
  },
  jobInvitationSummaryDetails(id: string) {
    return this.portal + `/job-invitation-summary-details/${id}`;
  },
  get jobCreate() {
    return this.portal + '/jobs/create';
  },
  get JobList() {
    return this.portal + '/jobs';
  },
  JobDetails(jobId: GUID) {
    return this.portal + `/jobs/${jobId}/wizard`;
  },
  jobView(jobId: GUID) {
    return this.portal + '/jobs/view/' + jobId;
  },
  jobCandidates(id: GUID) {
    return this.portal + `/jobs/view/${id}/candidates`;
  },
  jobCandidateProfile(jobId: GUID, candidateId: GUID) {
    return this.portal + `/jobs/view/${jobId}/candidates/${candidateId}`;
  },
  jobEdit(jobId: GUID) {
    return this.portal + '/jobs/edit/' + jobId;
  },
  jobPoints(jobId: GUID) {
    return this.portal + '/jobs/job-points/' + jobId;
  },
  jobPointsReview(jobId: GUID) {
    return this.portal + '/jobs/job-points-review/' + jobId;
  },
  get majorsSkillsManagement() {
    return this.portal + '/majors-skills-management'
  },
  get organizationStructures() {
    return this.portal + '/organization-structures'
  },
  get kawader() {
    return this.portal + '/kawader';
  },
  get ministerOfficeManagement() {
    return this.portal + '/minister-office-management';
  },
  get candidateUsersManagement() {
    return this.portal + '/candidate-users-management';
  },
  candidateUserProfile(id: string) {
    return this.candidateUsersManagement + `/${id}/view`;
  },
  get officeUsersManagement() {
    return this.portal + '/office-users-management';
  },


  get profileLogs() {
    return this.portal + '/profile-logs';
  },
  get systemAdminLogs() {
    return this.portal + '/system-admin-logs';
  },
  get roleManagement() {
    return this.portal + '/roles-management'
  },
  get usersManagement() {
    return this.portal + '/users-management'
  },
  get officesManagement() {
    return this.portal + '/offices-management'
  },
  get countriesManagement() {
    return this.portal + '/countries-management'
  },
  get languagesManagement() {
    return this.portal + '/languages-management'
  },
  get targetEntitiesManagement() {
    return this.portal + '/target-entities-management'
  },
  get religionsManagement() {
    return this.portal + '/religions-management'
  },
  get universitiesManagement() {
    return this.portal + '/universities-management'
  },
  get jobPointsConfiguration() {
    return this.portal + '/job-points-configuration'
  },
  get jobCategoryCandidateSettings() {
    return this.portal + '/job-category-candidate-settings'
  },
  get invitationExpiryConfiguration() {
    return this.portal + '/invitation-expiry-configuration'
  },
  get homeContentManagement() {
    return this.portal + '/home-content-management'
  },
  get jobTitlesManagement() {
    return this.portal + '/job-titles-management'
  },
  get notificationTester() {
    return this.portal + '/notification-tester'
  },
  get locationsManagement() {
    return this.portal + '/locations-management';
  }
}
