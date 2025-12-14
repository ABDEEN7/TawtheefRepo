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
    return this.employee + '/approval-job';
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
  get nominations(){
    return this.employee + '/nominations';
  }
}
