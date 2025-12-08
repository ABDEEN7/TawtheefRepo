import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { finalize, Subscription } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { ProfileApprovalService } from './services/profile-approval.service';
import {
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalListItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
} from './models/profile-approval.models';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-profile-approval-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule, I18nNamespaceDirective],
  templateUrl: './profile-approval.page.html',
  styleUrl: './profile-approval.page.scss',
})
export class ProfileApprovalPage implements OnInit, OnDestroy {
  private api = inject(ProfileApprovalService);
  private route = inject(ActivatedRoute);

  private subscriptions: Subscription[] = [];

  list = signal<ProfileApprovalListItem[]>([]);
  loadingList = signal(false);
  loadingDetail = signal(false);
  selectedProfileId = signal<string | null>(null);
  detail = signal<ProfileApprovalDetail | null>(null);
  error = signal<string | null>(null);
  listFilter = signal('');
  activeSection = signal<number | null>(null);

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  readonly hasSelection = computed(() => !!this.detail());

  readonly filteredList = computed(() => {
    const term = this.listFilter().trim().toLowerCase();
    return this.list().filter(item => {
      const matchesSearch =
        !term ||
        item.fullName.toLowerCase().includes(term) ||
        (item.targetEntity ?? '').toLowerCase().includes(term) ||
        (item.candidateType ?? '').toLowerCase().includes(term);
      return matchesSearch;
    });
  });

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
      .getProfiles()
      .pipe(finalize(() => this.loadingList.set(false)))
      .subscribe({
        next: profiles => {
          this.list.set(profiles);
          if (!this.selectedProfileId() && profiles.length > 0) {
            this.selectProfile(profiles[0].userProfileId);
          }
        },
        error: () => this.error.set('فشل تحميل الطلبات'),
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
        error: () => this.error.set('تعذر تحميل تفاصيل الملف الشخصي'),
      });
  }

  approve(item: ProfileApprovalItem): void {
    this.updateItem(item.reviewItemId, ReviewStatus.Approved);
  }

  reject(item: ProfileApprovalItem): void {
    const note = prompt('أدخل سبب الرفض');
    if (note === null) return;
    this.updateItem(item.reviewItemId, ReviewStatus.Rejected, note ?? undefined);
  }

  requestChanges(item: ProfileApprovalItem): void {
    const note = prompt('أدخل الملاحظات للتعديل');
    if (note === null) return;
    this.updateItem(item.reviewItemId, ReviewStatus.ChangesRequested, note ?? undefined);
  }

  canApproveAttachment(section: ProfileApprovalSection, item: ProfileApprovalItem): boolean {
    if (item.targetType !== ReviewTargetType.Attachment) return true;
    return section.sectionReview?.status === ReviewStatus.Approved;
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

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileApproval.sections.personal';
      case 2:
        return 'profileApproval.sections.contact';
      case 3:
        return 'profileApproval.sections.education';
      case 4:
        return 'profileApproval.sections.experience';
      case 5:
        return 'profileApproval.sections.skills';
      case 6:
        return 'profileApproval.sections.attachments';
      default:
        return 'profileApproval.sections.generic';
    }
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
          const message = err?.error?.[0]?.message ?? 'تعذر تحديث حالة المراجعة';
          this.error.set(message);
        },
      });
  }
}
