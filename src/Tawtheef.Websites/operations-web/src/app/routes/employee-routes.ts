import { GUID } from "../shared/types/guid.type";

export const employeeRoutes = {
  employee: '/employee',
  get dashboard() {
    return this.employee + '/dashboard';
  },
  get settings() {
    return this.employee + '/settings';
  },
  get profileDistribution(){
    return this.employee + '/profile-distribution';
  },
  approvalJob(jobId: GUID){
    return this.employee + '/jobs/approval-job/' + jobId;
  },
  get approvalProfile(){
    return this.employee + '/approval-profile';
  },
  approvalProfileReview(id: string) {
    return this.approvalProfile + `/${id}/wizard`;
  },
  approvalProfileChanges (id: string){
    return this.approvalProfile + `/${id}/changes`;
    },
  get jobInvitationSummary(){
    return this.employee + '/job-invitation-summary';
  },
  jobInvitationSummaryDetails(id: string){
    return this.employee + `/job-invitation-summary-details/${id}`;
  },
  get jobCreate(){
    return this.employee + '/jobs/create';
  },
  get JobList(){
    return this.employee + '/jobs';
  },
  JobDetails(jobId: GUID){
    return this.employee + `/jobs/${jobId}/wizard`;
  },
  jobView(jobId: GUID){
    return this.employee + '/jobs/view/' + jobId;
  },
  jobCandidates(id: GUID){
    return this.employee + `/jobs/view/${id}/candidates`;
  },
  jobEdit(jobId: GUID){
    return this.employee + '/jobs/edit/' + jobId;
  },
  get nominations(){
    return this.employee + '/nominations';
  },
  jobPoints(jobId: GUID){
    return this.employee + '/jobs/job-points/' + jobId;
  },
  get majorsSkillsManagement(){
    return this.employee + '/majors-skills-management'
  },
  get organizationStructures(){
    return this.employee + '/organization-structures'
  },
  get kawader(){
    return this.employee + '/kawader';
  },
  get candidateUsersManagement(){
    return this.employee + '/candidate-users-management';
  },
  get officeUsersManagement(){
    return this.employee + '/office-users-management';
  },
}
