import { ChangeDetectionStrategy, Component, computed, inject, signal, effect, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ButtonDirective } from 'primeng/button';
import { Skeleton } from 'primeng/skeleton';
import { ProgressSpinner } from 'primeng/progressspinner';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ToggleSwitchModule } from 'primeng/toggleswitch';

import {
  ProfileSectionEnum,
  ProfileChangeRequestDto,
  ProfileChangeRequestStatusEnum,
  ReviewStatusEnum,
  MyProfileReviewSummaryDto,
  UserProfileStatusEnum,
  MyProfileReviewNoteDto,
  ProfileChangeActionEnum,
  ReviewTargetTypeEnum,
  MyProfileReviewChangedItemDto
} from './models/profile-overview.model';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { ProfileEditDialogComponent } from './dialogs/profile-edit-dialog/profile-edit-dialog.component';
import {
  AdditionalAttachmentDto,
  ProfileStatusDto,
  QualificationDto
} from '../../../../core/models/auth/auth-response.model';
import { ProfilePrerequisitesSectionComponent } from './sections/prerequisites/prerequisites-section.component';
import { ProfilePersonalSectionComponent } from './sections/personal/personal-section.component';
import { ProfileContactSectionComponent } from './sections/contact/contact-section.component';
import { ProfileQualificationsSectionComponent } from './sections/qualifications/qualifications-section.component';
import { ProfileExperienceSectionComponent } from './sections/experience/experience-section.component';
import { ProfileTrainingSectionComponent } from './sections/training/training-section.component';
import { ProfileAchievementsSectionComponent } from './sections/achievements/achievements-section.component';
import { ProfileSkillsSectionComponent } from './sections/skills/skills-section.component';
import { ProfileLanguagesSectionComponent } from './sections/languages/languages-section.component';
import { ProfileAttachmentsSectionComponent } from './sections/attachments/attachments-section.component';
import { ProfileViewCqrs } from './profile-view.cqrs';
import { changeRequestDto } from './dtos/change-request-dto';
import { applyFieldChanges, detectChangedFields, FieldChange } from './utils/detect-change-fields';
import { AvatarUtils } from '../../../../core/utils/avatar-utils';
import { ProfileService } from '../wizard-profile/services/profile.service';
import { ProfileLookupsService } from '../wizard-profile/services/profile-lookups.service';
import { PROFILE_WRITE_MODE } from '../wizard-profile/services/profile-write-mode.token';
import { ProfileOverviewService } from './services/profile-overview.service';
import { createProfileOverviewVisibility, ProfileOverviewVisibility } from './services/profile-overview.visibility';
import { LanguageService } from '../../../../core/services/language.service';
import { AuthStateService } from '../../../../core/auth/auth-state.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { finalize, take } from 'rxjs';
import { ResubmitConfirmDialogComponent, ResubmitConfirmSectionVm } from './dialogs/resubmit-confirm-dialog/resubmit-confirm-dialog.component';

