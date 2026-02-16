import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnDestroy, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import {combineLatest, finalize, Subscription} from 'rxjs';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {
  ProfileApprovalStepperComponent,
  ProfileApprovalStepperSection,
  StepUiStatus,
} from './components/profile-approval-stepper/profile-approval-stepper.component';
import {
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
  SectionReviewSummary
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
import {LanguageService} from '../../../../../core/services/language.service';
import {NotificationService} from '../../../../../core/services/notification.service';
import {FaDirArrowDirective} from '../../../../../shared/directives/dir-arrow.directive';
import {ReviewAction, ReviewItemsComponent} from './components/review-items/review-items.component';
import {
  ItemDialogResult,
  ItemReviewDialogComponent,
} from '../approval-list/dialogs/item-review-dialog/item-review-dialog';
import {AvatarUtils} from '../../../../../core/utils/avatar-utils';

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
    FaDirArrowDirective,
    ReviewItemsComponent,
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
  private dialogService = inject(DialogService);

  private subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;

  private readonly flowSections = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  isChangesMode = signal(false);
  changesFocusMode = signal(true);

  protected readonly ReviewStatus = ReviewStatus;
  draftStatus: Record<number, ReviewStatus> = {};
  draftNote: Record<number, string> = {};

  loadingDetail = signal(false);

  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);

  activeSection = signal<number | null>(null);
  currentLang = signal(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  draftDirty: Record<number, boolean> = {};
  draftInitialized = signal(false);
  imageError = false;

  handleImageError() {
    this.imageError = true;
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
            const first = (merged.sections ?? [])[0]?.section ?? null;
            this.activeSection.set(first);
          }
        }
      });
  }


  previewFile(resourceUrl: string): void {
    this.fileUtils.previewUrl(resourceUrl, '', false).then(() => {});
  }

  onReviewItemAction(event: { item: ProfileApprovalItem; action: ReviewAction }): void {
    const item = event.item;
    const action = event.action;

    if (action === 'approve') {
      this.submitReviewItem(item.reviewItemId, ReviewStatus.Approved, null);
      return;
    }

    this.promptCorrection(item.reviewItemId, item.note ?? null, item);
  }

  private submitReviewItem(reviewItemId: string, status: ReviewStatus, note: string | null): void {
    this.api.decideReviewItem(reviewItemId, { status, note }).subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('profileApproval.detail.sectionSaved'));
          this.loadDetail();
        }
      });
  }

  private promptCorrection(reviewItemId: string, note?: string | null, item?: ProfileApprovalItem | null): void {
    const resolvedItem = item ?? this.findReviewItem(reviewItemId) ?? this.buildPlaceholderItem(reviewItemId);
    this.dialogService.open(ItemReviewDialogComponent, {
      header: this.translate.instant('profileApproval.dialog.title'),
      data: { item: resolvedItem, action: 'changes', note: note ?? resolvedItem.note },
      width: '520px',
      modal: true,
      draggable: false,   // ✅ disables dragging
      dismissableMask: false
    })?.onClose.subscribe((result: ItemDialogResult | undefined) => {
      if (!result) return;

      const nextNote = (result.note ?? '').trim() || null;
      const status = result.action === 'reject' ? ReviewStatus.Rejected : ReviewStatus.NeedsCorrection;
      this.submitReviewItem(reviewItemId, status, nextNote);
    });
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
    const sections = incoming.sections ?? [];

    // Full review: always show all flow sections.
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

    // Changes review: focus mode shows only sections with requested changes.
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

    // Changes review: full view shows all sections (including those without changes).
    const map = new Map<number, ProfileApprovalSection>();
    sections.forEach(s => map.set(s.section, s));

    const expanded: ProfileApprovalSection[] = this.flowSections.map(sectionId => {
      const existing = map.get(sectionId);
      if (!existing) {
        return {
          section: sectionId,
          status: ReviewStatus.Approved,
          items: [],
          hasAttachments: false,
        };
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

  onStepperChange(nextSection: number) {
    const current = this.activeSection();
    if (current != null && this.draftDirty[current]) {
      this.notifications.warn(this.translate.instant('profileApproval.detail.unsavedChangesWarning'));
    }
    this.activeSection.set(nextSection);
  }

  private sectionReviewFor(section?: ProfileApprovalSection | null): SectionReviewSummary {
    const rawReview = section?.sectionReview as SectionReviewSummary | null | undefined;
    const status = rawReview?.status ?? section?.status ?? ReviewStatus.Pending;
    const note = rawReview?.note ?? section?.note ?? null;
    const reviewedAtUtc =
      rawReview?.reviewedAtUtc ?? section?.reviewedAtUtc ?? null;

    return { status, note, reviewedAtUtc };
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

  protected readonly AvatarUtils = AvatarUtils;
}
