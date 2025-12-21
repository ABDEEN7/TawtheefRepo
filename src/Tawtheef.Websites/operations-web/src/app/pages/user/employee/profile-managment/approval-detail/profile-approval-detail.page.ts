import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize, Subscription, combineLatest } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  ProfileApprovalStepperComponent,
  ProfileApprovalStepperSection,
  StepUiStatus,
} from './components/profile-approval-stepper/profile-approval-stepper.component';
import { BasicInfoSectionComponent } from './components/sections/basic-info-section/basic-info-section.component';
import { QualificationsSectionComponent } from './components/sections/qualifications-section/qualifications-section.component';
import { ExperiencesSectionComponent } from './components/sections/experiences-section/experiences-section.component';
import { TrainingSectionComponent } from './components/sections/training-section/training-section.component';
import { AttachmentsSectionComponent } from './components/sections/attachments-section/attachments-section.component';
import {Select} from 'primeng/select';
import {Textarea} from 'primeng/textarea';
import { CertificatesSectionComponent } from './components/sections/certificates-section/certificates-section.component';
import {
  ProfileApprovalDetail, ProfileApprovalSection, ReviewStatus, SectionReviewSummary
} from '../approval-list/models/profile-approval.models';
import {routes} from '../../../../../routes/routes';
import {ProfileApprovalService} from '../approval-list/services/profile-approval.service';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {ProgressBarModule} from 'primeng/progressbar';
import {CardModule} from 'primeng/card';
import {ButtonModule} from 'primeng/button';
import {AvatarModule} from 'primeng/avatar';
import {DialogService} from 'primeng/dynamicdialog';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {ContactInfoSectionComponent} from './components/sections/contact-info-section/contact-info-section.component';
import {FirstInfoSectionComponent} from './components/sections/first-info-section/first-info-section.component';
import {SkillsSectionComponent} from './components/sections/skills-section/skills-section.component';
import {LanguagesSectionComponent} from './components/sections/languages-section/languages-section.component';
import {LanguageService} from '../../../../../core/services/language.service';
import {NotificationService} from '../../../../../core/services/notification.service';
import {FaDirArrowDirective} from '../../../../../shared/directives/dir-arrow.directive';
import {ProfileStatusNumber} from '../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-profile-approval-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,
    I18nNamespaceDirective,
    ProgressSpinnerModule,
    ProgressBarModule,
    CardModule,
    ButtonModule,
    AvatarModule,
    ProfileApprovalStepperComponent,
    BasicInfoSectionComponent,
    QualificationsSectionComponent,
    ExperiencesSectionComponent,
    TrainingSectionComponent,
    CertificatesSectionComponent,
    SkillsSectionComponent,
    LanguagesSectionComponent,
    AttachmentsSectionComponent,
    Select,
    Textarea,
    ContactInfoSectionComponent,
    FirstInfoSectionComponent,
    FaDirArrowDirective,
  ],
  templateUrl: './profile-approval-detail.page.html',
  styleUrl: './profile-approval-detail.page.scss',
  providers: [DialogService],
})

export class ProfileApprovalDetailPage implements OnInit, OnDestroy {
  private fileUtils = inject(FileUtilsService);
  private api = inject(ProfileApprovalService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private notifications = inject(NotificationService);
  private language = inject(LanguageService);

  private subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;

  private readonly flowSections = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];


  protected readonly ReviewStatus = ReviewStatus;
  draftStatus: Record<number, ReviewStatus> = {};
  draftNote: Record<number, string> = {};

  finalizeSummary = '';
  finalizeNote = '';

