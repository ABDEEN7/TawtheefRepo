import { inject, Injectable } from '@angular/core';
import { ApiConfigService } from '../services/api-config.service';
import { CaseUtils } from '../utils/case-utils';
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
        `/auth/external-login?provider=${CaseUtils.toPascalCase(provider)}${returnUrl ? `&returnUrl=${encodeURIComponent(returnUrl)}` : ''
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
    changePersonalLanguage: this.getFullUrl(`/user/profile/language`),
    profile: {
      socialAccounts: this.getFullUrl(`/user/profile/social-accounts`),
    },
  };

  notifications = {
    list: this.getFullUrl('/notifications'),
    unreadCount: this.getFullUrl('/notifications/unread-count'),
    markAsRead: (id: string) => this.getFullUrl(`/notifications/${id}/read`),
    markManyAsRead: this.getFullUrl('/notifications/read'),
    dismiss: (id: string) => this.getFullUrl(`/notifications/${id}/dismiss`),
    dismissMany: this.getFullUrl('/notifications/dismiss'),
  };

  files = {
    upload: this.getFullUrl(`/files/upload`),
    download: (id: string) => this.getFullUrl(`/files/${id}/download`),
  };

  job = {
    job: this.getFullUrl('/job'),
    changeStatus: (jobId: GUID, statusId: GUID) => this.getFullUrl(`/job/${jobId}/status?statusId=${statusId}`),
    jobApproval: this.getFullUrl('/jobApproval'),
    getLatestReview: (jobId: GUID) => this.getFullUrl(`/jobApproval/${jobId}/latest`),
    jobPoints: this.getFullUrl('/jobPoints'),
    getJobPoints: (jobId: GUID) => this.getFullUrl(`/jobPoints/${jobId}`),
    getJobPointsConfig: this.getFullUrl(`/jobPoints/configurations`),
    saveJobPointsConfig: this.getFullUrl(`/jobPoints/configurations/save`),
    approveJobPoints: (jobId: GUID) => this.getFullUrl(`/jobPoints/${jobId}/approve`),
    rejectJobPoints: (jobId: GUID) => this.getFullUrl(`/jobPoints/${jobId}/reject`),
    searchJob: this.getFullUrl('/job/search'),
    exportJobs: this.getFullUrl('/job/export'),
    CountByStatus: (jobStatusId: GUID) => this.getFullUrl(`/job/stats/count?jobStatusId=${jobStatusId}`),
    copyTemplate: (jobId: GUID) => this.getFullUrl(`/job/${jobId}/copy-template`),
    copyFromPrevious: (jobId: GUID) => this.getFullUrl(`/job/${jobId}/copy`),
    checkJobInvitations: (jobId: GUID) => this.getFullUrl(`/job/${jobId}/has-invitations`),
    deleteJobSpecialization: (id: GUID) => this.getFullUrl(`/job/specialization/${id}`),
    updateBasics: (id: GUID) => this.getFullUrl(`/job/${id}/basics`),
    updateOverview: (id: GUID) => this.getFullUrl(`/job/${id}/overview`),
    updateQualifications: (id: GUID) => this.getFullUrl(`/job/${id}/qualifications`),
    updateResponsibilities: (id: GUID) => this.getFullUrl(`/job/${id}/responsibilities`),
    updateConditions: (id: GUID) => this.getFullUrl(`/job/${id}/conditions`),
    updateSkills: (id: GUID) => this.getFullUrl(`/job/${id}/skills`),
    updateAttachments: (id: GUID) => this.getFullUrl(`/job/${id}/attachments`),
    updateBenefits: (id: GUID) => this.getFullUrl(`/job/${id}/benefits`),
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
      jobTitles: this.getFullUrl('/job/lookups/job-titles'),
      genders: this.getFullUrl('/job/lookups/genders'),
      targetEntities: this.getFullUrl('/job/lookups/target-entities'),
      nationalities: this.getFullUrl('/job/lookups/nationalities'),
      jobStatus: this.getFullUrl('/job/lookups/job-status'),
      jobInvitesStatus: this.getFullUrl('/job/lookups/invitation-statuses')
    }
  };

  jobCandidates = {
    overview: this.getFullUrl('/jobCandidates/overview'),
    search: this.getFullUrl('/jobCandidates/search'),
    profile: this.getFullUrl('/jobCandidates/profile'),
    export: this.getFullUrl('/jobCandidates/export'),
    sendInvitations: this.getFullUrl('/jobCandidates/send-invitations'),
    filters: this.getFullUrl('/jobCandidates/filters'),
    categorySettings: this.getFullUrl('/jobCandidates/category-settings'),
    invitationExpiryConfiguration: this.getFullUrl('/jobCandidates/invitation-expiry-configuration'),
    eligibilityCheck: (jobId: string, candidateId: string) => this.getFullUrl(`/jobCandidates/${jobId}/${candidateId}/eligibility-check`),
    searchAllCandidates: this.getFullUrl('/jobCandidates/search-all'),
    lookups: {
      candidateTypes: this.getFullUrl('/jobCandidates/lookups/candidate-types'),
      nationalities: this.getFullUrl('/jobCandidates/lookups/nationalities'),
    },
  };

  kawader = {
    upload: this.getFullUrl('/kawader/upload'),
    list: this.getFullUrl('/kawader'),
  };

  approvals = {
    list: this.getFullUrl('/profile-approvals'),
    targetEntities: this.getFullUrl('/profile-approvals/target-entities'),
    candidateTypes: this.getFullUrl('/profile-approvals/candidate-types'),
    detail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}`),
    changesDetail: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/changes`),
    reviewItem: (reviewItemId: string) => this.getFullUrl(`/profile-approvals/review-items/${reviewItemId}`),
    startReview: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/start-review`),
    decision: (profileId: string, section: string) => this.getFullUrl(`/profile-approvals/${profileId}/sections/${section}/decision`),
    sectionInternalNote: (profileId: string, section: string) =>
      this.getFullUrl(`/profile-approvals/${profileId}/sections/${section}/internal-note`),
    finalize: (profileId: string) => this.getFullUrl(`/profile-approvals/${profileId}/finalize`),
  };


  operationsDashboard = {
      overview: this.getFullUrl('/operations-dashboard/overview'),
      years: this.getFullUrl('/operations-dashboard/years'),
      latestJobs: this.getFullUrl('/operations-dashboard/jobs/latest'),
      latestInvitations: this.getFullUrl('/operations-dashboard/invitations/latest'),
      teamPerformance: this.getFullUrl('/operations-dashboard/team-performance'),
      exportList: this.getFullUrl('/operations-dashboard/export-list'),
  };
  distribution = {
    files: this.getFullUrl('/profile-distributions/profiles'),
    targetEntities: this.getFullUrl('/profile-distributions/target-entities'),
    candidateTypes: this.getFullUrl('/profile-distributions/candidate-types'),
    degrees: this.getFullUrl('/profile-distributions/degrees'),
    employees: this.getFullUrl('/profile-distributions/employees'),
    assignManual: this.getFullUrl('/profile-distributions/assign/manual'),
    assignAuto: this.getFullUrl('/profile-distributions/assign/auto'),
    reassign: this.getFullUrl('/profile-distributions/reassign')
  };

  JobInvitationSummary = {
    invitationsSummary: this.getFullUrl(`/jobInvitationSummary/get-invitations-summary`),
    export: this.getFullUrl(`/jobInvitationSummary/export`),
    details: {
      jobInfo: (jobId: string) => this.getFullUrl(`/jobInvitationSummary/${jobId}/info`),
      stats: this.getFullUrl(`/jobInvitationSummary/get-invitations-stats`),
      rows: this.getFullUrl(`/jobInvitationSummary/get-invitations-details`),
      getAttachments: (invitationId: string) => this.getFullUrl(`/jobInvitationSummary/${invitationId}/attachments`),
      reviewAttachment: (invitationId: string, attachmentId: string) => this.getFullUrl(`/jobInvitationSummary/${invitationId}/attachments/${attachmentId}/review`)
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

  candidateUsers = {
    list: this.getFullUrl('/candidateUsers'),
    export: this.getFullUrl('/candidateUsers/export'),
    blockStatus: (id: string) => this.getFullUrl(`/candidateUsers/${id}/block-status`),
    profile: (id: string) => this.getFullUrl(`/candidateUsers/${id}/profile`),
    profileLogs: (id: string) => this.getFullUrl(`/candidateUsers/${id}/profile-logs`)
  };

  officeUsers = {
    list: this.getFullUrl('/officeUsers'),
    create: this.getFullUrl('/officeUsers'),
    update: (id: string) => this.getFullUrl(`/officeUsers/${id}`),
    blockStatus: (id: string) => this.getFullUrl(`/officeUsers/${id}/block-status`)
  };

  offices = {
    listOffices: this.getFullUrl('/offices/list-offices'),
    officeDetails: (id: string) => this.getFullUrl(`/offices/office-details/${id}`),
    countries: this.getFullUrl('/offices/lookups/countries'),
    createOffice: this.getFullUrl('/offices/create-office'),
    updateOffice: (id: string) => this.getFullUrl(`/offices/update-office/${id}`),
    deleteOffice: (id: string) => this.getFullUrl(`/offices/delete-office/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/offices/${id}/status`),
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
  religions = {
    listReligions: this.getFullUrl('/religions/list-religions'),
    religionDetails: (id: string) => this.getFullUrl(`/religions/religion-details/${id}`),
    createReligion: this.getFullUrl('/religions/create-religion'),
    updateReligion: (id: string) => this.getFullUrl(`/religions/update-religion/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/religions/${id}/status`)
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

  jobTitles = {
    list: this.getFullUrl('/jobtitles'),
    create: this.getFullUrl('/jobtitles'),
    update: (id: string) => this.getFullUrl(`/jobtitles/${id}`),
    delete: (id: string) => this.getFullUrl(`/jobtitles/${id}`)
  };

  homeContent = {
    successStories: this.getFullUrl('/home-content/success-stories'),
    successStory: (id: string) => this.getFullUrl(`/home-content/success-stories/${id}`),
    successStoryStatus: (id: string) => this.getFullUrl(`/home-content/success-stories/${id}/status`),
    faqs: this.getFullUrl('/home-content/faqs'),
    faq: (id: string) => this.getFullUrl(`/home-content/faqs/${id}`),
    faqStatus: (id: string) => this.getFullUrl(`/home-content/faqs/${id}/status`)
  };
  profileLogs = {
    list: this.getFullUrl('/profile-logs'),
    lookups: this.getFullUrl('/profile-logs/lookups'),
  };
  systemAdminLogs = {
    list: this.getFullUrl('/system-admin-logs'),
    lookups: this.getFullUrl('/system-admin-logs/lookups'),
    navigation: this.getFullUrl('/system-admin-logs/navigation'),
  };
  targetEntities = {
    listTargetEntities: this.getFullUrl('/targetentities/list-target-entities'),
    targetEntityDetails: (id: string) => this.getFullUrl(`/targetentities/target-entity-details/${id}`),
    createTargetEntity: this.getFullUrl('/targetentities/create-target-entity'),
    updateTargetEntity: (id: string) => this.getFullUrl(`/targetentities/update-target-entity/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/targetentities/${id}/status`)
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
    lookups: {
      skills: this.getFullUrl('/SkillsManagement/lookups/skills'),
      skillTypes: this.getFullUrl('/SkillsManagement/lookups/skill-types'),
      majors: this.getFullUrl('/MajorSkillsManagement/lookups/majors'),
      subMajors: this.getFullUrl('/MajorSkillsManagement/lookups/sub-majors'),
    }
  };

  organizationStructures = {
    sectors: {
      list: this.getFullUrl('/OrganizationStructures/sectors'),
      create: this.getFullUrl('/OrganizationStructures/sectors'),
      update: (id: string) => this.getFullUrl(`/OrganizationStructures/sectors/${id}`),
      changeActivation: this.getFullUrl('/OrganizationStructures/sectors/change-activation')
    },
    managements: {
      list: this.getFullUrl('/OrganizationStructures/managements'),
      create: this.getFullUrl('/OrganizationStructures/managements'),
      update: (id: string) => this.getFullUrl(`/OrganizationStructures/managements/${id}`),
      changeActivation: this.getFullUrl('/OrganizationStructures/managements/change-activation')
    },
    departments: {
      list: this.getFullUrl('/OrganizationStructures/departments'),
      create: this.getFullUrl('/OrganizationStructures/departments'),
      update: (id: string) => this.getFullUrl(`/OrganizationStructures/departments/${id}`),
      changeActivation: this.getFullUrl('/OrganizationStructures/departments/change-activation')
    },
    lookups: {
      sectors: this.getFullUrl('/OrganizationStructures/lookups/sectors'),
      managements: (sectorId: string) =>
        this.getFullUrl(`/OrganizationStructures/lookups/managements?sectorId=${sectorId}`),
      departments: (managementId: string) =>
        this.getFullUrl(`/OrganizationStructures/lookups/departments?managementId=${managementId}`)
    }
  };

  countries = {
    listCountries: this.getFullUrl('/countryManagement/list-countries'),
    createCountry: this.getFullUrl('/countryManagement/create-country'),
    updateCountry: (id: string) => this.getFullUrl(`/countryManagement/update-country/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/countryManagement/${id}/status`)
  };
  cities = {
    listCities: this.getFullUrl('/cityManagement/list-cities'),
    createCity: this.getFullUrl('/cityManagement/create-city'),
    updateCity: (id: string) => this.getFullUrl(`/cityManagement/update-city/${id}`),
    updateStatus: (id: string) => this.getFullUrl(`/cityManagement/${id}/status`)
  };

  ministerOffice = {
    candidates: this.getFullUrl('/MinisterOffice/candidates'),
    phone: (id: string) => this.getFullUrl(`/MinisterOffice/candidates/${id}/phone`),
    followUpStatus: (id: string) => this.getFullUrl(`/MinisterOffice/candidates/${id}/follow-up-status`),
    invitations: (id: string) => this.getFullUrl(`/MinisterOffice/candidates/${id}/invitations`),
    auditLog: (id: string) => this.getFullUrl(`/MinisterOffice/candidates/${id}/audit-log`),
    genders: this.getFullUrl('/MinisterOffice/lookups/genders'),
    targetEntities: this.getFullUrl('/MinisterOffice/lookups/target-entities'),
    candidateTypes: this.getFullUrl('/MinisterOffice/lookups/candidate-types'),
  };

  notificationTester = {
    templates: this.getFullUrl('/notification-tester/templates'),
    send: this.getFullUrl('/notification-tester/send'),
    preview: this.getFullUrl('/notification-tester/preview'),
  };
}
