import { inject, Injectable } from '@angular/core';
import { ApiConfigService } from '../services/api-config.service';
import { CaseUtils } from '../utils/case-utils';
import {ReviewStatus} from '../../pages/user/employee/profile-managment/approval-list/models/profile-approval.models';
import { GUID } from '../../shared/types/guid.type';

@Injectable({ providedIn: 'root' })
export class EndpointsService {
  apiConfig = inject(ApiConfigService);

  private getFullUrl(endpoint: string): string {
    return `${this.apiConfig.baseUrl}${endpoint}`;
  }

  logger = this.getFullUrl(`/logger`);
  auth = {
    me: this.getFullUrl(this.getFullUrl(`/auth/me`)),
    login: this.getFullUrl(`/auth/login`),
    register: this.getFullUrl(`/auth/register`),
    refresh: this.getFullUrl(`/auth/refresh-token`),
    externalLogin: (provider: string, returnUrl: string | null = null) =>
      this.getFullUrl(
        `/auth/external-login?provider=${CaseUtils.toPascalCase(provider)}${
          returnUrl ? `&returnUrl=${encodeURIComponent(returnUrl)}` : ''
        }`
      ),
    externalLoginUsingToken: this.getFullUrl(`/auth/external-login/token`),
    logout: this.getFullUrl(`/auth/logout`),
    exchangeCode: this.getFullUrl(`/auth/exchange-code`),
    verifyOtp: this.getFullUrl(`/auth/verify-otp`),
    forgotPassword: this.getFullUrl(`/auth/forgot-password`),
    resetPassword: this.getFullUrl(`/auth/reset-password`),
    resendOtp: this.getFullUrl(`/auth/resend-otp`),
  };

  user = {
    profile: {
      socialAccounts: this.getFullUrl(`/user/profile/social-accounts`),
    },
  };

  files = {
    upload: this.getFullUrl(`/files/upload`),
    download: (id: string) => this.getFullUrl(`/files/${id}/download`),
  };

  job = {
    job : this.getFullUrl('/job'),
    jobApproval : this.getFullUrl('/jobApproval'),
    getLatestReview : (jobId : GUID) => this.getFullUrl(`/jobApproval/${jobId}/latest`),
    jobPoints : this.getFullUrl('/jobPoints'),
    getJobPoints :(jobId : GUID) => this.getFullUrl(`/jobPoints/${jobId}`),
    getJobPointsConfig : this.getFullUrl(`/jobPoints/config`),
    approveJobPoints : (jobId : GUID) => this.getFullUrl(`/jobPoints/${jobId}/approve`),
    searchJob : this.getFullUrl('/job/search'),
    CountByStatus : (jobStatusId : GUID) => this.getFullUrl(`/job/stats/count?jobStatusId=${jobStatusId}`),
    lookups: {
      sectors: this.getFullUrl('/job/lookups/sectors'),
      skills: this.getFullUrl('/job/lookups/skills'),
      managements: this.getFullUrl('/job/lookups/managements'),
      departments: this.getFullUrl('/job/lookups/departments'),
      majors: this.getFullUrl('/job/lookups/majors'),
      subMajors: this.getFullUrl('/job/lookups/sub-majors'),
      degrees: this.getFullUrl('/job/lookups/degrees'),
      workTypes: this.getFullUrl('/job/lookups/work-types'),
      jobCategories: this.getFullUrl('/job/lookups/job-categories'),
      genders: this.getFullUrl('/job/lookups/genders'),
      targetEntities: this.getFullUrl('/job/lookups/target-entities'),
      nationalities: this.getFullUrl('/job/lookups/nationalities'),
      jobStatus : this.getFullUrl('/job/lookups/job-status'),
      jobInvitesStatus : this.getFullUrl('/job/lookups/invitation-statuses')
    }
  };

  jobCandidates = {
    overview: this.getFullUrl('/jobCandidates/overview'),
    search: this.getFullUrl('/jobCandidates/search'),
    export: this.getFullUrl('/jobCandidates/export'),
    sendInvitations: this.getFullUrl('/jobCandidates/send-invitations'),
    lookups: {
      candidateTypes: this.getFullUrl('/jobCandidates/lookups/candidate-types'),
    },
  };

  kawader = {
    upload: this.getFullUrl('/kawader/upload'),
  };