  loadingDetail = signal(false);
  savingSection = signal<number | null>(null);

  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);

  activeSection = signal<number | null>(null);
  currentLang = signal(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  canStartReviewUi = computed(() => {
    const info = this.detail();
    if (!info) return false;
    if (info.profileStatus == null) return true;
    return info.profileStatus === ProfileStatusNumber.Submitted;
  });

  canFinalizeUi = computed(() => this.canFinalize());
  reviewStatusOptions = [
    { labelKey: 'profileApproval.status.approved', value: ReviewStatus.Approved },
    { labelKey: 'profileApproval.status.needsCorrection', value: ReviewStatus.NeedsCorrection },
  ];
  draftDirty: Record<number, boolean> = {};
  draftInitialized = signal(false);

  private markDirty(section: number) {
    this.draftDirty[section] = true;
  }
  onSectionStatusChange(section: number, status: ReviewStatus | null) {
    this.draftStatus[section] = status ?? ReviewStatus.Pending;
    this.markDirty(section);
  }

  onSectionNoteChange(section: number, note: string) {
    this.draftNote[section] = note ?? '';
    this.markDirty(section);
  }
  isSaving(section: number): boolean {
    return this.savingSection() === section;
  }

  initDraft(info: ProfileApprovalDetail) {
    for (const sec of info.sections ?? []) {
      const sectionId = sec.section;
      if (this.draftDirty[sectionId]) continue;

      const review = this.sectionReviewFor(sec);
      this.draftStatus[sectionId] = review.status ?? ReviewStatus.Pending;
      this.draftNote[sectionId] = review.note ?? '';
    }
    this.draftInitialized.set(true);
  }

  ngOnInit(): void {
    const langSub = this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.subscriptions.push(langSub);

    const sub = combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([params, query]) => {
      const profileId = params.get('profileId');
      if (!profileId) {
        this.backToList();
        return;
      }

      this.selectedProfileId.set(profileId);

      const loadKey = `${profileId}`;
      if (loadKey !== this.lastLoadedKey) {
        this.lastLoadedKey = loadKey;
        this.loadDetail();
      } else if (!this.detail()) {
        this.loadDetail();
      }
    });

    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  backToList(): void {
    this.selectedProfileId.set(null);
    this.detail.set(null);
    this.router.navigate([routes.employee.approvalProfile]);
  }

  loadDetail(): void {
    const profileId = this.selectedProfileId();
    if (!profileId) return;

    this.loadingDetail.set(true);
    this.error.set(null);

    this.api.getProfile(profileId)
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: detail => {
          const ordered = { ...detail, sections: this.sortSections(detail.sections) };
          const merged = this.mergeDetail(this.detail(), ordered);
          this.detail.set(merged);
          this.initDraft(merged);
          const current = this.activeSection();
          if (current == null || !(merged.sections ?? []).some(s => s.section === current)) {
            this.activeSection.set(this.flowSections[0] ?? null);
          }
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadDetail')),
      });
  }

  previewFile(resourceUrl: string): void {
    this.fileUtils.previewUrl(resourceUrl, '', false).then(() => {});
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:  return 'profileOverview.sections.prerequisites';
      case 2:  return 'profileOverview.sections.basicInfo';
      case 3:  return 'profileOverview.sections.contactInfo';
      case 4:  return 'profileOverview.sections.qualifications';
      case 5:  return 'profileOverview.sections.experiences';
      case 6:  return 'profileOverview.sections.training';
      case 7:  return 'profileOverview.sections.certificates';
      case 8:  return 'profileOverview.sections.skills';
      case 9:  return 'profileOverview.sections.languages';
      case 10: return 'profileOverview.sections.attachments';
      default: return 'profileOverview.sections.attachments';
    }
  }

  private normalizeSections(incoming: ProfileApprovalDetail): ProfileApprovalDetail {
    const map = new Map<number, ProfileApprovalSection>();
    (incoming.sections ?? []).forEach(s => map.set(s.section, s));

    const normalized: ProfileApprovalSection[] = this.flowSections.map(sectionId => {
      const existing = map.get(sectionId);
      const normalizedReview = this.sectionReviewFor(existing);
      const review = existing?.sectionReview ?? normalizedReview;
      const status = (review as SectionReviewSummary | null)?.status ?? normalizedReview.status;
      const note = (review as SectionReviewSummary | null)?.note ?? normalizedReview.note;
      const reviewedAtUtc = (review as SectionReviewSummary | null)?.reviewedAtUtc ?? normalizedReview.reviewedAtUtc;

      return {
        section: sectionId,
        sectionReview: review,
        status,
        note,
        reviewedAtUtc: reviewedAtUtc ?? undefined,
        items: existing?.items ?? [],
        hasAttachments: existing?.hasAttachments ?? false,
      };
    });

    return {
      ...incoming,
      profile: this.normalizeProfileData(incoming.profile),
      sections: normalized
    };
  }

  private mergeDetail(current: ProfileApprovalDetail | null, incoming: ProfileApprovalDetail): ProfileApprovalDetail {
    const normalizedIncoming = this.normalizeSections(incoming);
    if (!current) return normalizedIncoming;

    return {
      ...current,
      ...normalizedIncoming,
      profile: this.normalizeProfileData(normalizedIncoming.profile ?? current.profile),
      sections: normalizedIncoming.sections?.length ? normalizedIncoming.sections : current.sections ?? [],
    };
  }

  stepperSections(info: ProfileApprovalDetail): ProfileApprovalStepperSection[] {
    const current = this.activeSection();
    const sections = this.sortSections(info.sections);

    return sections.map(s => {
      const st = this.sectionReviewFor(s).status;

      let uiStatus: StepUiStatus = 'idle';
      if (st === ReviewStatus.Approved) uiStatus = 'done';
      else if (st === ReviewStatus.NeedsCorrection) uiStatus = 'bad';
      else if (s.section === current) uiStatus = 'progress';
      else uiStatus = 'idle';

      return {
        section: s.section,
        uiStatus,
        icon: this.sectionIcon(s.section),
        labelKey: this.sectionName(s.section),
      };
    });
  }

  orderedSections(info: ProfileApprovalDetail): ProfileApprovalSection[] {
    return this.sortSections(info.sections);
  }

  private sortSections(sections: ProfileApprovalSection[] | null | undefined): ProfileApprovalSection[] {
    if (!sections?.length) return [];
    return [...sections].sort((a, b) => a.section - b.section);
  }

  sectionIcon(section: number): string {
    switch (section) {
      case 1:  return 'pi pi-verified';
      case 2:  return 'pi pi-id-card';
      case 3:  return 'pi pi-address-book';
      case 4:  return 'pi pi-graduation-cap';
      case 5:  return 'pi pi-briefcase';
      case 6:  return 'pi pi-folder-open';
      case 7:  return 'pi pi-list';
      case 8:  return 'pi pi-star';
      case 9:  return 'pi pi-language';
      case 10: return 'pi pi-paperclip';
      default: return 'pi pi-clipboard';
    }
  }

  profileStatusLabelKey(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.InCreation:
        return 'profileApproval.status.inCreation';
      case ProfileStatusNumber.Submitted:
        return 'profileApproval.status.submitted';
      case ProfileStatusNumber.UnderReview:
        return 'profileApproval.status.underReview';
      case ProfileStatusNumber.RequiresUpdate:
        return 'profileApproval.status.requiresUpdate';
      case ProfileStatusNumber.Approved:
        return 'profileApproval.status.approved';
      case ProfileStatusNumber.Rejected:
        return 'profileApproval.status.rejected';
      case ProfileStatusNumber.Cancelled:
        return 'profileApproval.status.cancelled';
      case ProfileStatusNumber.AdminCancelled:
        return 'profileApproval.status.adminCancelled';
      default:
        return 'profileApproval.status.unknown';
    }
  }

  profileStatusClass(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.Approved:
        return 'status-success';
      case ProfileStatusNumber.Rejected:
      case ProfileStatusNumber.RequiresUpdate:
        return 'status-danger';
      case ProfileStatusNumber.UnderReview:
        return 'status-info';
      case ProfileStatusNumber.Submitted:
        return 'status-warning';
      case ProfileStatusNumber.Cancelled:
      case ProfileStatusNumber.AdminCancelled:
        return 'status-muted';
      case ProfileStatusNumber.InCreation:
        return 'status-draft';
      default:
        return 'status-unknown';
    }
  }

  profileStatusIcon(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.Approved:
        return 'pi-check-circle';
      case ProfileStatusNumber.Rejected:
        return 'pi-times-circle';
      case ProfileStatusNumber.RequiresUpdate:
        return 'pi-flag-fill';
      case ProfileStatusNumber.UnderReview:
        return 'pi-search';
      case ProfileStatusNumber.Submitted:
        return 'pi-send';
      case ProfileStatusNumber.Cancelled:
      case ProfileStatusNumber.AdminCancelled:
        return 'pi-ban';
      case ProfileStatusNumber.InCreation:
        return 'pi-pencil';
      default:
        return 'pi-question-circle';
    }
  }

  noteRequired(section: number): boolean {
    return this.draftStatus[section] === ReviewStatus.NeedsCorrection
      && (this.draftNote[section] ?? '').trim().length === 0;
  }
  onStepperChange(nextSection: number) {
    const current = this.activeSection();
    if (current != null && this.draftDirty[current]) {
      this.notifications.warn(this.translate.instant('profileApproval.detail.unsavedChangesWarning'));
    }
    this.activeSection.set(nextSection);
  }

  progressStats(info: ProfileApprovalDetail): {
    pendingSections: number;
    flaggedSections: number;
    approvedSections: number;
  } {
    const stats = { pendingSections: 0, flaggedSections: 0, approvedSections: 0 };
    const sections = info.sections ?? [];

    sections.forEach(sec => {
      const status = this.sectionReviewFor(sec).status ?? ReviewStatus.Pending;

      if (status === ReviewStatus.Approved) stats.approvedSections += 1;
      else if (status === ReviewStatus.NeedsCorrection) stats.flaggedSections += 1;
      else stats.pendingSections += 1;
    });

    return stats;
  }

  sectionProgress(info: ProfileApprovalDetail): {
    total: number;
    reviewed: number;
    pending: number;
    percent: number;
  } {
    const stats = this.progressStats(info);
    const total = stats.pendingSections + stats.flaggedSections + stats.approvedSections;
    const reviewed = total - stats.pendingSections;
    const percent = total ? Math.round((reviewed / total) * 100) : 0;

    return {
      total,
      reviewed,
      pending: stats.pendingSections,
      percent,
    };
  }

  canFinalize(): boolean {
    const info = this.detail();
    if (!info) return false;

    return (info.sections ?? []).every(s => {
      const review = this.sectionReviewFor(s);
      const st = review.status ?? ReviewStatus.Pending;
      if (st === ReviewStatus.Approved) return true;
      if (st === ReviewStatus.NeedsCorrection) return !!(review.note ?? '').trim();
      return false;
    });
  }

  startReview(): void {
    const id = this.selectedProfileId();
    if (!id) return;

    this.loadingDetail.set(true);
    this.api.startReview(id)
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('profileApproval.detail.startReviewOk'));
          this.loadDetail();
        },
        error: () => this.notifications.error(this.translate.instant('profileApproval.detail.startReviewFail')),
      });
  }

  saveSectionDecision(section: number) {
    const info = this.detail();
    if (!info) return;

    const st = this.draftStatus[section];
    const note = (this.draftNote[section] ?? '').trim();

    if (st !== ReviewStatus.Approved && st !== ReviewStatus.NeedsCorrection) return;

    if (st === ReviewStatus.NeedsCorrection && note.length === 0) {
      this.notifications.error(this.translate.instant('profileApproval.errors.notesRequired'));
      return;
    }

    this.savingSection.set(section);

    this.api.decideSection(info.userProfileId, section.toString(), { status: st, note: note || null })
      .pipe(finalize(() => this.savingSection.set(null)))
      .subscribe({
        next: () => {
          this.draftDirty[section] = false; // مهم
          this.notifications.success(this.translate.instant('profileApproval.detail.sectionSaved'));
          this.loadDetail();
        },
        error: err => {
          const msg = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.detail.sectionSaveFailed');
          this.notifications.error(msg);
        }
      });
  }

  finalize(): void {
    const summary = this.finalizeSummary.trim() || null;
    const note = this.finalizeNote.trim() || null;
    const info = this.detail();
    const id = this.selectedProfileId();
    if (!info || !id) return;

    if (!this.canFinalize()) {
      this.notifications.error(this.translate.instant('profileApproval.detail.finalizeBlocked'));
      return;
    }

    this.loadingDetail.set(true);

    this.api.finalizeProfile(id, { summary, note, exceptionalFile: null })
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('profileApproval.detail.finalizeOk'));
          this.loadDetail();
          this.finalizeSummary = '';
          this.finalizeNote = '';
        },
        error: (err) => {
          const msg = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.detail.finalizeFail');
          this.notifications.error(msg);
        }
      });
  }

  private sectionReviewFor(section?: ProfileApprovalSection | null): SectionReviewSummary {
    const rawReview = section?.sectionReview as SectionReviewSummary | null | undefined;
    const status = rawReview?.status ?? section?.status ?? ReviewStatus.Pending;
    const note = rawReview?.note ?? section?.note ?? null;
    const reviewedAtUtc =
      rawReview?.reviewedAtUtc ?? section?.reviewedAtUtc ?? null;

    return { status, note, reviewedAtUtc };
  }

  private normalizeProfileData(profile: ProfileApprovalDetail['profile']): ProfileApprovalDetail['profile'] {
    if (!profile) return profile;

    return {
      ...profile,
      qualifications: profile.qualifications ?? [],
      experiences: profile.experiences ?? [],
      trainingCourses: profile.trainingCourses ?? [],
      professionalCertificatesAndAwards: profile.professionalCertificatesAndAwards ?? [],
      skills: profile.skills ?? [],
      languages: profile.languages ?? [],
      attachments: profile.attachments ?? [],
    };
  }
}
