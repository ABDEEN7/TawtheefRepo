import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { finalize, Subscription } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ProfileApprovalService } from './services/profile-approval.service';
import {
  FinalApprovalAction,
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalListFilter,
  ProfileApprovalListItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
} from './models/profile-approval.models';
import { I18nNamespaceDirective } from '../../shared/directives/i18n-namespace.directive';
import { FileUtilsService } from '../../core/utils/file-utils';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { RadioButtonModule } from 'primeng/radiobutton';
import { MessageService, SortEvent } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-profile-approval-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,
    I18nNamespaceDirective,
    DialogModule,
    InputTextareaModule,
    DropdownModule,
    RadioButtonModule,
    ToastModule,
    InputTextModule,
    TableModule,
  ],
  templateUrl: './profile-approval.page.html',
  styleUrl: './profile-approval.page.scss',
  providers: [MessageService],
})
export class ProfileApprovalPage implements OnInit, OnDestroy {
  private fileUtils = inject(FileUtilsService);
  private api = inject(ProfileApprovalService);
  private route = inject(ActivatedRoute);
  private messages = inject(MessageService);
  private translate = inject(TranslateService);

  private subscriptions: Subscription[] = [];

  list = signal<ProfileApprovalListItem[]>([]);
  loadingList = signal(false);
  loadingDetail = signal(false);
  loadingFinalAction = signal(false);
  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);
  filters = signal<ProfileApprovalListFilter>({
    search: '',
    status: '',
    candidateType: '',
    targetEntity: '',
    specialization: '',
    sort: 'date',
    sortDirection: 'desc',
  });
  activeSection = signal<number | null>(null);
  activeFinalAction = signal<FinalApprovalAction | null>(null);
  finalSummary = signal('');
  finalActionNote = signal('');
  finalAttachment: File | null = null;

  itemDialogVisible = signal(false);
  sectionDialogVisible = signal(false);
  confirmationVisible = signal(false);
  dialogNote = signal('');
  dialogAction = signal<'approve' | 'reject' | 'changes' | 'section-approve' | 'section-changes' | null>(null);
  dialogItem = signal<ProfileApprovalItem | null>(null);
  dialogSection = signal<ProfileApprovalSection | null>(null);

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  readonly hasSelection = computed(() => !!this.detail());

  readonly statusOptions = [
    { label: 'profileApproval.status.approved', value: ReviewStatus.Approved },
    { label: 'profileApproval.status.changes', value: ReviewStatus.ChangesRequested },
    { label: 'profileApproval.status.pending', value: ReviewStatus.Pending },
    { label: 'profileApproval.status.rejected', value: ReviewStatus.Rejected },
  ];

  updateFilter(key: keyof ProfileApprovalListFilter, value: ProfileApprovalListFilter[keyof ProfileApprovalListFilter]): void {
    this.filters.set({ ...this.filters(), [key]: value });
  }

  ngOnInit(): void {
    this.loadList();
    const sub = this.route.queryParamMap.subscribe(params => {
      const profileId = params.get('profileId');
      if (profileId) {
        this.selectProfile(profileId);
      }
    });
    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  loadList(): void {
    this.loadingList.set(true);
    this.api
      .getProfiles(this.filters())
      .pipe(finalize(() => this.loadingList.set(false)))
      .subscribe({
        next: profiles => {
          this.list.set(profiles);
          if (!this.selectedProfileId() && profiles.length > 0) {
            this.selectProfile(profiles[0].userProfileId);
          }
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadList')),
      });
  }

  selectProfile(profileId: string): void {
    if (!profileId) return;
    this.selectedProfileId.set(profileId);
    this.loadDetail(profileId);
  }

  loadDetail(profileId: string): void {
    this.loadingDetail.set(true);
    this.error.set(null);
    this.api
      .getProfile(profileId)
      .pipe(finalize(() => this.loadingDetail.set(false)))
      .subscribe({
        next: detail => {
          this.detail.set(detail);
          const firstSection = detail.sections[0]?.section ?? null;
          this.activeSection.set(firstSection);
        },
        error: () => this.error.set(this.translate.instant('profileApproval.errors.loadDetail')),
      });
  }

  applyFilters(): void {
    this.loadList();
  }

  resetFilters(): void {
    this.filters.set({ search: '', status: '', candidateType: '', targetEntity: '', specialization: '', sort: 'date', sortDirection: 'desc' });
    this.loadList();
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;
    const sortMap: Record<string, ProfileApprovalListFilter['sort']> = {
      fullName: 'name',
      overallStatus: 'status',
      targetEntity: 'entity',
      submittedAtUtc: 'date',
    };
    const mapped = sortMap[event.field] ?? 'date';
    this.filters.set({ ...this.filters(), sort: mapped, sortDirection: event.order === 1 ? 'asc' : 'desc' });
    this.loadList();
  }

  openItemDialog(item: ProfileApprovalItem, action: 'approve' | 'reject' | 'changes'): void {
    this.dialogItem.set(item);
    this.dialogSection.set(null);
    this.dialogAction.set(action);
    this.dialogNote.set('');
    this.itemDialogVisible.set(true);
  }

  openSectionDialog(section: ProfileApprovalSection, action: 'section-approve' | 'section-changes'): void {
    this.dialogSection.set(section);
    this.dialogItem.set(null);
    this.dialogAction.set(action);
    this.dialogNote.set(section.sectionReview?.note ?? '');
    this.sectionDialogVisible.set(true);
  }

  submitDialog(): void {
    const action = this.dialogAction();
    if (!action) return;

    if (action === 'approve') {
      this.updateItem(this.dialogItem()!.reviewItemId, ReviewStatus.Approved);
      return;
    }

    if (action === 'reject') {
      if (!this.dialogNote().trim()) {
        this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.noteRequired') });
        return;
      }
      this.updateItem(this.dialogItem()!.reviewItemId, ReviewStatus.Rejected, this.dialogNote());
      return;
    }

    if (action === 'changes') {
      if (!this.dialogNote().trim()) {
        this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.noteRequired') });
        return;
      }
      this.updateItem(this.dialogItem()!.reviewItemId, ReviewStatus.ChangesRequested, this.dialogNote());
      return;
    }

    if (action === 'section-approve' && this.dialogSection()?.sectionReview) {
      this.updateItem(this.dialogSection()!.sectionReview!.reviewItemId, ReviewStatus.Approved, this.dialogNote());
      return;
    }

    if (action === 'section-changes' && this.dialogSection()?.sectionReview) {
      if (!this.dialogNote().trim()) {
        this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.noteRequired') });
        return;
      }
      this.updateItem(this.dialogSection()!.sectionReview!.reviewItemId, ReviewStatus.ChangesRequested, this.dialogNote());
      return;
    }
  }

  canApproveAttachment(section: ProfileApprovalSection, item: ProfileApprovalItem): boolean {
    if (item.targetType !== ReviewTargetType.Attachment) return true;
    return section.sectionReview?.status !== ReviewStatus.Rejected;
  }

  sectionCanBeApproved(section: ProfileApprovalSection): boolean {
    const attachments = section.items.filter(i => i.targetType === ReviewTargetType.Attachment);
    const allAttachmentsApproved = attachments.every(i => i.status === ReviewStatus.Approved);
    return allAttachmentsApproved;
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
    this.fileUtils.previewUrl(resourceUrl, '', false);
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

  setFinalAction(action: FinalApprovalAction): void {
    this.activeFinalAction.set(action);
    this.finalActionNote.set('');
    this.finalAttachment = null;
  }

  confirmFinalAction(): void {
    const action = this.activeFinalAction();
    if (!action) return;

    if (action === 'ApproveProfile' && !this.canApproveProfile()) {
      this.messages.add({ severity: 'error', summary: this.translate.instant('profileApproval.validation.cannotApproveProfile') });
      return;
    }

    if (action === 'NeedsCorrection' && !this.finalActionNote().trim()) {
      this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.noteRequired') });
      return;
    }

    if (action === 'NeedsCorrection' && this.collectNeedsCorrectionTargets().length === 0) {
      this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.correctionTargetsRequired') });
      return;
    }

    if (action === 'RejectProfile' && (!this.finalActionNote().trim() || !this.finalAttachment)) {
      this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.rejectRequirements') });
      return;
    }

    if (action === 'BlockProfile' && !this.finalActionNote().trim()) {
      this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.noteRequired') });
      return;
    }

    if (action === 'ExceptionalApproval' && !this.finalAttachment) {
      this.messages.add({ severity: 'warn', summary: this.translate.instant('profileApproval.validation.exceptionRequirements') });
      return;
    }

    this.confirmationVisible.set(true);
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
          this.confirmationVisible.set(false);
          this.messages.add({
            severity: 'success',
            summary: this.translate.instant('profileApproval.final.actionExecuted'),
            detail: this.translate.instant('profileApproval.final.actionQueued'),
          });
          this.activeFinalAction.set(null);
          this.finalActionNote.set('');
          this.finalAttachment = null;
          this.loadDetail(this.detail()!.userProfileId);
          this.loadList();
        },
        error: () =>
          this.messages.add({ severity: 'error', summary: this.translate.instant('profileApproval.errors.finalize') }),
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
          this.loadDetail(this.selectedProfileId()!);
          this.loadList();
        },
        error: err => {
          const message = err?.error?.[0]?.message ?? this.translate.instant('profileApproval.errors.reviewItem');
          this.error.set(message);
        },
      });
  }
}
