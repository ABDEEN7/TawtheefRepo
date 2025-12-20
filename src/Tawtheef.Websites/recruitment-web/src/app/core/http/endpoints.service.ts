import {inject, Injectable} from '@angular/core';
import {ApiConfigService} from '../services/api-config.service';
import {CaseUtils} from '../utils/case-utils';

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
      this.getFullUrl(`/auth/external-login?provider=${CaseUtils.toPascalCase(provider)}&returnUrl=${encodeURIComponent(returnUrl ?? '/')}`),
    logout: this.getFullUrl(`/auth/logout`),
    exchangeCode: this.getFullUrl(`/auth/exchange-code`),
    verifyOtp: this.getFullUrl(`/auth/verify-otp`),
    forgotPassword: this.getFullUrl(`/auth/forgot-password`),
    resetPassword: this.getFullUrl(`/auth/reset-password`),
    resendOtp: this.getFullUrl(`/auth/resend-otp`),
  };
  user= {
    bootstrap: this.getFullUrl(`/me/bootstrap`),
    profile: {
      reviewSummary: this.getFullUrl('/user/profile/review-summary'),
      savePrereq: this.getFullUrl(`/profiles/prereq`),
      savePersonal: this.getFullUrl(`/profiles/personal`),
      saveContact: this.getFullUrl(`/profiles/contact`),
      saveEducation: this.getFullUrl(`/profiles/education`),
      deleteEducation: (id: string) => this.getFullUrl(`/profiles/education/${id}/delete`),
      saveExperience: this.getFullUrl(`/profiles/experience`),
      deleteExperience: (id: string) => this.getFullUrl(`/profiles/experience/${id}/delete`),
      deleteTrainingCourse: (id: string) => this.getFullUrl(`/profiles/training/${id}/delete`),
      saveAchievements: this.getFullUrl(`/profiles/achievements`),
      deleteAchievement: (id: string) => this.getFullUrl(`/profiles/achievement/${id}/delete`),
      saveSkills: this.getFullUrl(`/profiles/skills`),
      deleteSkill: (id: string) => this.getFullUrl(`/profiles/skills/${id}/delete`),
      deleteLanguage: (id: string) => this.getFullUrl(`/profiles/languages/${id}/delete`),
      saveLanguages: this.getFullUrl(`/profiles/languages`),
      saveReferences: this.getFullUrl(`/profiles/references`),
      submit: this.getFullUrl(`/profiles/submit`),
      checkProfile: this.getFullUrl(`/profiles/check-profile`),
    },
    verify:{
      phone:{
        request: this.getFullUrl(`/user/verify/phone/request`),
        confirm: this.getFullUrl(`/user/verify/phone/confirm`),
      },
      email:{
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
    lookups: {
      invitationStatuses: this.getFullUrl(`/dashboard/lookups/invitation-statuses`),
      jobCategories: this.getFullUrl(`/dashboard/lookups/job-categories`),
      departments: this.getFullUrl(`/dashboard/lookups/department`)
    }
  };
}
