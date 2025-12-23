import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { SortEvent } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ProfileApprovalService } from './services/profile-approval.service';
import {
  ProfileApprovalListFilter,
  ProfileApprovalListItem,
  ReviewStatus,
} from './models/profile-approval.models';
import { Select } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { finalize } from 'rxjs';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {routes} from '../../../../../routes/routes';
import {ProfileStatusNumber} from '../../../../../core/enums/lookups.enum';
import {NotificationService} from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-profile-approval-list-page',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    RouterModule, TranslateModule,
    I18nNamespaceDirective, TableModule,
    Select, InputTextModule
  ],
  templateUrl: './profile-approval-list.page.html',
  styleUrl: './profile-approval-list.page.scss',
})
export class ProfileApprovalListPage implements OnInit {
  private api = inject(ProfileApprovalService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private notifications = inject(NotificationService);

  list = signal<ProfileApprovalListItem[]>([]);
  loadingList = signal(false);
  filters = signal<ProfileApprovalListFilter>({
    search: '',
    status: '',
    candidateType: '',
    targetEntity: '',
    specialization: '',
    sort: 'date',
    sortDirection: 'desc',
  });

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
  }

  loadList(): void {
    this.loadingList.set(true);
    this.api
      .getProfiles(this.filters())
      .pipe(finalize(() => this.loadingList.set(false)))
      .subscribe({
        next: profiles => {
          this.list.set(profiles);
        },
        error: () => {
          this.notifications.error(this.translate.instant('profileApproval.errors.loadList'));
        },
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

  openProfile(row: ProfileApprovalListItem): void {
    if (!row?.userProfileId) return;

    const url = row.profileStatus === ProfileStatusNumber.Approved
      ? routes.employee.approvalProfileChanges(row.userProfileId)
      : routes.employee.approvalProfileReview(row.userProfileId);

    this.router.navigate([url]);
  }
}
