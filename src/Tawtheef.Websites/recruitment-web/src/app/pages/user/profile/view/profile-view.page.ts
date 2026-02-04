import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  ViewChild,
  computed,
  inject,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { rxResource } from '@angular/core/rxjs-interop';

import { ButtonDirective } from 'primeng/button';
import { Skeleton } from 'primeng/skeleton';
import { ProgressSpinner } from 'primeng/progressspinner';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import {
  ProfileSectionEnum,
  ProfileChangeRequestDto,
  ProfileChangeRequestStatusEnum,
  ReviewStatusEnum,
  MyProfileReviewSummaryDto,
  UserProfileStatusEnum,
  MyProfileReviewNoteDto,
  ProfileChangeActionEnum,
  ReviewTargetTypeEnum
} from './models/profile-overview.model';

import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { ProfileEditDialogComponent } from './dialogs/profile-edit-dialog/profile-edit-dialog.component';
import { ProfileStatusDto } from '../../../../core/models/auth/auth-response.model';

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
import { NotificationService } from '../../../../core/services/notification.service';

interface SectionCard {
  section: ProfileSectionEnum;
  icon: string;
  labelKey: string;
}

type AnyRxRes = ReturnType<typeof rxResource>;
type RxRes<T> = Omit<AnyRxRes, 'value'> & { value: () => T | undefined };

