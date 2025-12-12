import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize, Subscription, combineLatest } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ProfileApprovalService } from './services/profile-approval.service';
import {
  FinalApprovalAction,
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
} from './models/profile-approval.models';
import { RadioButton } from 'primeng/radiobutton';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ItemDialogResult, ItemReviewDialogComponent } from './dialogs/item-review-dialog/item-review-dialog';
import { SectionDialogResult, SectionReviewDialogComponent } from './dialogs/section-review-dialog/section-review-dialog';
import { FinalActionConfirmDialogComponent } from './dialogs/final-action-confirm-dialog/final-action-confirm-dialog';
import { FileUtilsService } from '../../../core/utils/file-utils';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../routes/routes';

type ContentTab = 'profile' | 'review' | 'final';

type TabState = Record<ContentTab, { loading: boolean; loaded: boolean }>;

@Component({
  selector: 'app-profile-approval-detail-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule, I18nNamespaceDirective, RadioButton, ProgressSpinnerModule],
  templateUrl: './profile-approval-detail.page.html',
  styleUrl: './profile-approval.page.scss',
  providers: [DialogService, MessageService],
})
export class ProfileApprovalDetailPage implements OnInit, OnDestroy {
  private fileUtils = inject(FileUtilsService);
  private api = inject(ProfileApprovalService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private dialogService = inject(DialogService);
  private messages = inject(MessageService);

  private subscriptions: Subscription[] = [];
  private lastLoadedKey: string | null = null;

  loadingDetail = signal(false);
  loadingFinalAction = signal(false);
  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);
  partialMode = signal(false);
  activeContentTab = signal<ContentTab>('profile');
  tabState = signal<TabState>(this.createTabState());
  activeSection = signal<number | null>(null);
  activeFinalAction = signal<FinalApprovalAction | null>(null);
  finalSummary = signal('');
  finalActionNote = signal('');
  finalAttachment: File | null = null;

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  ngOnInit(): void {
    const sub = combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([params, query]) => {
      const profileId = params.get('profileId');
      const partial = this.parsePartialFlag(query.get('changes'));
      this.partialMode.set(partial);

      if (!profileId) {
        this.backToList();
        return;
      }

      this.selectedProfileId.set(profileId);
      const loadKey = `${profileId}-${partial}`;
      if (loadKey !== this.lastLoadedKey) {
        this.lastLoadedKey = loadKey;
        this.resetTabState();
        this.loadTabData(this.activeContentTab());
      } else if (!this.detail()) {
        this.resetTabState();
        this.loadTabData(this.activeContentTab());
      }
    });

    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  backToList(): void {
    this.partialMode.set(false);
    this.selectedProfileId.set(null);
    this.detail.set(null);
    this.router.navigate([routes.employee.approvalProfile]);
  }

  refreshActiveTab(): void {
    this.reloadCurrentProfile();
  }

  loadTabData(tab: ContentTab, forceReload = false): void {
    const profileId = this.selectedProfileId();
    this.activeContentTab.set(tab);

    if (!profileId) return;

    if (!forceReload && this.tabState()[tab].loaded && this.detail()) {
      return;
    }

    this.loadingDetail.set(true);
    this.error.set(null);
    this.setTabState(tab, { loading: true, loaded: false });
    const requestOptions = this.tabRequestOptions(tab);
    const loader = this.partialMode()
      ? this.api.getProfileChanges(profileId, requestOptions)
      : this.api.getProfile(profileId, requestOptions);

    loader
      .pipe(
        finalize(() => {
          this.loadingDetail.set(false);
          this.setTabState(tab, { loading: false });
        })
      )
      .subscribe({
        next: detail => {
          const merged = this.mergeDetail(this.detail(), detail);
          this.detail.set(merged);
          const firstSection = merged.sections[0]?.section ?? null;
          if (firstSection !== null) this.activeSection.set(firstSection);
          this.setTabState(tab, { loaded: true });
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadDetail')),
      });
  }

  openItemDialog(item: ProfileApprovalItem, action: 'approve' | 'reject' | 'changes'): void {
    this.dialogService.open(ItemReviewDialogComponent, {
      header: this.translate.instant('profileApproval.dialog.title'),
      width: '420px',
      data: { item, action },
    })?.onClose.subscribe((result?: ItemDialogResult) => {
      if (!result) return;

      if (result.action === 'approve') {
        this.updateItem(item.reviewItemId, ReviewStatus.Approved);
      } else if (result.action === 'reject') {
        this.updateItem(item.reviewItemId, ReviewStatus.Rejected, result.note);
      } else {
        this.updateItem(item.reviewItemId, ReviewStatus.ChangesRequested, result.note);
      }
    });
  }

  openSectionDialog(section: ProfileApprovalSection, action: 'section-approve' | 'section-changes'): void {
    this.dialogService.open(SectionReviewDialogComponent, {
      header: this.translate.instant('profileApproval.section.dialogTitle'),
      width: '480px',
      data: {
        section,
        action,
        sectionLabelKey: this.sectionName(section.section),
        initialNote: section.sectionReview?.note ?? '',
      },
    })?.onClose.subscribe((result?: SectionDialogResult) => {
      if (!result || !section.sectionReview) return;

      if (result.action === 'section-approve') {
        this.updateItem(section.sectionReview.reviewItemId, ReviewStatus.Approved, result.note);
      } else {
        this.updateItem(section.sectionReview.reviewItemId, ReviewStatus.ChangesRequested, result.note);
      }
    });
  }

  canApproveAttachment(section: ProfileApprovalSection, item: ProfileApprovalItem): boolean {
    if (item.targetType !== ReviewTargetType.Attachment) return true;
    return section.sectionReview?.status !== ReviewStatus.Rejected;
  }

  sectionCanBeApproved(section: ProfileApprovalSection): boolean {
    if (!section.sectionReview) return false;

    if (section.items.length === 0) return true;

    const unresolvedItems = section.items.some(
      item => item.status === ReviewStatus.Pending || item.status === ReviewStatus.ChangesRequested || item.status === ReviewStatus.Rejected
    );

    const hasUnapprovedAttachments = section.items
      .filter(i => i.targetType === ReviewTargetType.Attachment)
      .some(i => i.status !== ReviewStatus.Approved);

    return !unresolvedItems && !hasUnapprovedAttachments;
  }

  statusClass(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'pill soft';
      case ReviewStatus.Rejected:
        return 'pill danger';
      case ReviewStatus.ChangesRequested:
        return 'pill warning';
      default:
        return 'pill';
    }
  }

