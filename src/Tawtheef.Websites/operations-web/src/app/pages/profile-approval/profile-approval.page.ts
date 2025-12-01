import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { finalize, Subscription } from 'rxjs';
import { ProfileApprovalService } from './profile-approval.service';
import {
  ProfileApprovalDetail,
  ProfileApprovalItem,
  ProfileApprovalListItem,
  ProfileApprovalSection,
  ReviewStatus,
  ReviewTargetType,
} from './profile-approval.models';

@Component({
  selector: 'app-profile-approval-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
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

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  readonly hasSelection = computed(() => !!this.detail());

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
        next: detail => this.detail.set(detail),
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
        return 'badge text-bg-success';
      case ReviewStatus.Rejected:
        return 'badge text-bg-danger';
      case ReviewStatus.ChangesRequested:
        return 'badge text-bg-warning text-dark';
      default:
        return 'badge text-bg-secondary';
    }
  }

  statusLabel(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'تم الاعتماد';
      case ReviewStatus.Rejected:
        return 'مرفوض';
      case ReviewStatus.ChangesRequested:
        return 'بحاجة لتعديل';
      case ReviewStatus.Pending:
      default:
        return 'بانتظار المراجعة';
    }
  }

  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'المعلومات الشخصية';
      case 2:
        return 'معلومات التواصل';
      case 3:
        return 'المؤهلات العلمية';
      case 4:
        return 'الخبرات والدورات';
      case 5:
        return 'المهارات واللغات';
      case 6:
        return 'المرفقات';
      default:
        return 'قسم';
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
