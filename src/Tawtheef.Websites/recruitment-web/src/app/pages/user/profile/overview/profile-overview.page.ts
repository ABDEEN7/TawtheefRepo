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
import { DialogService } from 'primeng/dynamicdialog';
import { TooltipModule } from 'primeng/tooltip';

import { ProfileService } from '../wizard-profile/services/profile.service';
import { ProfileLookupsService } from '../wizard-profile/services/profile-lookups.service';
import { mapIdToDropdown } from '../wizard-profile/services/profile.mapper';
import {ProfileOverviewService} from './services/profile-overview.service';
import {
  MyProfileReviewSummaryDto,
  MyProfileReviewNoteDto,
  ProfileChangeRequestDto,
  ProfileChangeRequestStatusEnum,
  ProfileSectionEnum, ReviewStatusEnum, ReviewTargetTypeEnum, UserProfileStatusEnum
} from './models/profile-overview.model';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {FileUtilsService} from '../../../../core/utils/file-utils';
import { routes } from '../../../../routes/routes';
import {
  AchievementDto, AdditionalAttachmentDto,
  ExperienceDto, FileRefDto, LanguageDto,
  ProfileStatusDto, QualificationDto,
  TrainingCourseDto, SkillDto
} from '../../../../core/models/auth/auth-response.model';
import {createProfileOverviewVisibility} from './services/profile-overview.visibility';
import { ReviewStepsDialogComponent } from './dialogs/review-steps-dialog/review-steps-dialog.component';
import { ReviewItemEditDialogComponent } from './dialogs/review-item-edit-dialog/review-item-edit-dialog.component';

type ProfileEditSection =
  | 'prerequisites'
  | 'personal'
  | 'contact'
  | 'qualifications'
  | 'experience'
  | 'achievements'
  | 'skills'
  | 'languages'
  | 'attachments';

const REVIEW_STEP = 10;
const SECTION_STEP_MAP: Record<ProfileSectionEnum, number> = {
  [ProfileSectionEnum.Prerequisites]: 1,
  [ProfileSectionEnum.Personal]: 2,
  [ProfileSectionEnum.Contact]: 3,
  [ProfileSectionEnum.Qualifications]: 4,
  [ProfileSectionEnum.Experience]: 5,
  [ProfileSectionEnum.TrainingCourses]: 5,
  [ProfileSectionEnum.CertificatesAndAwards]: 6,
  [ProfileSectionEnum.Skills]: 7,
  [ProfileSectionEnum.Languages]: 8,
  [ProfileSectionEnum.Attachments]: 9
};

const SECTION_EDIT_SEGMENT_MAP: Record<ProfileSectionEnum, ProfileEditSection> = {
  [ProfileSectionEnum.Prerequisites]: 'prerequisites',
  [ProfileSectionEnum.Personal]: 'personal',
  [ProfileSectionEnum.Contact]: 'contact',
  [ProfileSectionEnum.Qualifications]: 'qualifications',
  [ProfileSectionEnum.Experience]: 'experience',
  [ProfileSectionEnum.TrainingCourses]: 'experience',
  [ProfileSectionEnum.CertificatesAndAwards]: 'achievements',
  [ProfileSectionEnum.Skills]: 'skills',
  [ProfileSectionEnum.Languages]: 'languages',
  [ProfileSectionEnum.Attachments]: 'attachments'
};

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
    I18nNamespaceDirective,
    TooltipModule
  ],
  templateUrl: './profile-overview.page.html',
  styleUrls: ['./profile-overview.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [DialogService]
})
export class ProfileOverviewPage {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly profileService = inject(ProfileService);
  private readonly profileOverviewService = inject(ProfileOverviewService);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly router = inject(Router);
  private readonly i18n = inject(TranslateService);
  private readonly dialogService = inject(DialogService);
  protected readonly ProfileSectionEnum = ProfileSectionEnum;
  protected readonly ReviewTargetTypeEnum = ReviewTargetTypeEnum;
  protected readonly sectionNav = [
    { id: 'sec-prerequisites', icon: 'pi pi-file', labelKey: 'profileOverview.sections.prerequisites', section: ProfileSectionEnum.Prerequisites, step: SECTION_STEP_MAP[ProfileSectionEnum.Prerequisites] },
    { id: 'sec-personal', icon: 'pi pi-id-card', labelKey: 'profileOverview.sections.personal', section: ProfileSectionEnum.Personal, step: SECTION_STEP_MAP[ProfileSectionEnum.Personal] },
    { id: 'sec-contact', icon: 'pi pi-map-marker', labelKey: 'profileOverview.sections.contact', section: ProfileSectionEnum.Contact, step: SECTION_STEP_MAP[ProfileSectionEnum.Contact] },
    { id: 'sec-qualifications', icon: 'pi pi-graduation-cap', labelKey: 'profileOverview.sections.qualifications', section: ProfileSectionEnum.Qualifications, step: SECTION_STEP_MAP[ProfileSectionEnum.Qualifications] },
    { id: 'sec-experiences', icon: 'pi pi-briefcase', labelKey: 'profileOverview.sections.experiences', section: ProfileSectionEnum.Experience, step: SECTION_STEP_MAP[ProfileSectionEnum.Experience] },
    { id: 'sec-training', icon: 'pi pi-book', labelKey: 'profileOverview.sections.trainingCourses', section: ProfileSectionEnum.TrainingCourses, step: SECTION_STEP_MAP[ProfileSectionEnum.TrainingCourses] },
    { id: 'sec-achievements', icon: 'pi pi-star', labelKey: 'profileOverview.sections.certificatesAndAwards', section: ProfileSectionEnum.CertificatesAndAwards, step: SECTION_STEP_MAP[ProfileSectionEnum.CertificatesAndAwards] },
    { id: 'sec-skills', icon: 'pi pi-bolt', labelKey: 'profileOverview.sections.skills', section: ProfileSectionEnum.Skills, step: SECTION_STEP_MAP[ProfileSectionEnum.Skills] },
    { id: 'sec-languages', icon: 'pi pi-language', labelKey: 'profileOverview.sections.languages', section: ProfileSectionEnum.Languages, step: SECTION_STEP_MAP[ProfileSectionEnum.Languages] },
    { id: 'sec-attachments', icon: 'pi pi-paperclip', labelKey: 'profileOverview.sections.attachments', section: ProfileSectionEnum.Attachments, step: SECTION_STEP_MAP[ProfileSectionEnum.Attachments] }
  ];

