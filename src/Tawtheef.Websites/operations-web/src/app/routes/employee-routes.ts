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
  get approvalJob(){
    return this.employee + '/jobs/approval-job';
  },
  get approvalProfile(){
    return this.employee + '/approval-profile';
  },
  approvalProfileDetail(id: string){
    return this.approvalProfile + `/${id}`;
  },
  approvalProfileReview(id: string) {
    return this.approvalProfile + `/${id}/review`;
  },
  approvalProfileChanges (id: string){
    return this.approvalProfile + `/${id}/changes`;
    },
  get jobInvitationSummary(){
    return this.employee + '/job-invitation-summary';
  },
  get jobCreate(){
    return this.employee + '/jobs/create';
  },
  get JobList(){
    return this.employee + '/jobs';
  },
  get jobView(){
    return this.employee + '/jobs/view';
  },
  get jobEdit(){
    return this.employee + '/jobs/edit';
  },
  get nominations(){
    return this.employee + '/nominations';
  },
   get jobPoints(){
    return this.employee + '/jobs/job-points';
  }
}
