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
  notifications = {
    list: this.getFullUrl('/notifications'),
    unreadCount: this.getFullUrl('/notifications/unread-count'),
    updateStateSingle: (id: string) => this.getFullUrl(`/notifications/${id}/state`),
    updateStateMany: this.getFullUrl('/notifications/state')
  };
  auth = {
    me: this.getFullUrl(this.getFullUrl(`/auth/me`)),
    login: this.getFullUrl(`/auth/login`),
    register: this.getFullUrl(`/auth/register`),
    refresh: this.getFullUrl(`/auth/refresh-token`),
    externalLogin: (provider: string, returnUrl: string | null = null) =>
      this.getFullUrl(`/auth/external-login?provider=${CaseUtils.toPascalCase(provider)}&returnUrl=${encodeURIComponent(returnUrl ?? '/')}`),
    logout: this.getFullUrl(`/auth/logout`),
    exchangeCode: this.getFullUrl(`/auth/exchange-code`),
    verifyOtp: this.getFullUrl(`/auth/verify-otp`),
    forgotPassword: this.getFullUrl(`/auth/forgot-password`),
    resetPassword: this.getFullUrl(`/auth/reset-password`),
    resendOtp: this.getFullUrl(`/auth/resend-otp`),
    qatarResident: {
      requestOtp: this.getFullUrl('/auth/qatar-resident/request-otp'),
      verifyOtp: this.getFullUrl('/auth/qatar-resident/verify-otp')
    }
  };
  user = {
    changePersonalLanguage: this.getFullUrl(`/user/profile/language`),
    bootstrap: this.getFullUrl(`/me/bootstrap`),
    agreeToTerms: this.getFullUrl('/user/agree-terms'),
    profile: {
      reviewSummary: this.getFullUrl('/user/profile/review-summary'),
      basics: this.getFullUrl(`/profiles/basics`),
      savePrereq: this.getFullUrl(`/profiles/prereq`),
      savePersonal: this.getFullUrl(`/profiles/personal`),
      saveAvailability: this.getFullUrl(`/profiles/availability`),
      saveContact: this.getFullUrl(`/profiles/contact`),
      saveEducation: this.getFullUrl(`/profiles/education`),
      deleteEducation: (id: string) => this.getFullUrl(`/profiles/education/${id}/delete`),
      saveExperience: this.getFullUrl(`/profiles/experience`),
      deleteExperience: (id: string) => this.getFullUrl(`/profiles/experience/${id}/delete`),
      deleteTrainingCourse: (id: string) => this.getFullUrl(`/profiles/training/${id}/delete`),
      saveAchievements: this.getFullUrl(`/profiles/achievements`),
      deleteAchievement: (id: string) => this.getFullUrl(`/profiles/achievement/${id}/delete`),
      saveSkills: this.getFullUrl(`/profiles/skills`),
      deleteSkill: (id: string) => this.getFullUrl(`/profiles/skill/${id}/delete`),
      deleteLanguage: (id: string) => this.getFullUrl(`/profiles/language/${id}/delete`),
      saveLanguages: this.getFullUrl(`/profiles/languages`),
      saveReferences: this.getFullUrl(`/profiles/references`),
      submit: this.getFullUrl(`/profiles/submit`),
      resubmit: this.getFullUrl(`/profiles/revisions/submit`),
      checkProfile: this.getFullUrl(`/profiles/check-profile`),
      changeRequests: this.getFullUrl(`/profiles/change-requests`),
      requestChanges: {
        prereq: this.getFullUrl(`/profiles/change-requests/prereq`),
        personal: this.getFullUrl(`/profiles/change-requests/personal`),
        contact: this.getFullUrl(`/profiles/change-requests/contact`),
        education: this.getFullUrl(`/profiles/change-requests/education`),
        experience: this.getFullUrl(`/profiles/change-requests/experience`),
        achievements: this.getFullUrl(`/profiles/change-requests/achievements`),
        skills: this.getFullUrl(`/profiles/change-requests/skills`),
        languages: this.getFullUrl(`/profiles/change-requests/languages`),
        references: this.getFullUrl(`/profiles/change-requests/references`),
      },
      revisions: {
        prereq: this.getFullUrl(`/profiles/revisions/prereq`),
        prereqAttachment: this.getFullUrl(`/profiles/revisions/prereq/Attachment`),
        personal: this.getFullUrl(`/profiles/revisions/personal`),
        personalAttachment: this.getFullUrl(`/profiles/revisions/personal/Attachment`),
        contact: this.getFullUrl(`/profiles/revisions/contact`),
        contactAttachment: this.getFullUrl(`/profiles/revisions/contact/Attachment`),
        availability: this.getFullUrl(`/profiles/revisions/availability`),
        education: this.getFullUrl(`/profiles/revisions/education`),
        deleteEducation: (id: string) => this.getFullUrl(`/profiles/revisions/education/${id}/delete`),
        experience: this.getFullUrl(`/profiles/revisions/experience`),
        deleteExperience: (id: string) => this.getFullUrl(`/profiles/revisions/experience/${id}/delete`),
        deleteTrainingCourse: (id: string) => this.getFullUrl(`/profiles/revisions/training/${id}/delete`),
        achievements: this.getFullUrl(`/profiles/revisions/achievements`),
        deleteAchievement: (id: string) => this.getFullUrl(`/profiles/revisions/achievement/${id}/delete`),
        skills: this.getFullUrl(`/profiles/revisions/skills`),
        deleteSkill: (id: string) => this.getFullUrl(`/profiles/revisions/skill/${id}/delete`),
        languages: this.getFullUrl(`/profiles/revisions/languages`),
        deleteLanguage: (id: string) => this.getFullUrl(`/profiles/revisions/language/${id}/delete`),
        references: this.getFullUrl(`/profiles/revisions/references`),
        deleteReference: (id: string) => this.getFullUrl(`/profiles/revisions/references/${id}/delete`),
      },
      sections: {
        prereq: this.getFullUrl(`/profiles/prereq`),
        personal: this.getFullUrl(`/profiles/personal`),
        contact: this.getFullUrl(`/profiles/contact`),
        education: this.getFullUrl(`/profiles/education`),
        experience: this.getFullUrl(`/profiles/experience`),
        training: this.getFullUrl(`/profiles/training`),
        achievements: this.getFullUrl(`/profiles/achievements`),
        skills: this.getFullUrl(`/profiles/skills`),
        languages: this.getFullUrl(`/profiles/languages`),
        attachments: this.getFullUrl(`/profiles/references`),
      },
    },
    verify: {
      phone: {
        request: this.getFullUrl(`/user/verify/phone/request`),
        confirm: this.getFullUrl(`/user/verify/phone/confirm`),
        update: this.getFullUrl(`/user/update/phone`),
      },
      email: {
        request: this.getFullUrl(`/user/verify/email/request`),
        confirm: this.getFullUrl(`/user/verify/email/confirm`),
      }
    }
  };
  files = {
    download: (id: string) => this.getFullUrl(`/files/${id}/download`)
  };

  profile = {
    lookups: {
      candidateTypes: this.getFullUrl(`/profiles/lookups/candidate-types`),
      targetEntities: this.getFullUrl(`/profiles/lookups/target-entities`),
      genders: this.getFullUrl(`/profiles/lookups/genders`),
      religions: this.getFullUrl(`/profiles/lookups/religions`),
      maritalStatuses: this.getFullUrl(`/profiles/lookups/marital-statuses`),
      countries: this.getFullUrl(`/profiles/lookups/countries`),
      nationalities: this.getFullUrl(`/profiles/lookups/nationalities`),
      graduationCountries: this.getFullUrl(`/profiles/lookups/graduation-countries`),
      degrees: this.getFullUrl(`/profiles/lookups/degrees`),
      universities: this.getFullUrl(`/profiles/lookups/universities`),
      majors: this.getFullUrl(`/profiles/lookups/majors`),
      subMajors: this.getFullUrl(`/profiles/lookups/sub-majors`),
      studyTypes: this.getFullUrl(`/profiles/lookups/study-types`),
      achievementTypes: this.getFullUrl(`/profiles/lookups/achievement-types`),
      ratingGrades: this.getFullUrl(`/profiles/lookups/rating-grades`),
      skillLevels: this.getFullUrl(`/profiles/lookups/skill-levels`),
      languages: this.getFullUrl(`/profiles/lookups/languages`),
      languageLevels: this.getFullUrl(`/profiles/lookups/language-levels`),
      sponsorTypes: this.getFullUrl(`/profiles/lookups/sponsor-types`),
      offices: this.getFullUrl(`/profiles/lookups/offices`),
      skill: this.getFullUrl(`/profiles/lookups/skill-search`),
    },
    overview: this.getFullUrl(`/profiles/overview`)
  }

  dashboard = {
    candidateInvitations: this.getFullUrl(`/dashboard/get-candidate-invitations`),
    candidateInvitationStatistics: this.getFullUrl(`/dashboard/candidate-invitation-statistics`),
    candidateInvitationDetails: (invitationId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}`),
    candidateInvitationJobDetails: (invitationId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/job-details`),
    applyCandidateInvitation: (invitationId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/apply`),
    changeStatusCandidateInvitationRead: (invitationId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/read`),
    changeStatusCandidateInvitationReject: (invitationId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/reject`),
    uploadInvitationAttachment: (invitationId: string, jobRequiredAttachmentId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/attachments/${jobRequiredAttachmentId}`),
    deleteInvitationAttachment: (invitationId: string, attachmentId: string) =>
      this.getFullUrl(`/dashboard/candidate-invitations/${invitationId}/attachments/${attachmentId}`),
    lookups: {
      invitationStatuses: this.getFullUrl(`/dashboard/lookups/invitation-statuses`),
      jobCategories: this.getFullUrl(`/dashboard/lookups/job-categories`),
      departments: this.getFullUrl(`/dashboard/lookups/department`)
    }
  };

  homeContent = {
    content: this.getFullUrl('/home-content')
  };
}