  data = rxResource({
    stream: () => forkJoin({
      profile: this.profileService.getProfileStatus(),
      review: this.profileOverviewService.getMyProfileReviewSummary(),
      changes: this.profileOverviewService.getMyChangeRequests()
    })
  });

  loading = computed(() => this.data.status() === 'loading');
  error = computed(() => this.data.error() ? this.i18n.instant('common.loadFailed') : null);

  vm = computed(() => {
    const p = this.data.value()?.profile as ProfileStatusDto | undefined;
    const review = this.data.value()?.review as MyProfileReviewSummaryDto | undefined;
    const changeRequests = (this.data.value()?.changes as ProfileChangeRequestDto[] | undefined) ?? [];
    if (!p || !review) return null;

    const statusUi = this.mapStatus(review.profileStatus);
    const reviewSections = (review.sections ?? []).map(s => ({
      ...s,
      labelKey: this.sectionLabelKey(s.section),
      scrollId: 'sec-' + this.sectionSlug(s.section)
    }));
    const sectionIndex = reviewSections.reduce((acc, section) => {
      acc[section.section] = section.notesCount ?? 0;
      return acc;
    }, {} as Record<number, number>);
    const sectionCorrections = reviewSections.reduce((acc, section) => {
      const notes = section.notes ?? [];
      acc[section.section] = notes.some(n => n.status === ReviewStatusEnum.NeedsCorrection);
      return acc;
    }, {} as Record<number, boolean>);

    const visibility = createProfileOverviewVisibility(p);
    return {
      visibility:{
        ...visibility(),
      },
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
        sections: reviewSections,
        sectionIndex,
        sectionCorrections
      },
      prereq: {
        candidateType: p.candidateType,
        targetEntity: p.targetEntity,
        office: p.office,

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
        nationality: p.nationality,
        gender: p.gender,
        religion: p.religion,
        maritalStatus: p.maritalStatus,
        hasDisability: p.hasDisability,
        disabilityDetails: p.disabilityDetails ?? null,

        sponsorType: p.sponsorType,
        sponsorEmployerName: p.sponsorEmployerName ?? null,
        sponsorEmployerNumber: p.sponsorEmployerNumber ?? null,
        sponsorQidExpiry: p.sponsorQidExpiry ?? null
      },
      contact: {
        residenceCountry: p.residenceCountry,
        interviewLocation: p.interviewLocation,
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
      attachments: (p.additionalAttachments ?? []) as AdditionalAttachmentDto[],
      changeRequests: changeRequests.map(req => ({
        ...req,
        sectionLabelKey: this.sectionLabelKey(req.section),
        statusLabelKey: this.changeStatusLabelKey(req.status),
        statusSeverity: this.changeStatusSeverity(req.status)
      }))
    };
  });
  prereqFiles = computed(() => {
    const v = this.vm();
    if (!v) return [];
    const vis = v.visibility;
    return [
      { key: 'resume', titleKey: 'profileOverview.files.resume', file: v.prereq.resume },
      { key: 'nationalCard', titleKey: 'profileOverview.files.nationalCard', file: v.prereq.nationalCard },
      ...(vis.needsBirth ? [{
        key: 'birthdayCertificate',
        titleKey: 'profileOverview.files.birthdayCertificate',
        file: v.prereq.birthdayCertificate
      }] : []),
      ...(vis.needsMarriage ? [{
        key: 'marriageCertificate',
        titleKey: 'profileOverview.files.marriageCertificate',
        file: v.prereq.marriageCertificate
      }] : []),
      ...(vis.isResident ? [{
        key: 'residenceAddressCertificate',
        titleKey: 'profileOverview.files.residenceAddressCertificate',
        file: v.prereq.residenceAddressCertificate
      }] : []),
      ...(vis.needsSponsor ? [{
        key: 'sponsorCard',
        titleKey: 'profileOverview.files.sponsorCard',
        file: v.prereq.sponsorCard
      }] : []),
    ];
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

  private isProfileComplete(): boolean {
    const profile = this.data.value()?.profile as ProfileStatusDto | undefined;
    return !!profile?.isComplete;
  }

  private findFileUrl(note: MyProfileReviewNoteDto): string | null {
    if (!note.resourceId) return null;

    const targetId = (note.resourceId as string).toLowerCase();
    const vm = this.vm();
    if (!vm) return null;

    const prereqFiles = [
      vm.prereq.resume,
      vm.prereq.nationalCard,
      vm.prereq.birthdayCertificate,
      vm.prereq.marriageCertificate,
      vm.prereq.residenceAddressCertificate,
      vm.prereq.sponsorCard
    ];

    const sectionFiles = [
      ...vm.qualifications.map(q => q.attachment ?? null),
      ...vm.experiences.map(e => e.attachment ?? null),
      ...vm.trainingCourses.map(t => t.attachment ?? null),
      ...vm.achievements.map(a => a.attachment ?? null),
      ...vm.attachments.map(a => a.file ?? null)
    ];

    const match = [...prereqFiles, ...sectionFiles].find(f =>
      f?.resourceId?.toString().toLowerCase() === targetId
    );

    return match?.url ?? null;
  }

  editSection(section: ProfileSectionEnum) {
    const viewModel = this.vm();
    if (viewModel && this.isLockedForReview(viewModel.status.value)) return;

    if (!this.isProfileComplete()) {
      const step = SECTION_STEP_MAP[section] ?? 1;
      this.navigateToWizard(step);
      return;
    }

    this.navigateToEditSection(section);
  }

  openWizard() {
    if (!this.isProfileComplete()) {
      this.navigateToWizard();
      return;
    }

    this.navigateToEditSection(ProfileSectionEnum.Prerequisites);
  }

  openReviewStep() {
    if (!this.isProfileComplete()) {
      this.navigateToWizard(REVIEW_STEP);
      return;
    }

    const review = this.data.value()?.review as MyProfileReviewSummaryDto | undefined;
    this.dialogService.open(ReviewStepsDialogComponent, {
      header: this.i18n.instant('profileOverview.reviewSteps.dialogTitle'),
      data: { sections: review?.sections ?? [] },
      styleClass: 'w-100 w-md-75'
    })?.onClose.subscribe(result => {
      if (result?.section) {
        this.navigateToEditSection(result.section as ProfileSectionEnum);
      }
    });
  }

  openReviewItem(note: MyProfileReviewNoteDto, section: ProfileSectionEnum) {
    this.dialogService.open(ReviewItemEditDialogComponent, {
      header: this.i18n.instant('profileOverview.reviewItemDialog.title'),
      data: {
        note,
        section,
        sectionLabel: this.sectionLabelKey(section),
        canEdit: this.canShowEdit(section),
        fileUrl: this.findFileUrl(note)
      },
      styleClass: 'w-100 w-md-50'
    })?.onClose.subscribe(result => {
      if (!result) return;
      if (result.section) {
        this.navigateToEditSection(result.section as ProfileSectionEnum);
        return;
      }
      if (result.preview) {
        this.fileUtils.previewUrl(result.preview as string);
      }
    });
  }

  // ================== UI Mapping ==================
  private mapStatus(status: number): {
    severity: 'success'|'warn'|'danger'|'info'|'secondary',
    labelKey: string,
    hintKey: string
  } {
    switch (status) {
      case UserProfileStatusEnum.Submitted:
        return { severity: 'info', labelKey: 'profileOverview.status.submitted', hintKey: 'profileOverview.statusHint.submitted' };
      case UserProfileStatusEnum.UnderReview:
        return { severity: 'info', labelKey: 'profileOverview.status.underReview', hintKey: 'profileOverview.statusHint.underReview' };
      case UserProfileStatusEnum.RequiresUpdate:
        return { severity: 'warn', labelKey: 'profileOverview.status.requiresUpdate', hintKey: 'profileOverview.statusHint.requiresUpdate' };
      case UserProfileStatusEnum.Approved:
        return { severity: 'success', labelKey: 'profileOverview.status.approved', hintKey: 'profileOverview.statusHint.approved' };
      case UserProfileStatusEnum.Rejected:
        return { severity: 'danger', labelKey: 'profileOverview.status.rejected', hintKey: 'profileOverview.statusHint.rejected' };
      case UserProfileStatusEnum.AdminCancelled:
        return { severity: 'secondary', labelKey: 'profileOverview.status.adminCancelled', hintKey: 'profileOverview.statusHint.adminCancelled' };
      case UserProfileStatusEnum.InCreation:
      default:
        return { severity: 'secondary', labelKey: 'profileOverview.status.inCreation', hintKey: 'profileOverview.statusHint.inCreation' };
    }
  }
  canShowEdit(section: ProfileSectionEnum): boolean {
    const v = this.vm();
    if (!v) return false;

    const status = v.status.value;
    if (this.isLockedForReview(status)) return false;

    if (status === UserProfileStatusEnum.RequiresUpdate || status === UserProfileStatusEnum.Rejected) {
      return v.review.sectionCorrections?.[section] ?? false;
    }

    if (status === UserProfileStatusEnum.Approved) return true;

    if (status === UserProfileStatusEnum.InCreation) return true;

    return false;
  }
  private isLockedForReview(status: number): boolean {
    return status === UserProfileStatusEnum.UnderReview || status === UserProfileStatusEnum.Submitted;
  }
  private sectionLabelKey(section: number): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites: return 'profileOverview.sections.prerequisites';
      case ProfileSectionEnum.Personal: return 'profileOverview.sections.personal';
      case ProfileSectionEnum.Contact: return 'profileOverview.sections.contact';
      case ProfileSectionEnum.Qualifications: return 'profileOverview.sections.qualifications';
      case ProfileSectionEnum.Experience: return 'profileOverview.sections.experiences';
      case ProfileSectionEnum.TrainingCourses: return 'profileOverview.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards: return 'profileOverview.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills: return 'profileOverview.sections.skills';
      case ProfileSectionEnum.Languages: return 'profileOverview.sections.languages';
      case ProfileSectionEnum.Attachments: return 'profileOverview.sections.attachments';
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

  private navigateToWizard(step?: number) {
    const profile = this.data.value()?.profile ?? null;
    const queryParams = step ? { step } : undefined;
    this.router.navigate(
      [routes.user.profileWizard],
      {
        queryParams,
        state: profile ?? undefined
      }
    );
  }

  private navigateToEditSection(section: ProfileSectionEnum) {
    const segment = SECTION_EDIT_SEGMENT_MAP[section] ?? 'personal';
    this.router.navigate([routes.user.profileEditSection(segment)]);
  }

  noteSeverity(status: number): 'warn' | 'danger' | 'secondary' {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'warn';
    if (status === ReviewStatusEnum.Rejected) return 'danger';
    return 'secondary';
  }

  noteStatusLabelKey(status: number): string {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'profileOverview.reviewStatus.needsCorrection';
    if (status === ReviewStatusEnum.Rejected) return 'profileOverview.reviewStatus.rejected';
    return 'profileOverview.reviewStatus.other';
  }

  changeStatusLabelKey(status: number): string {
    switch (status) {
      case ProfileChangeRequestStatusEnum.Pending:
        return 'profileOverview.changeRequests.status.pending';
      case ProfileChangeRequestStatusEnum.UnderReview:
        return 'profileOverview.changeRequests.status.underReview';
      case ProfileChangeRequestStatusEnum.Approved:
        return 'profileOverview.changeRequests.status.approved';
      case ProfileChangeRequestStatusEnum.Rejected:
        return 'profileOverview.changeRequests.status.rejected';
      case ProfileChangeRequestStatusEnum.Canceled:
        return 'profileOverview.changeRequests.status.canceled';
      default:
        return 'profileOverview.changeRequests.status.pending';
    }
  }

  changeStatusSeverity(status: number): 'success' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case ProfileChangeRequestStatusEnum.Approved:
        return 'success';
      case ProfileChangeRequestStatusEnum.Pending:
      case ProfileChangeRequestStatusEnum.UnderReview:
        return 'warn';
      case ProfileChangeRequestStatusEnum.Rejected:
        return 'danger';
      default:
        return 'secondary';
    }
  }

}
