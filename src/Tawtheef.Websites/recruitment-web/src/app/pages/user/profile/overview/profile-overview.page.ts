import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { ButtonDirective } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { Skeleton } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { Chip } from 'primeng/chip';
import { rxResource } from '@angular/core/rxjs-interop';

import { ProfileService } from '../wizard-profile/services/profile.service';
import { ProfileLookupsService } from '../wizard-profile/services/profile-lookups.service';
import { mapIdToDropdown } from '../wizard-profile/services/profile.mapper';
import {ProfileOverviewService} from './services/profile-overview.service';
import {
  MyProfileReviewSummaryDto,
  ProfileSectionEnum, ReviewStatusEnum, UserProfileStatusEnum
} from './models/profile-overview.model';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {FileUtilsService} from '../../../../core/utils/file-utils';
import {
  AchievementDto, AdditionalAttachmentDto,
  ExperienceDto, FileRefDto, LanguageDto,
  ProfileStatusDto, QualificationDto,
  TrainingCourseDto, SkillDto
} from '../../../../core/models/auth/auth-response.model';

@Component({
  selector: 'app-profile-view-page',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    ButtonDirective,
    Tag,
    Skeleton,
    TableModule,
    Chip,
    I18nNamespaceDirective
  ],
  templateUrl: './profile-overview.page.html',
  styleUrls: ['./profile-overview.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileOverviewPage {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly profileService = inject(ProfileService);
  private readonly profileOverviewService = inject(ProfileOverviewService);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly router = inject(Router);
  private readonly i18n = inject(TranslateService);
  protected readonly ProfileSectionEnum = ProfileSectionEnum;

  data = rxResource({
    stream: () => forkJoin({
      lookups: this.lookups.loadAll(),
      profile: this.profileService.getProfileStatus(),
      review: this.profileOverviewService.getMyProfileReviewSummary(),
    })
  });

  loading = computed(() => this.data.status() === 'loading');
  error = computed(() => this.data.error() ? this.i18n.instant('common.loadFailed') : null);

  vm = computed(() => {
    const p = this.data.value()?.profile as ProfileStatusDto | undefined;
    const review = this.data.value()?.review as MyProfileReviewSummaryDto | undefined;
    if (!p || !review) return null;

    const statusUi = this.mapStatus(review.profileStatus);

    return {
      header: {
        avatar: p.avatar ?? null,
        fullName: (p.fullNameEn || p.fullNameAr || '').trim(),
        email: p.email ?? '',
        phone: p.phone ?? '',
        emailVerified: p.emailVerified,
        phoneVerified: p.phoneVerified,
      },
      status: {
        value: review.profileStatus,
        labelKey: statusUi.labelKey,
        severity: statusUi.severity,
        hintKey: statusUi.hintKey,
      },
      review: {
        totalNotes: review.totalNotes ?? 0,
        sections: (review.sections ?? []).map(s => ({
          ...s,
          labelKey: this.sectionLabelKey(s.section),
          editRoute: this.sectionEditRoute(s.section),
          scrollId: 'sec-' + this.sectionSlug(s.section)
        }))
      },

      prereq: {
        candidateType: mapIdToDropdown(this.lookups, 'candidateType', p.candidateTypeId),
        targetEntity: mapIdToDropdown(this.lookups, 'targetEntity', p.targetEntityId),
        office: mapIdToDropdown(this.lookups, 'office', p.officeId),

        resume: p.resumeAttachment ?? null,
        nationalCard: p.nationalCard ?? null,
        birthdayCertificate: p.birthdayCertificate ?? null,
        marriageCertificate: p.marriageCertificate ?? null,
        sponsorCard: p.sponsorCard ?? null,
        residenceAddressCertificate: p.residenceAddressCertificate ?? null
      },

      personal: {
        nationalNumber: p.nationalNumber ?? null,
        qidExpiry: p.qidExpiry ?? null,
        birthDate: p.birthDate ?? null,
        nationality: mapIdToDropdown(this.lookups, 'countries', p.nationalityId),
        gender: mapIdToDropdown(this.lookups, 'gender', p.genderId),
        religion: mapIdToDropdown(this.lookups, 'religion', p.religionId),
        maritalStatus: mapIdToDropdown(this.lookups, 'marital', p.maritalStatusId),
        childrenCount: p.childrenCount ?? 0,
        hasDisability: p.hasDisability,
        disabilityDetails: p.disabilityDetails ?? null,

        sponsorType: mapIdToDropdown(this.lookups, 'sponsorType', p.sponsorTypeId),
        sponsorEmployerName: p.sponsorEmployerName ?? null,
        sponsorEmployerNumber: p.sponsorEmployerNumber ?? null,
        sponsorQidExpiry: p.sponsorQidExpiry ?? null
      },

      contact: {
        residenceCountry: mapIdToDropdown(this.lookups, 'countries', p.residenceCountryId),
        interviewLocation: mapIdToDropdown(this.lookups, 'countries', p.interviewLocationId),
        address: p.address ?? null,
        naZone: p.naZone ?? null,
        naStreet: p.naStreet ?? null,
        naBuilding: p.naBuilding ?? null,
        naUnit: p.naUnit ?? null
      },

      qualifications: (p.qualifications ?? []) as QualificationDto[],
      experiences: (p.experiences ?? []) as ExperienceDto[],
      trainingCourses: (p.trainingCourses ?? []) as TrainingCourseDto[],
      achievements: (p.achievements ?? []) as AchievementDto[],
      skills: (p.skills ?? []) as SkillDto[],
      languages: (p.languages ?? []) as LanguageDto[],
      attachments: (p.additionalAttachments ?? []) as AdditionalAttachmentDto[]
    };
  });

  openFile(file: FileRefDto | null): void {
    if (!file?.url) return;
    this.fileUtils.previewUrl(file.url);
  }

  reload(): void {
    this.data.reload();
  }

  scrollTo(id: string) {
    const el = document.getElementById(id);
    if (!el) return;
    const y = el.getBoundingClientRect().top + window.scrollY - 12;
    window.scrollTo({ top: y, behavior: 'smooth' });
  }

  editSectionByRoute(route: string) {
    this.router.navigate([route]);
  }

  editSection(section: ProfileSectionEnum) {
    this.router.navigate([this.sectionEditRoute(section)]);
  }

  // ================== UI Mapping ==================
  private mapStatus(status: number): {
    severity: 'success'|'warn'|'danger'|'info'|'secondary',
    labelKey: string,
    hintKey: string
  } {
    switch (status) {
      case UserProfileStatusEnum.Submitted:
        return { severity: 'info', labelKey: 'profileView.status.submitted', hintKey: 'profileView.statusHint.submitted' };
      case UserProfileStatusEnum.UnderReview:
        return { severity: 'info', labelKey: 'profileView.status.underReview', hintKey: 'profileView.statusHint.underReview' };
      case UserProfileStatusEnum.RequiresUpdate:
        return { severity: 'warn', labelKey: 'profileView.status.requiresUpdate', hintKey: 'profileView.statusHint.requiresUpdate' };
      case UserProfileStatusEnum.Approved:
        return { severity: 'success', labelKey: 'profileView.status.approved', hintKey: 'profileView.statusHint.approved' };
      case UserProfileStatusEnum.Rejected:
        return { severity: 'danger', labelKey: 'profileView.status.rejected', hintKey: 'profileView.statusHint.rejected' };
      case UserProfileStatusEnum.AdminCancelled:
        return { severity: 'secondary', labelKey: 'profileView.status.adminCancelled', hintKey: 'profileView.statusHint.adminCancelled' };
      case UserProfileStatusEnum.InCreation:
      default:
        return { severity: 'secondary', labelKey: 'profileView.status.inCreation', hintKey: 'profileView.statusHint.inCreation' };
    }
  }
  private sectionLabelKey(section: number): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites: return 'profileView.sections.prerequisites';
      case ProfileSectionEnum.Personal: return 'profileView.sections.personal';
      case ProfileSectionEnum.Contact: return 'profileView.sections.contact';
      case ProfileSectionEnum.Qualifications: return 'profileView.sections.qualifications';
      case ProfileSectionEnum.Experience: return 'profileView.sections.experiences';
      case ProfileSectionEnum.TrainingCourses: return 'profileView.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards: return 'profileView.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills: return 'profileView.sections.skills';
      case ProfileSectionEnum.Languages: return 'profileView.sections.languages';
      case ProfileSectionEnum.Attachments: return 'profileView.sections.attachments';
      default: return 'common.section';
    }
  }

  private sectionSlug(section: number): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites: return 'prerequisites';
      case ProfileSectionEnum.Personal: return 'personal';
      case ProfileSectionEnum.Contact: return 'contact';
      case ProfileSectionEnum.Qualifications: return 'qualifications';
      case ProfileSectionEnum.Experience: return 'experiences';
      case ProfileSectionEnum.TrainingCourses: return 'training';
      case ProfileSectionEnum.CertificatesAndAwards: return 'achievements';
      case ProfileSectionEnum.Skills: return 'skills';
      case ProfileSectionEnum.Languages: return 'languages';
      case ProfileSectionEnum.Attachments: return 'attachments';
      default: return 'overview';
    }
  }

  private sectionEditRoute(section: number): string {
    return `user/profile/edit/${this.sectionSlug(section)}`;
  }
  noteSeverity(status: number): 'warn' | 'danger' | 'secondary' {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'warn';
    if (status === ReviewStatusEnum.Rejected) return 'danger';
    return 'secondary';
  }

  noteStatusLabelKey(status: number): string {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'profileView.reviewStatus.needsCorrection';
    if (status === ReviewStatusEnum.Rejected) return 'profileView.reviewStatus.rejected';
    return 'profileView.reviewStatus.other';
  }

}