interface SectionCard {
  section: ProfileSectionEnum;
  icon: string;
  labelKey: string;
}
// Base (non-generic) rxResource instance type
type AnyRxRes = ReturnType<typeof rxResource>;
// Generic wrapper: same shape, but value() is strongly typed
type RxRes<T> = Omit<AnyRxRes, 'value'> & { value: () => T | undefined };
@Component({
  selector: 'app-profile-view-page',
  standalone: true,
  providers: [
    DialogService,
    ProfileService,
  ],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    Skeleton,
    TooltipModule,
    ToggleSwitchModule,
    I18nNamespaceDirective,
    ProfilePrerequisitesSectionComponent,
    ProfilePersonalSectionComponent,
    ProfileContactSectionComponent,
    ProfileQualificationsSectionComponent,
    ProfileExperienceSectionComponent,
    ProfileTrainingSectionComponent,
    ProfileAchievementsSectionComponent,
    ProfileSkillsSectionComponent,
    ProfileLanguagesSectionComponent,
    ProfileAttachmentsSectionComponent,
    ButtonDirective,
    ProgressSpinner,
  ],
  templateUrl: './profile-view.page.html',
  styleUrls: ['./profile-view.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileViewPage {
  private readonly i18n = inject(TranslateService);
  private readonly overviewService = inject(ProfileOverviewService);
  private readonly profileCqrs = inject(ProfileViewCqrs);
  private readonly dialogService = inject(DialogService);
  private readonly lookups = inject(ProfileLookupsService);
  protected readonly languageService = inject(LanguageService);
  private readonly profileService = inject(ProfileService);
  private readonly authState = inject(AuthStateService);
  private readonly notify = inject(NotificationService);
  protected readonly ProfileSectionEnum = ProfileSectionEnum;
  private readonly sectionDataFieldPath = 'SectionData';
  protected readonly resubmitting = signal(false);
  protected readonly availabilitySaving = signal(false);
  private readonly emptyVisibility: ProfileOverviewVisibility = {
    type: undefined,
    isResident: false,
    needsSponsor: false,
    needsBirth: false,
    needsMarriage: false,
    showQidExpiry: false,
    showNationalAddress: false,
    showForeignAddress: true,
    showSponsorSection: false
  };

  private readonly modeEffect = effect(() => {
    const status = this.profileStatus();
    if (status === UserProfileStatusEnum.Approved) {
      this.profileService.setWriteMode('change-request');
    } else if (status === UserProfileStatusEnum.RequiresUpdate || status === UserProfileStatusEnum.Submitted) {
      this.profileService.setWriteMode('review-edit');
    } else {
      this.profileService.setWriteMode('create');
    }
  });

  protected readonly cards: SectionCard[] = [
    { section: ProfileSectionEnum.Prerequisites, icon: 'pi pi-file', labelKey: 'profileOverview.sections.prerequisites' },
    { section: ProfileSectionEnum.Personal, icon: 'pi pi-id-card', labelKey: 'profileOverview.sections.personal' },
    { section: ProfileSectionEnum.Contact, icon: 'pi pi-map-marker', labelKey: 'profileOverview.sections.contact' },
    { section: ProfileSectionEnum.Qualifications, icon: 'pi pi-graduation-cap', labelKey: 'profileOverview.sections.qualifications' },
    { section: ProfileSectionEnum.Experience, icon: 'pi pi-briefcase', labelKey: 'profileOverview.sections.experiences' },
    { section: ProfileSectionEnum.TrainingCourses, icon: 'pi pi-book', labelKey: 'profileOverview.sections.trainingCourses' },
    { section: ProfileSectionEnum.CertificatesAndAwards, icon: 'pi pi-star', labelKey: 'profileOverview.sections.certificatesAndAwards' },
    { section: ProfileSectionEnum.Skills, icon: 'pi pi-bolt', labelKey: 'profileOverview.sections.skills' },
    { section: ProfileSectionEnum.Languages, icon: 'pi pi-language', labelKey: 'profileOverview.sections.languages' },
    { section: ProfileSectionEnum.Attachments, icon: 'pi pi-paperclip', labelKey: 'profileOverview.sections.attachments' }
  ];
  readonly pageLoading = computed(() =>
    this.basics.status() === 'loading' ||
    this.review.status() === 'loading' ||
    this.changeRequests.status() === 'loading'
  );
  readonly reviewLoading = computed(() => this.review.status() === 'loading');
  readonly changeRequestsLoading = computed(() => this.changeRequests.status() === 'loading');
  readonly summaryLoading = computed(() => this.reviewLoading() || this.changeRequestsLoading());
  get keyLabel() {
    return this.cards.find(c => c.section === this.expanded())?.labelKey;
  }
  get avatar() {
    return this.header()?.avatar || AvatarUtils.build(this.header()?.fullNameEn ?? null);
  }
  defaultAvatar = AvatarUtils.default;
  get canReplaceAttachment() {
    const status = this.profileStatus();
    return status === UserProfileStatusEnum.RequiresUpdate;
  }
  get enableChangeMode() {
    return false;
  }
  private readonly basics = rxResource({
    params: () => true,
    stream: () => this.profileCqrs.basics()
  });

  private readonly review = rxResource({
    params: () => true,
    stream: () => this.overviewService.getMyProfileReviewSummary(),
  });

  private readonly changeRequests = rxResource({
    params: () => true,
    stream: () => this.overviewService.getMyChangeRequests(),
  });

  private readonly sections = new Map<ProfileSectionEnum, RxRes<ProfileStatusDto>>();
  protected readonly expanded = signal<ProfileSectionEnum>(ProfileSectionEnum.Personal);

  protected changes(section: ProfileSectionEnum) {
    if (this.profileStatus() !== UserProfileStatusEnum.Approved) return [];
    const sectionChanges = this.changeRequestsVm()
      .filter(cr => cr.section === section);

    let resultChanges: FieldChange[] = [];
    sectionChanges.forEach((change) => {
      if (change.action === ProfileChangeActionEnum.UpdateField) {
        resultChanges = resultChanges.concat(
          detectChangedFields(change.oldValue ?? '', change.newValue ?? '')
        );
        return;
      }

      if (
        change.action === ProfileChangeActionEnum.AddListItem ||
        change.action === ProfileChangeActionEnum.ReplaceAttachment
      ) {
        const field = change.fieldPath ?? change.targetKey ?? '';
        if (!field) return;
        resultChanges.push({
          field,
          oldValue: parseJsonValue(change.oldValue),
          newValue: parseJsonValue(change.newValue)
        });
      }
    });

    return resultChanges;
  }

  constructor() {
    this.cards.forEach(card => {
      this.sections.set(
        card.section,
        rxResource<ProfileStatusDto, unknown>({
          stream: () => this.profileCqrs.section(card.section),
        })
      );
    });

    this.lookups.loadAll().subscribe(() => { });
  }

  readonly header = computed(() => this.basics.value());
  protected readonly availableForRecruitment = linkedSignal(() => this.header()?.availableForRecruitment ?? true);
  readonly visibility = computed<ProfileOverviewVisibility>(() => {
    const profile = this.basics.value();
    if (!profile) return this.emptyVisibility;
    return createProfileOverviewVisibility(profile)();
  });

  readonly reviewNotes = computed(() => {
    const review = this.review.value() as MyProfileReviewSummaryDto | undefined;
    if (!review) return undefined;

    const sectionIndex = (review.sections ?? []).reduce((acc, section) => {
      acc[section.section] = this.actionableNotesCount(section.section as ProfileSectionEnum, section.notes ?? []);
      return acc;
    }, {} as Record<number, number>);

    return { ...review, sectionIndex } as MyProfileReviewSummaryDto & { sectionIndex: Record<number, number> };
  });

  readonly profileStatus = computed(() => (this.review.value() as MyProfileReviewSummaryDto | undefined)?.profileStatus ?? null);

  readonly statusVm = computed(() => {
    const status = this.profileStatus();
    switch (status) {
      case UserProfileStatusEnum.Approved:
        return {
          labelKey: 'profileOverview.status.approved',
          hintKey: 'profileOverview.statusHint.approved',
          severity: 'chip-ok'
        };
      case UserProfileStatusEnum.RequiresUpdate:
        if (!this.canResubmit()) {
          return {
            labelKey: 'profileOverview.status.correctionsRequired',
            hintKey: 'profileOverview.statusHint.correctionsRequired',
            severity: 'chip-warn'
          };
        }
        return {
          labelKey: 'profileOverview.status.readyForResubmit',
          hintKey: 'profileOverview.statusHint.readyForResubmit',
          severity: 'chip-warn'
        };
      case UserProfileStatusEnum.Rejected:
        return {
          labelKey: 'profileOverview.status.rejected',
          hintKey: 'profileOverview.statusHint.rejected',
          severity: 'chip-warn'
        };
      case UserProfileStatusEnum.UnderReview:
        return {
          labelKey: 'profileOverview.status.underReview',
          hintKey: 'profileOverview.statusHint.underReview',
          severity: 'chip-warn'
        };
      case UserProfileStatusEnum.Submitted:
        return {
          labelKey: 'profileOverview.status.submitted',
          hintKey: 'profileOverview.statusHint.submitted',
          severity: 'chip-warn'
        };
      case UserProfileStatusEnum.InCreation:
        return {
          labelKey: 'profileOverview.status.inCreation',
          hintKey: 'profileOverview.statusHint.inCreation',
          severity: 'chip-warn'
        };
      case UserProfileStatusEnum.AdminCancelled:
        return {
          labelKey: 'profileOverview.status.adminCancelled',
          hintKey: 'profileOverview.statusHint.adminCancelled',
          severity: 'chip-warn'
        };
      default:
        return {
          labelKey: 'profileOverview.status.pending',
          hintKey: 'profileOverview.statusDescriptions.pending',
          severity: 'chip-warn'
        };
    }
  });

  get canAddAttachments() {
    return this.enableChangeMode;
  }
  canEditSections(section: ProfileSectionEnum) {
    const status = this.profileStatus();
    if (status === UserProfileStatusEnum.RequiresUpdate) {
      if (this.isMixedSection(section)) {
        return this.hasSectionDataNote(section) || this.hasSolvedSectionDataCorrection(section);
      }

      if (section === ProfileSectionEnum.Skills || section === ProfileSectionEnum.Languages) {
        return this.sectionNotes(section).some(note => note.targetType === ReviewTargetTypeEnum.Section) ||
          this.editableItemsForSection(section).some(item => item.targetType === ReviewTargetTypeEnum.Section);
      }

      return false;
    }
    if (status === UserProfileStatusEnum.Approved) {
      // I will change it in future
      return false;
    }
    return false;
  }

  readonly activeSectionNotes = computed(() => {
    const active = this.expanded();
    const review = this.review.value() as MyProfileReviewSummaryDto | undefined;
    const notes = review?.sections?.find(s => s.section === active)?.notes ?? [];
    return notes as MyProfileReviewNoteDto[];
  });

  readonly activeSectionReviewNotes = computed(() => {
    const active = this.expanded();
    const notes = this.activeSectionNotes();
    const sectionNotes = notes.filter(n => n.targetType === ReviewTargetTypeEnum.Section);

    if (this.isSectionNoteActionable(active)) {
      return sectionNotes;
    }

    const targetNotes = notes.filter(n => n.targetType !== ReviewTargetTypeEnum.Section);
    return targetNotes.length > 0 ? sectionNotes : [];
  });

  readonly activeSectionTargetNotes = computed(() => {
    const notes = this.activeSectionNotes();
    return notes.filter(n => n.targetType !== ReviewTargetTypeEnum.Section);
  });

  readonly activeSectionResolvedCorrections = computed(() => {
    const active = this.expanded();
    if (this.profileStatus() !== UserProfileStatusEnum.RequiresUpdate) return false;
    if (this.isSectionNoteActionable(active)) return false;
    if (this.activeSectionTargetNotes().length > 0) return false;

    const notes = this.activeSectionNotes();
    const hasSectionContextNote = notes.some(note => note.targetType === ReviewTargetTypeEnum.Section);
    const hasSavedCorrection = (this.reviewNotes()?.changedSections ?? []).some(section => section === active);
    return hasSectionContextNote && hasSavedCorrection;
  });

  readonly canResubmit = computed(() =>
    this.profileStatus() === UserProfileStatusEnum.RequiresUpdate &&
    !!this.reviewNotes()?.canResubmit
  );

  readonly changedSectionLabels = computed(() => {
    const changed = this.reviewNotes()?.changedSections ?? [];
    return changed.map(section => this.i18n.instant(this.sectionLabelKey(section)));
  });

  readonly changedSectionsForConfirm = computed<ResubmitConfirmSectionVm[]>(() => {
    const changedItems = this.reviewNotes()?.changedItems ?? [];
    const changedSections = this.reviewNotes()?.changedSections ?? [];

    return changedSections.map(section => ({
      section,
      labelKey: this.sectionLabelKey(section),
      items: changedItems.filter(item => item.section === section)
    })).filter(section => section.items.length > 0);
  });

  editableItemsForSection(section: ProfileSectionEnum): MyProfileReviewChangedItemDto[] {
    if (this.profileStatus() !== UserProfileStatusEnum.RequiresUpdate) return [];
    return (this.reviewNotes()?.changedItems ?? []).filter(item => item.section === section);
  }

  openCard(section: ProfileSectionEnum) {
    const res = this.sections.get(section);
    this.expanded.set(section);
    res?.reload();
  }

  reloadSection(section: ProfileSectionEnum) {
    this.sections.get(section)?.reload();
    this.basics.reload();
    this.review.reload();
    this.changeRequests.reload();
    this.authState.resetBootstrap();
    this.authState.getAuthBootstrap$().subscribe();
  }

  onAvailabilityChange(available: boolean) {
    if (this.availabilitySaving()) return;

    const previous = this.availableForRecruitment();
    this.availableForRecruitment.set(available);
    this.availabilitySaving.set(true);

    this.profileService.saveRecruitmentAvailability(available)
      .pipe(
        take(1),
        finalize(() => this.availabilitySaving.set(false))
      )
      .subscribe({
        next: () => {
          this.notify.success(this.i18n.instant('profileView.notifications.saved'));
          this.basics.reload();
          this.authState.resetBootstrap();
          this.authState.getAuthBootstrap$().subscribe();
        },
        error: () => {
          this.availableForRecruitment.set(previous);
          this.notify.error(this.i18n.instant('profileView.notifications.saveFailed'));
        }
      });
  }

  resubmitProfile() {
    if (!this.canResubmit() || this.resubmitting()) return;

    this.dialogService.open(ResubmitConfirmDialogComponent, {
      header: this.i18n.instant('profileOverview.resubmitConfirm.header'),
      width: '680px',
      contentStyle: { 'max-height': '86vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      draggable: false,
      data: {
        sections: this.changedSectionsForConfirm(),
        fallbackLabels: this.changedSectionLabels()
      }
    })?.onClose.pipe(take(1)).subscribe((confirmed: boolean) => {
      if (!confirmed) return;
      this.executeResubmitProfile();
    });
  }

  private executeResubmitProfile() {
    this.resubmitting.set(true);
    this.profileService.resubmitProfile()
      .pipe(
        take(1),
        finalize(() => this.resubmitting.set(false))
      )
      .subscribe({
        next: () => {
          this.notify.success(this.i18n.instant('profileOverview.resubmit.success'));
          this.basics.reload();
          this.review.reload();
          this.changeRequests.reload();
          this.sections.forEach(section => section.reload());
          this.authState.resetBootstrap();
          this.authState.getAuthBootstrap$().subscribe();
        },
        error: () => {
          this.notify.error(this.i18n.instant('profileOverview.resubmit.failed'));
        }
      });
  }

  sectionStatus(section: ProfileSectionEnum) {
    return this.sections.get(section)?.status() ?? 'idle';
  }

  sectionValue(section: ProfileSectionEnum) {
    const value = this.sections.get(section)?.value() ?? null;
    return applyFieldChanges(value, this.changes(section));
  }

  readonly changeRequestsVm = computed(() => {
    const items = (this.changeRequests.value() as ProfileChangeRequestDto[] | undefined) ?? [];
    return items.map(item => ({
      ...item,
      statusLabelKey: this.changeStatusLabelKey(item.status),
      statusSeverity: this.changeStatusSeverity(item.status),
      sectionLabelKey: this.sectionLabelKey(item.section)
    })) as changeRequestDto[];
  });

  openEditDialog(section: ProfileSectionEnum) {
    const status = this.profileStatus();
    const mode = status === UserProfileStatusEnum.Approved
      ? 'change-request'
      : (status === UserProfileStatusEnum.RequiresUpdate || status === UserProfileStatusEnum.Submitted)
        ? 'review-edit'
        : 'create';
    this.dialogService.open(ProfileEditDialogComponent, {
      header: this.i18n.instant('profileView.editDialog.title'),
      data: { section, mode, notes: this.activeSectionNotes() },
      draggable: true,
      closable: true,
      styleClass: 'modal-dialog  modal-xl'
    })?.onClose.subscribe(result => {
      if (!result) return;
      this.reloadSection(section);
    });
  }

  sectionLabelKey(section: number): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites:
        return 'profileOverview.sections.prerequisites';
      case ProfileSectionEnum.Personal:
        return 'profileOverview.sections.personal';
      case ProfileSectionEnum.Contact:
        return 'profileOverview.sections.contact';
      case ProfileSectionEnum.Qualifications:
        return 'profileOverview.sections.qualifications';
      case ProfileSectionEnum.Experience:
        return 'profileOverview.sections.experiences';
      case ProfileSectionEnum.TrainingCourses:
        return 'profileOverview.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards:
        return 'profileOverview.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills:
        return 'profileOverview.sections.skills';
      case ProfileSectionEnum.Languages:
        return 'profileOverview.sections.languages';
      case ProfileSectionEnum.Attachments:
      default:
        return 'profileOverview.sections.attachments';
    }
  }

  noteSeverity(status: number): 'warn' | 'danger' | 'secondary' {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'warn';
    if (status === ReviewStatusEnum.Rejected) return 'danger';
    return 'secondary';
  }

  noteSeverityClass(status: number): string {
    const sev = this.noteSeverity(status);
    if (sev === 'warn' || sev === 'danger') return 'chip-warn';
    return 'chip-ok';
  }

  noteStatusLabelKey(status: number): string {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'profileOverview.reviewStatus.needsCorrection';
    if (status === ReviewStatusEnum.Rejected) return 'profileOverview.reviewStatus.rejected';
    return 'profileOverview.reviewStatus.other';
  }

  protected reviewNoteTitle(note: MyProfileReviewNoteDto): string {
    const fallbackTitle = this.translatedReviewTitle(note);

    switch (this.expanded()) {
      case ProfileSectionEnum.Qualifications:
        return this.qualificationReviewNoteTitle(note) ?? fallbackTitle;
      case ProfileSectionEnum.Experience:
        return this.experienceReviewNoteTitle(note) ?? fallbackTitle;
      case ProfileSectionEnum.TrainingCourses:
        return this.trainingReviewNoteTitle(note) ?? fallbackTitle;
      case ProfileSectionEnum.CertificatesAndAwards:
        return this.achievementReviewNoteTitle(note) ?? fallbackTitle;
      case ProfileSectionEnum.Attachments:
        return this.attachmentReviewNoteTitle(note) ?? fallbackTitle;
      default:
        return fallbackTitle;
    }
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

  protected readonly UserProfileStatusEnum = UserProfileStatusEnum;

  private sectionNotes(section: ProfileSectionEnum): MyProfileReviewNoteDto[] {
    const review = this.review.value() as MyProfileReviewSummaryDto | undefined;
    return (review?.sections ?? []).find(item => item.section === section)?.notes ?? [];
  }

  private hasSectionDataNote(section: ProfileSectionEnum): boolean {
    const notes = this.sectionNotes(section);
    const hasSectionData = notes.some(note =>
      note.targetType === ReviewTargetTypeEnum.Field &&
      note.fieldPath === this.sectionDataFieldPath
    );

    if (hasSectionData) return true;

    return false;
  }

  private hasSolvedSectionDataCorrection(section: ProfileSectionEnum): boolean {
    return this.editableItemsForSection(section).some(item =>
      item.targetType === ReviewTargetTypeEnum.Field &&
      item.fieldPath === this.sectionDataFieldPath
    );
  }

  private isMixedSection(section: ProfileSectionEnum): boolean {
    return section === ProfileSectionEnum.Prerequisites ||
      section === ProfileSectionEnum.Personal ||
      section === ProfileSectionEnum.Contact;
  }

  private actionableNotesCount(section: ProfileSectionEnum, notes: MyProfileReviewNoteDto[]): number {
    if (section === ProfileSectionEnum.Skills || section === ProfileSectionEnum.Languages) {
      return notes.filter(note => note.targetType === ReviewTargetTypeEnum.Section).length;
    }

    const detailNotes = notes.filter(note => note.targetType !== ReviewTargetTypeEnum.Section);
    return detailNotes.length;
  }

  private isSectionNoteActionable(section: ProfileSectionEnum): boolean {
    return section === ProfileSectionEnum.Skills || section === ProfileSectionEnum.Languages;
  }

  private translatedReviewTitle(note: MyProfileReviewNoteDto): string {
    const key = REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(note.title)] ??
      REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(note.fieldPath)] ??
      REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(note.entityName)] ??
      null;

    return key ? this.i18n.instant(key) : note.title;
  }

  private qualificationReviewNoteTitle(note: MyProfileReviewNoteDto): string | null {
    const profile = this.sectionValue(ProfileSectionEnum.Qualifications) as ProfileStatusDto | null;
    const qualification = this.qualificationForReviewNote(note, profile?.qualifications ?? []);
    if (!qualification) return null;

    const degree = cleanLabel(qualification.degree?.name);
    const major = cleanLabel(qualification.major?.name);
    const subMajor = cleanLabel(qualification.subMajor?.name);
    const specialization = [major, subMajor].filter(Boolean).join(' / ');
    const university = cleanLabel(qualification.university?.name);
    const year = qualification.graduationYear ? String(qualification.graduationYear) : '';
    const details = [degree, specialization, university, year].filter(Boolean);

    if (details.length) return details.join(' - ');

    return cleanLabel(qualification.attachment?.fileName) ?? null;
  }

  private qualificationForReviewNote(
    note: MyProfileReviewNoteDto,
    qualifications: QualificationDto[]
  ): QualificationDto | null {
    return this.itemForReviewNote(note, qualifications);
  }

  private experienceReviewNoteTitle(note: MyProfileReviewNoteDto): string | null {
    const profile = this.sectionValue(ProfileSectionEnum.Experience) as ProfileStatusDto | null;
    const experience = this.itemForReviewNote(note, profile?.experiences ?? []);
    if (!experience) return null;

    const details = [
      cleanLabel(experience.jobTitle),
      cleanLabel(experience.employerName),
      cleanLabel(experience.qualification?.name),
      cleanLabel(experience.country?.name)
    ].filter(Boolean);

    if (details.length) return details.join(' - ');

    return cleanLabel(experience.attachment?.fileName) ?? null;
  }

  private trainingReviewNoteTitle(note: MyProfileReviewNoteDto): string | null {
    const profile = this.sectionValue(ProfileSectionEnum.TrainingCourses) as ProfileStatusDto | null;
    const course = this.itemForReviewNote(note, profile?.trainingCourses ?? []);
    if (!course) return null;

    const details = [
      cleanLabel(course.title),
      cleanLabel(course.provider),
      cleanLabel(course.country?.name)
    ].filter(Boolean);

    if (details.length) return details.join(' - ');

    return cleanLabel(course.attachment?.fileName) ?? null;
  }

  private achievementReviewNoteTitle(note: MyProfileReviewNoteDto): string | null {
    const profile = this.sectionValue(ProfileSectionEnum.CertificatesAndAwards) as ProfileStatusDto | null;
    const achievement = this.itemForReviewNote(note, profile?.achievements ?? []);
    if (!achievement) return null;

    const details = [
      cleanLabel(achievement.title),
      cleanLabel(achievement.achievementType?.name),
      cleanLabel(achievement.issuingAuthority),
      cleanLabel(achievement.country?.name)
    ].filter(Boolean);

    if (details.length) return details.join(' - ');

    return cleanLabel(achievement.attachment?.fileName) ?? null;
  }

  private attachmentReviewNoteTitle(note: MyProfileReviewNoteDto): string | null {
    const profile = this.sectionValue(ProfileSectionEnum.Attachments) as ProfileStatusDto | null;
    const attachments = profile?.additionalAttachments ?? [];
    const entityId = note.entityId?.toLowerCase();
    if (entityId) {
      const row = attachments.find(item => item.id?.toLowerCase() === entityId);
      if (row) return this.additionalAttachmentTitle(row);
    }

    const resourceId = note.resourceId?.toLowerCase();
    if (resourceId) {
      const row = attachments.find(item => item.file?.resourceId?.toLowerCase() === resourceId);
      if (row) return this.additionalAttachmentTitle(row);
    }

    return null;
  }

  private itemForReviewNote<
    T extends { id?: string | null; attachment?: { resourceId?: string | null } | null }
  >(note: MyProfileReviewNoteDto, items: T[]): T | null {
    const entityId = note.entityId?.toLowerCase();
    if (entityId) {
      const row = items.find(item => item.id?.toLowerCase() === entityId);
      if (row) return row;
    }

    const resourceId = note.resourceId?.toLowerCase();
    if (resourceId) {
      const row = items.find(item => item.attachment?.resourceId?.toLowerCase() === resourceId);
      if (row) return row;
    }

    return null;
  }

  private additionalAttachmentTitle(item: AdditionalAttachmentDto): string | null {
    const details = [
      cleanLabel(item.title),
      cleanLabel(item.file?.fileName)
    ].filter(Boolean);

    return details.length ? details.join(' - ') : null;
  }
}

