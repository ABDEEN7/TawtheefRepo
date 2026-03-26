import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { combineLatest, finalize, Subscription } from 'rxjs';

import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { Select } from 'primeng/select';
import { Textarea } from 'primeng/textarea';
import { DialogService } from 'primeng/dynamicdialog';

import { routes } from '../../../../../routes/routes';
import { LanguageService } from '../../../../../core/services/language.service';
import { ProfileApprovalService } from '../approval-list/services/profile-approval.service';
import {
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
  SectionReviewSummary,
} from '../approval-list/models/profile-approval.models';
import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';
import { FileUtilsService } from '../../../../../core/utils/file-utils';
import { NotificationService } from '../../../../../core/services/notification.service';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { AvatarUtils } from '../../../../../core/utils/avatar-utils';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';

import {
  FirstInfoSectionComponent
} from '../approval-detail/components/sections/first-info-section/first-info-section.component';
import {
  BasicInfoSectionComponent
} from '../approval-detail/components/sections/basic-info-section/basic-info-section.component';
import {
  ContactInfoSectionComponent
} from '../approval-detail/components/sections/contact-info-section/contact-info-section.component';
import {
  QualificationsSectionComponent
} from '../approval-detail/components/sections/qualifications-section/qualifications-section.component';
import {
  ExperiencesSectionComponent
} from '../approval-detail/components/sections/experiences-section/experiences-section.component';
import {
  TrainingSectionComponent
} from '../approval-detail/components/sections/training-section/training-section.component';
import {
  CertificatesSectionComponent
} from '../approval-detail/components/sections/certificates-section/certificates-section.component';
import { SkillsSectionComponent } from '../approval-detail/components/sections/skills-section/skills-section.component';
import {
  LanguagesSectionComponent
} from '../approval-detail/components/sections/languages-section/languages-section.component';
import {
  AttachmentsSectionComponent,
} from '../approval-detail/components/sections/attachments-section/attachments-section.component';

import { FileViewerComponent } from '../../../../../shared/components/file-viewer/file-viewer.component';
import { ReviewAction, ReviewItemsComponent } from '../approval-detail/components/review-items/review-items.component';
import {
  ItemDialogResult,
  ItemReviewDialogComponent
} from '../approval-list/dialogs/item-review-dialog/item-review-dialog';

@Component({
  selector: 'app-profile-approval-wizard-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,

    AvatarModule,
    ButtonModule,
    ProgressBarModule,
    ProgressSpinnerModule,
    ToggleSwitchModule,
    Select,
    Textarea,
    DialogModule,

    FaDirArrowDirective,
    I18nNamespaceDirective,

    FirstInfoSectionComponent,
    BasicInfoSectionComponent,
    ContactInfoSectionComponent,
    QualificationsSectionComponent,
    ExperiencesSectionComponent,
    TrainingSectionComponent,
    CertificatesSectionComponent,
    SkillsSectionComponent,
    LanguagesSectionComponent,
    AttachmentsSectionComponent,
    ReviewItemsComponent,
    FileViewerComponent
  ],
  providers: [DialogService],
  templateUrl: './profile-approval-wizard.page.html',
  styleUrls: ['./profile-approval-wizard.page.scss'],
})
export class ProfileApprovalWizardPage implements OnInit, OnDestroy {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly api = inject(ProfileApprovalService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);
  private readonly notifications = inject(NotificationService);
  private readonly language = inject(LanguageService);
  private readonly dialogService = inject(DialogService);

  protected readonly routes = routes;
  protected readonly AvatarUtils = AvatarUtils;
  protected readonly ReviewStatus = ReviewStatus;

  private readonly subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;

  private readonly flowSections = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  isChangesMode = signal(false);
  changesFocusMode = signal(true);

  draftStatus: Record<number, ReviewStatus> = {};
  draftNote: Record<number, string> = {};
  draftDirty: Record<number, boolean> = {};

  finalizeSummary = '';
  finalizeNote = '';

  finalDialogVisible = signal(false);
  private finalDialogAutoShown = false;