@Component({
  selector: 'app-profile-view-page',
  standalone: true,
  providers: [
    DialogService,
    ProfileService,
    { provide: PROFILE_WRITE_MODE, useValue: 'review-edit' },
  ],
  imports: [
    CommonModule,
    TranslatePipe,
    Skeleton,
    TooltipModule,
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
  private readonly notificationService = inject(NotificationService);
  private readonly profileService = inject(ProfileService);

  protected readonly ProfileSectionEnum = ProfileSectionEnum;
  protected readonly UserProfileStatusEnum = UserProfileStatusEnum;

  @ViewChild('sectionContent') sectionContent?: ElementRef<HTMLElement>;

  /** Optional sections must show only if: has data OR has notes OR has pending change */
  private readonly optionalSections = new Set<ProfileSectionEnum>([
    ProfileSectionEnum.TrainingCourses,
    ProfileSectionEnum.CertificatesAndAwards,
    ProfileSectionEnum.Skills,
    ProfileSectionEnum.Languages,
    ProfileSectionEnum.Attachments
  ]);

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

  protected readonly expanded = signal<ProfileSectionEnum>(ProfileSectionEnum.Personal);

  /** Resources */
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

  /** Per-section resources */
  private readonly sections = new Map<ProfileSectionEnum, RxRes<ProfileStatusDto>>();

  constructor() {
    this.cards.forEach(card => {
      this.sections.set(
        card.section,
        rxResource<ProfileStatusDto, unknown>({
          stream: () => this.profileCqrs.section(card.section),
        })
      );
    });

    // load lookups
    this.lookups.loadAll().subscribe(() => {});

    // preload first section
    this.openCard(this.expanded(), false);
  }

  /** Basic computed values */
  readonly header = computed(() => this.basics.value());
  readonly reviewSummary = computed(() => this.review.value() as MyProfileReviewSummaryDto | undefined);
  readonly profileStatus = computed(() => this.reviewSummary()?.profileStatus ?? null);

  readonly pageLoading = computed(() =>
    this.basics.status() === 'loading' ||
    this.review.status() === 'loading' ||
    this.changeRequests.status() === 'loading'
  );

  readonly reviewLoading = computed(() => this.review.status() === 'loading');
  readonly changeRequestsLoading = computed(() => this.changeRequests.status() === 'loading');
  readonly summaryLoading = computed(() => this.reviewLoading() || this.changeRequestsLoading());

  /** UI helpers */
  get keyLabel() {
    return this.cards.find(c => c.section === this.expanded())?.labelKey;
  }

  get avatar() {
    return this.header()?.avatar || AvatarUtils.build(this.header()?.fullNameEn ?? null);
  }

  /** For now disabled like your code; you can later implement properly */
  get enableChangeMode() {
    return false;
  }

  get canReplaceAttachment() {
    return this.enableChangeMode;
  }

  get canAddAttachments() {
    return this.enableChangeMode;
  }

  /** Visibility config */
  readonly visibility = computed<ProfileOverviewVisibility>(() => {
    const profile = this.basics.value();
    if (!profile) return this.emptyVisibility;
    return createProfileOverviewVisibility(profile)();
  });

  /** Review notes with a quick lookup index */
  readonly reviewNotes = computed(() => {
    const review = this.reviewSummary();
    if (!review) return undefined;

    const sectionIndex = (review.sections ?? []).reduce((acc, section) => {
      acc[section.section] = section.notesCount ?? 0;
      return acc;
    }, {} as Record<number, number>);

    return { ...review, sectionIndex } as MyProfileReviewSummaryDto & { sectionIndex: Record<number, number> };
  });

  /** Status chip VM */
  readonly statusVm = computed(() => {
    const status = this.profileStatus();

    switch (status) {
      case UserProfileStatusEnum.Approved:
        return { labelKey: 'profileOverview.status.approved', hintKey: 'profileOverview.statusHint.approved', severity: 'chip-ok' };
      case UserProfileStatusEnum.RequiresUpdate:
        return { labelKey: 'profileOverview.status.requiresUpdate', hintKey: 'profileOverview.statusHint.requiresUpdate', severity: 'chip-warn' };
      case UserProfileStatusEnum.Rejected:
        return { labelKey: 'profileOverview.status.rejected', hintKey: 'profileOverview.statusHint.rejected', severity: 'chip-warn' };
      case UserProfileStatusEnum.UnderReview:
        return { labelKey: 'profileOverview.status.underReview', hintKey: 'profileOverview.statusHint.underReview', severity: 'chip-warn' };
      case UserProfileStatusEnum.Submitted:
        return { labelKey: 'profileOverview.status.submitted', hintKey: 'profileOverview.statusHint.submitted', severity: 'chip-warn' };
      case UserProfileStatusEnum.InCreation:
        return { labelKey: 'profileOverview.status.inCreation', hintKey: 'profileOverview.statusHint.inCreation', severity: 'chip-warn' };
      case UserProfileStatusEnum.AdminCancelled:
        return { labelKey: 'profileOverview.status.adminCancelled', hintKey: 'profileOverview.statusHint.adminCancelled', severity: 'chip-warn' };
      default:
        return { labelKey: 'profileOverview.status.pending', hintKey: 'profileOverview.statusDescriptions.pending', severity: 'chip-warn' };
    }
  });

  /** Change Requests VM */
  readonly changeRequestsVm = computed(() => {
    const items = (this.changeRequests.value() as ProfileChangeRequestDto[] | undefined) ?? [];
    return items.map(item => ({
      ...item,
      statusLabelKey: this.changeStatusLabelKey(item.status),
      statusSeverity: this.changeStatusSeverity(item.status),
      sectionLabelKey: this.sectionLabelKey(item.section)
    })) as changeRequestDto[];
  });

  readonly pendingChangeRequests = computed(() => {
    return this.changeRequestsVm().filter(
      item =>
        item.status === ProfileChangeRequestStatusEnum.Pending ||
        item.status === ProfileChangeRequestStatusEnum.UnderReview
    );
  });

  /** ===== FIX: Optional section visibility must be based on SECTION resource, not basics() ===== */
  private sectionResource(section: ProfileSectionEnum) {
    return this.sections.get(section);
  }

  private sectionRawValue(section: ProfileSectionEnum): ProfileStatusDto | null {
    return (this.sectionResource(section)?.value() ?? null) as ProfileStatusDto | null;
  }

  private hasSectionData(section: ProfileSectionEnum): boolean {
    const profile = this.sectionRawValue(section);
    if (!profile) return false;

    switch (section) {
      case ProfileSectionEnum.TrainingCourses:
        return (profile.trainingCourses ?? []).length > 0;
      case ProfileSectionEnum.CertificatesAndAwards:
        return (profile.achievements ?? []).length > 0;
      case ProfileSectionEnum.Skills:
        return (profile.skills ?? []).length > 0;
      case ProfileSectionEnum.Languages:
        return (profile.languages ?? []).length > 0;
      case ProfileSectionEnum.Attachments:
        return (profile.additionalAttachments ?? []).length > 0;
      default:
        return true;
    }
  }

  /** ===== FIX: Tiles are rendered from tilesVm(), so counts + optional logic is correct ===== */
  readonly tilesVm = computed(() => {
    const review = this.reviewNotes();
    const pending = this.pendingChangeRequests();
    const active = this.expanded();

    return this.cards
      .map(card => {
        const notesCount = review?.sectionIndex?.[card.section] ?? 0;
        const hasPending = pending.some(cr => cr.section === card.section);
        const hasData = this.hasSectionData(card.section);

        const isOptional = this.optionalSections.has(card.section);
        const shouldShow = !isOptional || hasData || notesCount > 0 || hasPending;

        return {
          ...card,
          notesCount,
          hasPending,
          hasData,
          isOptional,
          shouldShow,
          isActive: active === card.section
        };
      })
      .filter(x => x.shouldShow);
  });

  /** Notes for active section */
  readonly activeSectionNotes = computed(() => {
    const active = this.expanded();
    const review = this.reviewSummary();
    const notes = review?.sections?.find(s => s.section === active)?.notes ?? [];
    return notes as MyProfileReviewNoteDto[];
  });

  readonly activeSectionReviewNotes = computed(() => {
    const notes = this.activeSectionNotes();
    return notes.filter(n => n.targetType === ReviewTargetTypeEnum.Section);
  });

  /** Section operations */
  openCard(section: ProfileSectionEnum, shouldScroll: boolean = true) {
    const res = this.sections.get(section);
    this.expanded.set(section);
    res?.reload();
    if (shouldScroll) this.scrollToSection();
  }

  reloadSection(section: ProfileSectionEnum) {
    this.sections.get(section)?.reload();
    this.review.reload();
    this.changeRequests.reload();
  }

  sectionStatus(section: ProfileSectionEnum) {
    return this.sections.get(section)?.status() ?? 'idle';
  }

  sectionValue(section: ProfileSectionEnum) {
    const value = this.sections.get(section)?.value() ?? null;
    return applyFieldChanges(value, this.changes(section));
  }

  sectionNotesCount(section: ProfileSectionEnum) {
    return this.reviewNotes()?.sectionIndex?.[section] ?? 0;
  }

  sectionNeedsUpdate(section: ProfileSectionEnum): boolean {
    const notes = this.reviewNotes()?.sections?.find(s => s.section === section)?.notes ?? [];
    return notes.some(note => note.status === ReviewStatusEnum.NeedsCorrection || note.status === ReviewStatusEnum.Rejected);
  }

  sectionHasPendingChanges(section: ProfileSectionEnum): boolean {
    return this.pendingChangeRequests().some(request => request.section === section);
  }

  /** Editing permissions */
  canEditSections(section: ProfileSectionEnum) {
    const status = this.profileStatus();

    if (this.enableChangeMode) return true;

    if (status === UserProfileStatusEnum.RequiresUpdate) {
      const sectionNotes = this.reviewSummary()?.sections?.find(item => item.section === section);
      return (sectionNotes?.notesCount ?? 0) > 0;
    }

    return false;
  }

  openEditDialog(section: ProfileSectionEnum) {
    const status = this.profileStatus();
    const mode =
      status === UserProfileStatusEnum.Approved
        ? 'change-request'
        : status === UserProfileStatusEnum.RequiresUpdate
          ? 'review-edit'
          : 'create';

    this.dialogService.open(ProfileEditDialogComponent, {
      header: this.i18n.instant('profileView.editDialog.title'),
      data: { section, mode },
      styleClass: 'modal-dialog modal-xl'
    })?.onClose.subscribe(result => {
      if (!result) return;
      this.reloadSection(section);
    });
  }

  resubmitProfile() {
    this.profileService.resubmitProfile().subscribe({
      next: () => {
        this.notificationService.success(this.i18n.instant('profileView.notifications.resubmitted'));
        this.basics.reload();
        this.review.reload();
        this.changeRequests.reload();
      },
      error: () => {
        this.notificationService.error(this.i18n.instant('profileView.notifications.resubmitFailed'));
      }
    });
  }

  /** Change Request -> Field changes */
  protected changes(section: ProfileSectionEnum) {
    if (this.profileStatus() !== UserProfileStatusEnum.Approved) return [];

    const sectionChanges = this.changeRequestsVm().filter(cr => cr.section === section);

    let resultChanges: FieldChange[] = [];

    sectionChanges.forEach(change => {
      if (change.action === ProfileChangeActionEnum.UpdateField) {
        if (change.fieldPath) {
          resultChanges.push({
            field: change.fieldPath,
            oldValue: parseJsonValue(change.oldValue),
            newValue: parseJsonValue(change.newValue)
          });
          return;
        }

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

  /** Helpers */
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

  sectionLabelKey(section: number): string {
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
      case ProfileSectionEnum.Attachments:
      default: return 'profileOverview.sections.attachments';
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

  private scrollToSection() {
    if (!this.sectionContent?.nativeElement) return;
    setTimeout(() => {
      this.sectionContent?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 0);
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
