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
  approvalProfileDetail(profileId: string){
    return this.approvalProfile + '/' + profileId;
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
  }
}