  loadingDetail = signal(false);
  savingSection = signal<number | null>(null);

  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);

  activeSection = signal<number | null>(null);
  selectedFile = signal<{ url: string, name: string, mimeType?: string } | null>(null);
  loadingFile = signal(false);

  currentLang = signal(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  isNeedsCorrectionDisabled = computed(() => {
    const sectionId = this.activeSection();
    if (sectionId == null) return false;

    // Target sections: Qualifications (4), Experiences (5), Training (6), Certificates (7), Attachments (10)
    const targetSections = [4, 5, 6, 7, 10];
    if (!targetSections.includes(sectionId)) return false;

    const info = this.detail();
    if (!info) return false;

    const sec = (info.sections ?? []).find(s => s.section === sectionId);
    if (!sec || !sec.items?.length) return false;

    return sec.items.every(item => item.status === ReviewStatus.Approved);
  });

  reviewStatusOptions = computed(() => [
    { labelKey: 'profileApproval.status.approved', value: ReviewStatus.Approved, disabled: false },
    {
      labelKey: 'profileApproval.status.needsCorrection',
      value: ReviewStatus.NeedsCorrection,
      disabled: this.isNeedsCorrectionDisabled()
    },
  ]);

  total = computed(() => this.orderedSections(this.detail()).length || this.flowSections.length);
  current = computed(() => {
    // number of completed sections
    const sections = this.orderedSections(this.detail());
    return sections.filter(sec => {
      const status = this.sectionReviewFor(sec).status ?? ReviewStatus.Pending;
      return status !== ReviewStatus.Pending && status !== ReviewStatus.NotReviewed;
    }).length;
  });

  progress = computed(() => {
    const sections = this.orderedSections(this.detail());
    if (!sections.length) return 0;

    const completed = sections.filter(sec => {
      const status = this.sectionReviewFor(sec).status ?? ReviewStatus.Pending;
      return status !== ReviewStatus.Pending && status !== ReviewStatus.NotReviewed;
    }).length;

    return Math.round((completed / sections.length) * 100);
  });

  canFinalizeUi = computed(() => this.canFinalize());

  ngOnInit(): void {
    this.subscriptions.push(this.language.current$.subscribe(lang => this.currentLang.set(lang)));

    const sub = combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([params]) => {
      const profileId = params.get('profileId');
      if (!profileId) {
        this.backToList();
        return;
      }

      const isChanges = this.router.url.includes('/changes');
      this.isChangesMode.set(isChanges);

      this.selectedProfileId.set(profileId);

      const loadKey = `${profileId}|${isChanges ? 'changes' : 'review'}`;
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

  trackBySection = (_: number, s: ProfileApprovalSection) => s.section;

  backToList(): void {
    this.selectedProfileId.set(null);
    this.detail.set(null);
    this.router.navigate([routes.portal.approvalProfile]);
  }

  hasUnsavedCurrent(): boolean {
    const current = this.activeSection();
    return current != null && !!this.draftDirty[current];
  }

  onStepperChange(nextSection: number) {
    const current = this.activeSection();
    if (current != null && this.draftDirty[current]) {
      this.notifications.warn(this.translate.instant('profileApproval.detail.unsavedChangesWarning'));
      return;
    }
    this.activeSection.set(nextSection);
  }

  go(step: number): void {
    this.onStepperChange(step);
  }

  next(): void {
    const sections = this.orderedSections(this.detail());
    const current = this.activeSection();
    if (!sections.length || current == null) return;

    const idx = sections.findIndex(s => s.section === current);
    const nextSection = sections[Math.min(idx + 1, sections.length - 1)]?.section;
    if (nextSection != null && nextSection !== current) this.onStepperChange(nextSection);
  }

  prev(): void {
    const sections = this.orderedSections(this.detail());
    const current = this.activeSection();
    if (!sections.length || current == null) return;

    const idx = sections.findIndex(s => s.section === current);
    const prevSection = sections[Math.max(idx - 1, 0)]?.section;
    if (prevSection != null && prevSection !== current) this.onStepperChange(prevSection);
  }

  canGoTo(target: number): boolean {
    return this.orderedSections(this.detail()).some(s => s.section === target);
  }

  sectionPosition(section: number): number {
    const sections = this.orderedSections(this.detail());
    const idx = sections.findIndex(s => s.section === section);
    return idx >= 0 ? idx + 1 : 1;
  }

  loadDetail(): void {
    const profileId = this.selectedProfileId();
    if (!profileId) return;

    this.loadingDetail.set(true);
    this.error.set(null);

    const request$ = this.isChangesMode()
      ? this.api.getProfileChanges(profileId)
      : this.api.getProfile(profileId);

    request$
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: detail => {
          const ordered = { ...detail, sections: this.sortSections(detail.sections) };
          const merged = this.mergeDetail(this.detail(), ordered);

          this.detail.set(merged);
          this.initDraft(merged);

          const current = this.activeSection();
          if (current == null || !(merged.sections ?? []).some(s => s.section === current)) {
            this.activeSection.set((merged.sections ?? [])[0]?.section ?? null);
          }

          this.ensureFinalizeDialog();
        }
      });
  }

  toggleChangesFocusMode(): void {
    if (!this.isChangesMode()) return;

    this.changesFocusMode.set(!this.changesFocusMode());

    const current = this.detail();
    if (!current) return;

    const normalized = this.normalizeSections(current);
    this.detail.set(normalized);

    const active = this.activeSection();
    if (active == null || !(normalized.sections ?? []).some(s => s.section === active)) {
      this.activeSection.set((normalized.sections ?? [])[0]?.section ?? null);
    }
  }

  async previewFile(event: string | { url: string; fileName: string }): Promise<void> {
    const resourceUrl = typeof event === 'string' ? event : event.url;
    const name = typeof event === 'string' 
      ? (resourceUrl.split('/').pop()?.split('?')[0] || 'file') 
      : event.fileName;
    
    this.loadingFile.set(true);

    try {
      const { blobUrl, mimeType } = await this.fileUtils.getBlobUrl(resourceUrl);
      // Revoke old blob if exists
      const current = this.selectedFile();
      if (current?.url && current.url.startsWith('blob:')) {
        URL.revokeObjectURL(current.url);
      }
      this.selectedFile.set({ url: blobUrl, name, mimeType });
    } catch (e) {
      this.selectedFile.set({ url: resourceUrl, name });
    } finally {
      this.loadingFile.set(false);
    }
  }

  closeFileViewer(): void {
    const current = this.selectedFile();
    if (current?.url && current.url.startsWith('blob:')) {
      URL.revokeObjectURL(current.url);
    }
    this.selectedFile.set(null);
  }

  onInlineReview(event: { reviewItemId: string; status: ReviewStatus; note?: string | null; specializationRelation?: number | null }): void {
    if (event.status === ReviewStatus.Approved) {
      this.submitReviewItem(event.reviewItemId, event.status, null, event.specializationRelation);
      return;
    }

    this.promptCorrection(event.reviewItemId, event.note ?? null);
  }

  onReviewItemAction(event: { item: ProfileApprovalItem; action: ReviewAction }): void {
    const { item, action } = event;

    if (action === 'approve') {
      this.submitReviewItem(item.reviewItemId, ReviewStatus.Approved, null);
      return;
    }

    this.promptCorrection(item.reviewItemId, item.note ?? null, item);
  }

  private submitReviewItem(reviewItemId: string, status: ReviewStatus, note: string | null, specializationRelation?: number | null): void {
    this.api.decideReviewItem(reviewItemId, { status, note, specializationRelation }).subscribe({
      next: () => {
        this.notifications.success(this.translate.instant('profileApproval.detail.sectionSaved'));
        this.loadDetail();
      }
    });
  }

  private promptCorrection(reviewItemId: string, note?: string | null, item?: ProfileApprovalItem | null): void {
    const resolvedItem = item ?? this.findReviewItem(reviewItemId) ?? this.buildPlaceholderItem(reviewItemId);

    const ref = this.dialogService.open(ItemReviewDialogComponent, {
      header: this.translate.instant('profileApproval.dialog.title'),
      data: { item: resolvedItem, action: 'changes', note: note ?? resolvedItem.note },
      width: '520px',
      modal: true,
      draggable: false,   // ✅ disables dragging
      dismissableMask: false
    });

    const sub = ref?.onClose.subscribe((result: ItemDialogResult | undefined) => {
      if (!result) return;

      const nextNote = (result.note ?? '').trim() || null;
      this.submitReviewItem(reviewItemId, ReviewStatus.ChangesRequested, nextNote);
    });

    if (sub) this.subscriptions.push(sub);
  }

  onSectionStatusChange(section: number, status: ReviewStatus | null) {
    if (status === ReviewStatus.Approved && this.sectionHasCorrections(section)) {
      this.draftStatus[section] = ReviewStatus.NeedsCorrection;
      this.markDirty(section);
      return;
    }

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

  sectionIsAutoApprovedEmpty(section: number): boolean {
    const sec = (this.detail()?.sections ?? []).find(s => s.section === section);
    if (!sec) return false;

    const review = this.sectionReviewFor(sec);
    return (sec.items?.length ?? 0) === 0 && review.status === ReviewStatus.Approved;
  }

  sectionHasUndecidedItems(section: number): boolean {
    const sec = (this.detail()?.sections ?? []).find(s => s.section === section);
    return (sec?.items ?? []).some(item =>
      item.status === ReviewStatus.Pending || item.status === ReviewStatus.NotReviewed,
    );
  }

  sectionHasCorrections(section: number): boolean {
    const sec = (this.detail()?.sections ?? []).find(s => s.section === section);
    return (sec?.items ?? []).some(item => item.status === ReviewStatus.NeedsCorrection);
  }

  sectionApprovalBlocked(section: number): boolean {
    const st = this.draftStatus[section];

    // Block Approved if there are corrections
    if (st === ReviewStatus.Approved && this.sectionHasCorrections(section)) return true;

    // Block Needs Correction if all items are approved (for target sections)
    if (st === ReviewStatus.NeedsCorrection && section === this.activeSection() && this.isNeedsCorrectionDisabled()) return true;

    return false;
  }

  private markDirty(section: number) {
    this.draftDirty[section] = true;
  }

  resetDraft(section: number): void {
    const info = this.detail();
    if (!info) return;

    const sec = (info.sections ?? []).find(s => s.section === section);
    const review = this.sectionReviewFor(sec);

    this.draftStatus[section] = review.status ?? ReviewStatus.Pending;
    this.draftNote[section] = review.note ?? '';
    this.draftDirty[section] = false;
  }

  initDraft(info: ProfileApprovalDetail) {
    for (const sec of info.sections ?? []) {
      const sectionId = sec.section;
      if (this.draftDirty[sectionId]) continue;

      const review = this.sectionReviewFor(sec);
      this.draftStatus[sectionId] = review.status ?? ReviewStatus.Pending;
      this.draftNote[sectionId] = review.note ?? '';
    }
  }

  saveSectionDecision(section: number) {
    const info = this.detail();
    if (!info) return;

    const st = this.draftStatus[section];
    const note = (this.draftNote[section] ?? '').trim();

    if (this.sectionIsAutoApprovedEmpty(section)) {
      this.notifications.success(
        this.translate.instant('profileApproval.detail.sectionApproval.autoApprovedLocked'),
      );
      return;
    }

    if (st !== ReviewStatus.Approved && st !== ReviewStatus.NeedsCorrection) return;

    if (this.sectionHasUndecidedItems(section)) {
      this.notifications.error(
        this.translate.instant('profileApproval.detail.sectionApproval.pendingItemsBlock'),
      );
      return;
    }

    if (st === ReviewStatus.Approved && this.sectionHasCorrections(section)) {
      this.notifications.error(
        this.translate.instant('profileApproval.detail.sectionApproval.correctionBlock'),
      );
      this.draftStatus[section] = ReviewStatus.NeedsCorrection;
      this.markDirty(section);
      return;
    }

    if (st === ReviewStatus.NeedsCorrection && note.length === 0) {
      this.notifications.error(this.translate.instant('profileApproval.errors.notesRequired'));
      return;
    }

    this.savingSection.set(section);

    this.api
      .decideSection(info.userProfileId, section.toString(), { status: st, note: note || null })
      .pipe(finalize(() => this.savingSection.set(null)))
      .subscribe({
        next: () => {
          this.draftDirty[section] = false;
          this.notifications.success(this.translate.instant('profileApproval.detail.sectionSaved'));
          this.loadDetail();

          //move to next section
          const current = this.activeSection();
          if (current === section) {
            this.next();
          }
        }
      });
  }

  noteRequired(section: number): boolean {
    return (
      this.draftStatus[section] === ReviewStatus.NeedsCorrection &&
      (this.draftNote[section] ?? '').trim().length === 0
    );
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

    this.api
      .finalizeProfile(id, { summary, note, exceptionalFile: null })
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('profileApproval.detail.finalizeOk'));
          this.closeFinalizeDialog();
          // back to prev page
          this.backToList();
        }
      });
  }

  openFinalizeDialog(): void {
    if (!this.canFinalize()) {
      this.notifications.error(this.translate.instant('profileApproval.detail.finalizeBlocked'));
      return;
    }

    this.finalDialogVisible.set(true);
  }

  closeFinalizeDialog(): void {
    this.finalDialogVisible.set(false);
  }

  stepperSections(info: ProfileApprovalDetail): ProfileApprovalSection[] {
    return this.sortSections(info.sections);
  }

  orderedSections(info: ProfileApprovalDetail | null): ProfileApprovalSection[] {
    return this.sortSections(info?.sections);
  }

  private sortSections(sections: ProfileApprovalSection[] | null | undefined): ProfileApprovalSection[] {
    if (!sections?.length) return [];
    return [...sections].sort((a, b) => a.section - b.section);
  }

  private normalizeSections(incoming: ProfileApprovalDetail): ProfileApprovalDetail {
    const sections = incoming.sections ?? [];

    if (!this.isChangesMode()) {
      const map = new Map<number, ProfileApprovalSection>();
      sections.forEach(s => map.set(s.section, s));

      const normalized: ProfileApprovalSection[] = this.flowSections.map(sectionId => {
        const existing = map.get(sectionId);
        const normalizedReview = this.sectionReviewFor(existing);

        const review = existing?.sectionReview ?? normalizedReview;
        const status = (review as SectionReviewSummary | null)?.status ?? normalizedReview.status;
        const note = (review as SectionReviewSummary | null)?.note ?? normalizedReview.note;
        const reviewedAtUtc =
          (review as SectionReviewSummary | null)?.reviewedAtUtc ?? normalizedReview.reviewedAtUtc;

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
        sections: normalized,
      };
    }

    if (this.changesFocusMode()) {
      const focused = sections
        .map(sec => ({
          ...sec,
          items: sec.items ?? [],
          hasAttachments: sec.hasAttachments ?? false,
        }))
        .filter(sec => (sec.items?.length ?? 0) > 0)
        .sort((a, b) => a.section - b.section);

      return {
        ...incoming,
        profile: this.normalizeProfileData(incoming.profile),
        sections: focused,
      };
    }

    const map = new Map<number, ProfileApprovalSection>();
    sections.forEach(s => map.set(s.section, s));

    const expanded: ProfileApprovalSection[] = this.flowSections.map(sectionId => {
      const existing = map.get(sectionId);
      if (!existing) {
        return { section: sectionId, status: ReviewStatus.Approved, items: [], hasAttachments: false };
      }

      return {
        ...existing,
        items: existing.items ?? [],
        hasAttachments: existing.hasAttachments ?? false,
      };
    });

    return {
      ...incoming,
      profile: this.normalizeProfileData(incoming.profile),
      sections: expanded,
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

  sectionName(section: number): string {
    switch (section) {
      case 1: return 'profileOverview.sections.prerequisites';
      case 2: return 'profileOverview.sections.basicInfo';
      case 3: return 'profileOverview.sections.contactInfo';
      case 4: return 'profileOverview.sections.qualifications';
      case 5: return 'profileOverview.sections.experiences';
      case 6: return 'profileOverview.sections.training';
      case 7: return 'profileOverview.sections.certificates';
      case 8: return 'profileOverview.sections.skills';
      case 9: return 'profileOverview.sections.languages';
      case 10: return 'profileOverview.sections.attachments';
      default: return 'profileOverview.sections.attachments';
    }
  }

  sectionIcon(section: number): string {
    switch (section) {
      case 1: return 'pi pi-verified';
      case 2: return 'pi pi-id-card';
      case 3: return 'pi pi-address-book';
      case 4: return 'pi pi-graduation-cap';
      case 5: return 'pi pi-briefcase';
      case 6: return 'pi pi-folder-open';
      case 7: return 'pi pi-list';
      case 8: return 'pi pi-star';
      case 9: return 'pi pi-language';
      case 10: return 'pi pi-paperclip';
      default: return 'pi pi-clipboard';
    }
  }

  progressStats(info: ProfileApprovalDetail): {
    pendingSections: number;
    flaggedSections: number;
    approvedSections: number;
  } {
    const stats = { pendingSections: 0, flaggedSections: 0, approvedSections: 0 };
    (info.sections ?? []).forEach(sec => {
      const status = this.sectionReviewFor(sec).status ?? ReviewStatus.Pending;
      if (status === ReviewStatus.NeedsCorrection) stats.flaggedSections++;
      else if (status === ReviewStatus.Approved) stats.approvedSections++;
      else stats.pendingSections++;
    });
    return stats;
  }

  profileStatusLabelKey(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.InCreation: return 'profileApproval.status.inCreation';
      case ProfileStatusNumber.Submitted: return 'profileApproval.status.submitted';
      case ProfileStatusNumber.UnderReview: return 'profileApproval.status.underReview';
      case ProfileStatusNumber.RequiresUpdate: return 'profileApproval.status.requiresUpdate';
      case ProfileStatusNumber.Approved: return 'profileApproval.status.approved';
      case ProfileStatusNumber.Rejected: return 'profileApproval.status.rejected';
      case ProfileStatusNumber.Cancelled: return 'profileApproval.status.cancelled';
      case ProfileStatusNumber.AdminCancelled: return 'profileApproval.status.adminCancelled';
      default: return 'profileApproval.status.unknown';
    }
  }

  profileStatusClass(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.Approved: return 'status-success';
      case ProfileStatusNumber.Rejected:
      case ProfileStatusNumber.RequiresUpdate: return 'status-danger';
      case ProfileStatusNumber.UnderReview: return 'status-info';
      case ProfileStatusNumber.Submitted: return 'status-warning';
      case ProfileStatusNumber.Cancelled:
      case ProfileStatusNumber.AdminCancelled: return 'status-muted';
      case ProfileStatusNumber.InCreation: return 'status-draft';
      default: return 'status-unknown';
    }
  }

  profileStatusIcon(status?: ProfileStatusNumber | null): string {
    switch (status) {
      case ProfileStatusNumber.Approved: return 'pi-check-circle';
      case ProfileStatusNumber.Rejected: return 'pi-times-circle';
      case ProfileStatusNumber.RequiresUpdate: return 'pi-flag-fill';
      case ProfileStatusNumber.UnderReview: return 'pi-search';
      case ProfileStatusNumber.Submitted: return 'pi-send';
      case ProfileStatusNumber.Cancelled:
      case ProfileStatusNumber.AdminCancelled: return 'pi-ban';
      case ProfileStatusNumber.InCreation: return 'pi-pencil';
      default: return 'pi-question-circle';
    }
  }

  sectionReviewFor(section?: ProfileApprovalSection | null): SectionReviewSummary {
    const rawReview = section?.sectionReview as SectionReviewSummary | null | undefined;
    const status = rawReview?.status ?? section?.status ?? ReviewStatus.Pending;
    const note = rawReview?.note ?? section?.note ?? null;
    const reviewedAtUtc = rawReview?.reviewedAtUtc ?? section?.reviewedAtUtc ?? null;
    return { status, note, reviewedAtUtc };
  }

  private ensureFinalizeDialog(): void {
    if (this.isChangesMode()) {
      this.closeFinalizeDialog();
      this.finalDialogAutoShown = false;
      return;
    }

    if (this.canFinalize()) {
      if (!this.finalDialogAutoShown) {
        this.finalDialogVisible.set(true);
        this.finalDialogAutoShown = true;
      }
    } else {
      this.closeFinalizeDialog();
      this.finalDialogAutoShown = false;
    }
  }

  private findReviewItem(reviewItemId: string): ProfileApprovalItem | null {
    const sections = this.detail()?.sections ?? [];
    for (const sec of sections) {
      const match = (sec.items ?? []).find(item => item.reviewItemId === reviewItemId);
      if (match) return match;
    }
    return null;
  }

  private buildPlaceholderItem(reviewItemId: string): ProfileApprovalItem {
    return {
      reviewItemId,
      status: ReviewStatus.Pending,
      targetType: ReviewTargetType.Row,
      title: '',
      version: 0,
    };
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