function parseJsonValue(value?: string | null) {
  if (!value) return value;
  try {
    return JSON.parse(value);
  } catch {
    return value;
  }
}

function cleanLabel(value?: string | null): string | null {
  const normalized = value?.trim();
  return normalized && normalized !== '-' ? normalized : null;
}

function normalizeReviewTitle(value?: string | null): string {
  return (value ?? '').toLowerCase().replace(/[^a-z0-9]/g, '');
}

const REVIEW_TITLE_TRANSLATION_KEYS: Record<string, string> = {
  sectiondata: 'profileOverview.resubmitConfirm.sectionData',
  resume: 'profileOverview.attachments.resume',
  cv: 'profileOverview.attachments.resume',
  resumeattachmentid: 'profileOverview.attachments.resume',
  nationalcard: 'profileOverview.attachments.nationalCard',
  nationalidcard: 'profileOverview.attachments.nationalCard',
  nationalcardid: 'profileOverview.attachments.nationalCard',
  birthcertificate: 'profileOverview.attachments.birthdayCertificate',
  birthdaycertificate: 'profileOverview.attachments.birthdayCertificate',
  birthdaycertificateid: 'profileOverview.attachments.birthdayCertificate',
  marriagecertificate: 'profileOverview.attachments.marriageCertificate',
  marriagecertificateid: 'profileOverview.attachments.marriageCertificate',
  sponsorcard: 'profileOverview.attachments.sponsorCard',
  sponsorcardid: 'profileOverview.attachments.sponsorCard',
  sponsorcardresourceid: 'profileOverview.attachments.sponsorCard',
  nationaladdresscertificate: 'profileOverview.attachments.residenceAddressCertificate',
  nationaladdresscertificateid: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddresscertificateid: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddress: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddresscertificate: 'profileOverview.attachments.residenceAddressCertificate',
  qualification: 'profileOverview.sections.qualifications',
  experience: 'profileOverview.sections.experiences',
  trainingcourse: 'profileOverview.sections.trainingCourses',
  achievement: 'profileOverview.sections.certificatesAndAwards',
  skill: 'profileOverview.sections.skills',
  language: 'profileOverview.sections.languages',
  attachment: 'profileOverview.files.attachment',
  profileadditionalattachment: 'profileOverview.files.attachment',
  additionalattachments: 'profileOverview.files.attachment',
  attachmentid: 'profileOverview.files.attachment',
  attachmentresourceid: 'profileOverview.files.attachment',
  certificateid: 'profileOverview.files.attachment',
};
