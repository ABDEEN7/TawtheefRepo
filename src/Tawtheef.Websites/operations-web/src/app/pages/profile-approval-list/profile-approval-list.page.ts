import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { ProfileApprovalListItem, ReviewStatus } from '../profile-approval/profile-approval.models';
import { ProfileApprovalService } from '../profile-approval/profile-approval.service';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-profile-approval-list-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile-approval-list.page.html',
  styleUrl: './profile-approval-list.page.scss',
})
export class ProfileApprovalListPage implements OnInit {
  private api = inject(ProfileApprovalService);
  private router = inject(Router);

  list = signal<ProfileApprovalListItem[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  search = signal('');
  statusFilter = signal<ReviewStatus | 'all'>('all');

  readonly filteredList = computed(() => {
    const searchTerm = this.search().toLowerCase().trim();
    const status = this.statusFilter();

    return this.list()
      .filter(item => {
        const matchesStatus =
          status === 'all' ? true : item.overallStatus === status || (status === ReviewStatus.Pending && item.pendingCount > 0);
        const matchesSearch =
          !searchTerm ||
          item.fullName.toLowerCase().includes(searchTerm) ||
          (item.targetEntity ?? '').toLowerCase().includes(searchTerm) ||
          (item.candidateType ?? '').toLowerCase().includes(searchTerm);

        return matchesStatus && matchesSearch;
      })
      .sort((a, b) => {
        const aDate = a.lastUpdatedAtUtc ? new Date(a.lastUpdatedAtUtc).getTime() : 0;
        const bDate = b.lastUpdatedAtUtc ? new Date(b.lastUpdatedAtUtc).getTime() : 0;
        return bDate - aDate;
      });
  });

  readonly statusOptions: { value: ReviewStatus | 'all'; label: string }[] = [
    { value: 'all', label: 'الكل' },
    { value: ReviewStatus.Pending, label: 'بانتظار المراجعة' },
    { value: ReviewStatus.ChangesRequested, label: 'مطلوب تعديل' },
    { value: ReviewStatus.Approved, label: 'تم الاعتماد' },
    { value: ReviewStatus.Rejected, label: 'مرفوض' },
  ];

  protected readonly ReviewStatus = ReviewStatus;

  ngOnInit(): void {
    this.loadList();
  }

  loadList(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api
      .getProfiles()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: profiles => this.list.set(profiles),
        error: () => this.error.set('تعذر تحميل قوائم الملفات.'),
      });
  }

  statusClass(status: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'badge rounded-pill status-pill bg-success-subtle text-success';
      case ReviewStatus.Rejected:
        return 'badge rounded-pill status-pill bg-danger-subtle text-danger';
      case ReviewStatus.ChangesRequested:
        return 'badge rounded-pill status-pill bg-warning-subtle text-dark';
      default:
        return 'badge rounded-pill status-pill bg-secondary-subtle text-secondary';
    }
  }

  statusLabel(status: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'تم الاعتماد';
      case ReviewStatus.Rejected:
        return 'مرفوض';
      case ReviewStatus.ChangesRequested:
        return 'مطلوب تعديل';
      case ReviewStatus.Pending:
      default:
        return 'بانتظار المراجعة';
    }
  }

  openProfile(profile: ProfileApprovalListItem): void {
    this.router.navigate(['/approval'], { queryParams: { profileId: profile.userProfileId } });
  }
}