  statusLabel(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'profileApproval.status.approved';
      case ReviewStatus.Rejected:
        return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested:
        return 'profileApproval.status.changes';
      case ReviewStatus.Pending:
      default:
        return 'profileApproval.status.pending';
    }
  }

  previewFile(resourceUrl: string): void {
    this.fileUtils.previewUrl(resourceUrl, '', false).then(() => {});
  }

  detailStatus(): ReviewStatus {
    const info = this.detail();
    if (!info) return ReviewStatus.Pending;

    const statuses = info.sections
      .map(s => s.sectionReview?.status)
      .filter((s): s is ReviewStatus => s !== undefined && s !== null);

    if (statuses.some(s => s === ReviewStatus.Rejected)) return ReviewStatus.Rejected;
    if (statuses.some(s => s === ReviewStatus.ChangesRequested)) return ReviewStatus.ChangesRequested;
    if (statuses.length && statuses.every(s => s === ReviewStatus.Approved)) return ReviewStatus.Approved;

    return ReviewStatus.Pending;
  }

  tabHasPending(section: ProfileApprovalSection): boolean {
    const pendingItem = section.items.some(i => i.status === ReviewStatus.Pending || i.status === ReviewStatus.ChangesRequested || i.status === ReviewStatus.Rejected);
    return pendingItem || (section.sectionReview?.status === ReviewStatus.ChangesRequested || section.sectionReview?.status === ReviewStatus.Rejected || section.sectionReview?.status === ReviewStatus.Pending);
  }

  sectionHasUnresolvedItems(section: ProfileApprovalSection): boolean {
    return section.items.some(i => i.status !== ReviewStatus.Approved);
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileApproval.sections.basicInfo';
      case 2:
        return 'profileApproval.sections.qualifications';
      case 3:
        return 'profileApproval.sections.experiences';
      case 4:
        return 'profileApproval.sections.training';
      case 5:
        return 'profileApproval.sections.certificates';
      case 6:
        return 'profileApproval.sections.skillsLanguages';
      case 7:
        return 'profileApproval.sections.attachments';
      case 8:
        return 'profileApproval.sections.profilePhoto';
      default:
        return 'profileApproval.sections.generic';
    }
  }

  setFinalAction(action: string): void {
    this.activeFinalAction.set(action as FinalApprovalAction);
    this.finalActionNote.set('');
    this.finalAttachment = null;
  }

  confirmFinalAction(): void {
    const action = this.activeFinalAction()!;

    if (action === 'ApproveProfile' && !this.canApproveProfile()) {
      this.messages.add({
        severity: 'warn',
        summary: this.translate.instant('profileApproval.validation.pendingSectionsSummary'),
        detail: this.translate.instant('profileApproval.validation.pendingSectionsDetail'),
      });
      return;
    }

    this.dialogService.open(FinalActionConfirmDialogComponent, {
      header: this.translate.instant('profileApproval.final.confirmTitle'),
      width: '480px',
      data: {
        message: this.translate.instant('profileApproval.final.confirmMessage', {
          action: this.translate.instant('profileApproval.final.actions.' + action),
        }),
      },
    })?.onClose.subscribe((confirmed?: boolean) => {
      if (confirmed) {
        this.executeFinalAction();
      }
    });
  }

  executeFinalAction(): void {
    if (!this.detail() || !this.activeFinalAction()) return;

    const correctionTargets = this.collectNeedsCorrectionTargets();
    const form = new FormData();
    form.append('action', this.activeFinalAction()!);
    if (this.finalActionNote()) form.append('notes', this.finalActionNote());
    if (this.finalSummary()) form.append('summary', this.finalSummary());

    if (this.activeFinalAction() === 'NeedsCorrection') {
      correctionTargets.forEach(id => form.append('needsCorrectionItems', id));
    }

    if (this.activeFinalAction() === 'RejectProfile' && this.finalAttachment) {
      form.append('rejectionDocument', this.finalAttachment);
    }

    if (this.activeFinalAction() === 'ExceptionalApproval' && this.finalAttachment) {
      form.append('exceptionalFile', this.finalAttachment);
    }

    this.loadingFinalAction.set(true);
    this.api
      .finalizeProfile(this.detail()!.userProfileId, form)
      .pipe(finalize(() => this.loadingFinalAction.set(false)))
      .subscribe({
        next: () => {
          this.activeFinalAction.set(null);
          this.finalActionNote.set('');
          this.finalAttachment = null;
          this.reloadCurrentProfile('final');
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.finalize')),
      });
  }

  private collectNeedsCorrectionTargets(): string[] {
    const detail = this.detail();
    if (!detail) return [];

    const targets = new Set<string>();
    detail.sections.forEach(section => {
      if (section.sectionReview && section.sectionReview.status === ReviewStatus.ChangesRequested)
        targets.add(section.sectionReview.reviewItemId);

      section.items
        .filter(item => item.status === ReviewStatus.ChangesRequested)
        .forEach(item => targets.add(item.reviewItemId));
    });

    return Array.from(targets);
  }

  private parsePartialFlag(value: string | null): boolean {
    if (!value) return false;
    return value === '1' || value.toLowerCase() === 'true';
  }

  canApproveProfile(): boolean {
    const info = this.detail();
    if (!info) return false;
    return info.sections.length > 0 && info.sections.every(s => s.sectionReview?.status === ReviewStatus.Approved && !this.tabHasPending(s));
  }

  private updateItem(reviewItemId: string, status: ReviewStatus, note?: string): void {
    if (!this.selectedProfileId()) return;
    this.loadingDetail.set(true);
    this.api
      .reviewItem(reviewItemId, status, note)
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: () => {
          this.reloadCurrentProfile('review');
        },
        error: err => {
          const message = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.errors.reviewItem');
          this.error.set(message);
        },
      });
  }

  isTabLoading(tab: ContentTab): boolean {
    return this.tabState()[tab].loading || this.loadingDetail();
  }

  tabReady(tab: ContentTab): boolean {
    return this.tabState()[tab].loaded && !!this.detail();
  }

  visibleTabs(): { key: ContentTab; label: string }[] {
    return [
      { key: 'profile', label: 'profileApproval.tabs.profileData' },
      { key: 'review', label: 'profileApproval.tabs.reviewSections' },
      ...(this.partialMode() ? [] : [{ key: 'final', label: 'profileApproval.tabs.finalDecision' }]),
    ];
  }

  private resetTabState(): void {
    this.tabState.set(this.createTabState());
    this.activeContentTab.set('profile');
    this.detail.set(null);
    this.activeSection.set(null);
  }

  private reloadCurrentProfile(tab: ContentTab = this.activeContentTab()): void {
    this.activeContentTab.set(tab);
    this.loadTabData(tab, true);
  }

  private tabRequestOptions(tab: ContentTab): { includeProfile: boolean; includeSections: boolean } {
    if (tab === 'profile') {
      return { includeProfile: true, includeSections: false };
    }

    return { includeProfile: false, includeSections: true };
  }

  private mergeDetail(
    current: ProfileApprovalDetail | null,
    incoming: ProfileApprovalDetail
  ): ProfileApprovalDetail {
    if (!current) return incoming;

    return {
      ...current,
      ...incoming,
      profile: incoming.profile ?? current.profile,
      sections: incoming.sections?.length ? incoming.sections : current.sections ?? [],
    };
  }

  private createTabState(): TabState {
    return {
      profile: { loading: false, loaded: false },
      review: { loading: false, loaded: false },
      final: { loading: false, loaded: false },
    };
  }

  private setTabState(tab: ContentTab, changes: Partial<TabState[ContentTab]>): void {
    const current = this.tabState();
    this.tabState.set({ ...current, [tab]: { ...current[tab], ...changes } });
  }
}
