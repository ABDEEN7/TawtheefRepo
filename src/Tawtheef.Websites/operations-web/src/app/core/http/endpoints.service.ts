import { inject, Injectable } from '@angular/core';
import { ApiConfigService } from '../services/api-config.service';
import { CaseUtils } from '../utils/case-utils';

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
    searchJob : this.getFullUrl('/job/search'),
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

  approvals = {
    list: this.getFullUrl('/profile-approvals'),
    detail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}`),
    changesDetail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/changes`),
    reviewItem: (reviewItemId: string) => this.getFullUrl(`/profile-approvals/review-items/${reviewItemId}`),
    finalize: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/finalize`)
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
    lookups: {
      jobStatuses: this.getFullUrl(`/jobInvitationSummary/lookups/job-statuses`),
      jobCategories: this.getFullUrl(`/jobInvitationSummary/lookups/job-categories`),
      departments: this.getFullUrl(`/jobInvitationSummary/lookups/departments`)
    }
  };

  roles = {
    listRoles: this.getFullUrl('/roles/list-roles'),
    roleDetails: (id: string) => this.getFullUrl(`/roles/role-details/${id}`),
    createRole: this.getFullUrl('/roles/create-role'),
    updateRole: (id: string) => this.getFullUrl(`/roles/update-role/${id}`),
    deleteRole: (id: string) => this.getFullUrl(`/roles/delete-role/${id}`),

    listPermissions: this.getFullUrl('/roles/list-permissions')
  };
}