  approvals = {
    list: this.getFullUrl('/profile-approvals'),
    detail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}`),
    changesDetail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/changes`),
    reviewItem: (reviewItemId: string) => this.getFullUrl(`/profile-approvals/review-items/${reviewItemId}`),
    startReview: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/start-review`),
    decision: (profileId: string, section: string) => this.getFullUrl(`/profile-approvals/${profileId}/sections/${section}/decision`),
    finalize: (profileId:string) => this.getFullUrl(`/profile-approvals/${profileId}/finalize`),
  };

  distribution = {
    files: this.getFullUrl('/profile-distributions/profiles'),
    employees: this.getFullUrl('/profile-distributions/employees'),
    assignManual: this.getFullUrl('/profile-distributions/assign/manual'),
    assignAuto: this.getFullUrl('/profile-distributions/assign/auto'),
    reassign: this.getFullUrl('/profile-distributions/reassign')
  };

  JobInvitationSummary = {
    invitationsSummary: this.getFullUrl(`/jobInvitationSummary/get-invitations-summary`),
    details: {
      jobInfo: (jobId: string) => this.getFullUrl(`/jobInvitationSummary/${jobId}/info`),
      stats: this.getFullUrl(`/jobInvitationSummary/get-invitations-stats`),
      rows: this.getFullUrl(`/jobInvitationSummary/get-invitations-details`)
    },
    lookups: {
      jobStatuses: this.getFullUrl(`/jobInvitationSummary/lookups/job-statuses`),
      jobCategories: this.getFullUrl(`/jobInvitationSummary/lookups/job-categories`),
      departments: this.getFullUrl(`/jobInvitationSummary/lookups/departments`)
    }
  };

  roles = {
    listRoles: this.getFullUrl('/roles/list-roles'),
    lookups: this.getFullUrl('/roles/lookups'),
    roleDetails: (id: string) => this.getFullUrl(`/roles/role-details/${id}`),
    createRole: this.getFullUrl('/roles/create-role'),
    updateRole: (id: string) => this.getFullUrl(`/roles/update-role/${id}`),
    deleteRole: (id: string) => this.getFullUrl(`/roles/delete-role/${id}`),

    listPermissions: this.getFullUrl('/roles/list-permissions')
  };

  users = {
    listUsers: this.getFullUrl('/userManagement/list-users'),
    userRoles: (id: string) => this.getFullUrl(`/userManagement/${id}/roles`),
    userRoleIds: (id: string) => this.getFullUrl(`/userManagement/${id}/role-ids`),
    blockStatus: (id: string) => this.getFullUrl(`/userManagement/${id}/block-status`)
  };

  offices = {
    listOffices: this.getFullUrl('/offices/list-offices'),
    officeDetails: (id: string) => this.getFullUrl(`/offices/office-details/${id}`),
    countries: this.getFullUrl('/offices/lookups/countries'),
    createOffice: this.getFullUrl('/offices/create-office'),
    updateOffice: (id: string) => this.getFullUrl(`/offices/update-office/${id}`),
    deleteOffice: (id: string) => this.getFullUrl(`/offices/delete-office/${id}`),
    updateOfficeUserBlockStatus: (officeId: string, userId: string) =>
      this.getFullUrl(`/offices/${officeId}/users/${userId}/block-status`),
    setOfficeAdmin: (officeId: string, userId: string) => this.getFullUrl(`/offices/${officeId}/set-admin/${userId}`)
  };

  languages = {
    listLanguages: this.getFullUrl('/languages/list-languages'),
    languageDetails: (id: string) => this.getFullUrl(`/languages/language-details/${id}`),
    createLanguage: this.getFullUrl('/languages/create-language'),
    updateLanguage: (id: string) => this.getFullUrl(`/languages/update-language/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/languages/${id}/status`)
  };
  universities = {
    listUniversities: this.getFullUrl('/universities/list-universities'),
    universityDetails: (id: string) => this.getFullUrl(`/universities/university-details/${id}`),
    createUniversity: this.getFullUrl('/universities/create-university'),
    updateUniversity: (id: string) => this.getFullUrl(`/universities/update-university/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/universities/${id}/status`),
    lookups: {
      countries: this.getFullUrl('/universities/lookups/countries'),
      cities: (countryId: string) => this.getFullUrl(`/universities/lookups/cities?countryId=${countryId}`)
    }
  };
  majorSkillsManagement = {
    list: this.getFullUrl('/MajorSkillsManagement'),
    details: (id: string) => this.getFullUrl(`/MajorSkillsManagement/${id}`),
    create: this.getFullUrl('/MajorSkillsManagement'),
    update: this.getFullUrl('/MajorSkillsManagement'),
    changeActivation: this.getFullUrl('/MajorSkillsManagement/change-activation'),
    majors: {
      main_list: this.getFullUrl('/MajorsManagement/GetMainMajors'),
      sub_list: this.getFullUrl('/MajorsManagement/GetSubMajors'),
      create: this.getFullUrl('/MajorsManagement'),
      update: this.getFullUrl('/MajorsManagement'),
      changeActivation: this.getFullUrl('/MajorsManagement/change-activation')
    },
    skills: {
      list: this.getFullUrl('/SkillsManagement'),
      create: this.getFullUrl('/SkillsManagement'),
      update: this.getFullUrl('/SkillsManagement'),
      changeActivation: this.getFullUrl('/SkillsManagement/change-activation')
    },
    lookups:{
      skills: this.getFullUrl('/SkillsManagement/lookups/skills'),
      skillTypes: this.getFullUrl('/SkillsManagement/lookups/skill-types'),
      majors: this.getFullUrl('/MajorSkillsManagement/lookups/majors'),
      subMajors: this.getFullUrl('/MajorSkillsManagement/lookups/sub-majors'),
    }
  };

  countries = {
    listCountries: this.getFullUrl('/countryManagement/list-countries'),
    updateStatus: (id: string) => this.getFullUrl(`/countryManagement/${id}/status`)
  };
}
