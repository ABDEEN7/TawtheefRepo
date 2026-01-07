import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Router, RouterModule} from '@angular/router';
import {SortEvent} from 'primeng/api';
import {TableModule} from 'primeng/table';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {ProfileApprovalService} from './services/profile-approval.service';
import {ProfileApprovalListFilter, ProfileApprovalListItem, ReviewStatus,} from './models/profile-approval.models';
import {Select} from 'primeng/select';
import {InputTextModule} from 'primeng/inputtext';
import {finalize} from 'rxjs';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {routes} from '../../../../../routes/routes';
import {ProfileStatusNumber} from '../../../../../core/enums/lookups.enum';
import {NotificationService} from '../../../../../core/services/notification.service';
import {PaginationComponent} from '../../../../../shared/components/pagination/pagination.component';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';

@Component({
  selector: 'app-profile-approval-list-page',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    RouterModule, TranslateModule,
    I18nNamespaceDirective, TableModule,
    Select, InputTextModule, PaginationComponent
  ],
  templateUrl: './profile-approval-list.page.html',
  styleUrl: './profile-approval-list.page.scss',
})
export class ProfileApprovalListPage implements OnInit {
  private api = inject(ProfileApprovalService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private notifications = inject(NotificationService);

  items = signal<ProfileApprovalListItem[]>([]);
  meta = signal<PaginationMetadata>({
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1,
    hasNext: false,
    hasPreviousPage: false
  });

  loading = signal(false);

  filters = signal<ProfileApprovalListFilter>({
    search: '',
    status: '',
    candidateType: '',
    targetEntity: '',
    specialization: '',
    sortBy: 'date',
    sortDirection: 'desc',
    pageNumber: 1,
    pageSize: 10
  });

  readonly statusOptions = [
    { label: 'profileApproval.status.approved', value: ReviewStatus.Approved },
    { label: 'profileApproval.status.changes', value: ReviewStatus.ChangesRequested },
    { label: 'profileApproval.status.pending', value: ReviewStatus.Pending },
    { label: 'profileApproval.status.rejected', value: ReviewStatus.Rejected },
  ];

  readonly totalItems = computed(() => this.meta().totalCount);
  readonly currentPage = computed(() => this.filters().pageNumber);
  readonly pageSize = computed(() => this.filters().pageSize);


  ngOnInit(): void {
    this.loadList();
  }

  updateFilter<K extends keyof ProfileApprovalListFilter>(
    key: K,
    value: ProfileApprovalListFilter[K]
  ): void {
    this.filters.set({ ...this.filters(), [key]: value });
  }

  loadList(): void {
    this.loading.set(true);

    this.api.getProfiles(this.filters())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: res => {
          this.items.set(res.items ?? []);
          this.meta.set(res.metadata ?? this.meta());
        }
      });
  }

  applyFilters(): void {
    this.setPage(1);
  }

  resetFilters(): void {
    this.filters.set({
      search: '',
      status: '',
      candidateType: '',
      targetEntity: '',
      specialization: '',
      sortBy: 'date',
      sortDirection: 'desc',
      pageNumber: 1,
      pageSize: this.filters().pageSize // keep page size
    });

    this.loadList();
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;

    const sortMap: Record<string, ProfileApprovalListFilter['sortBy']> = {
      fullName: 'name',
      overallStatus: 'status',
      targetEntity: 'entity',
      submittedAtUtc: 'date',
    };

    const mapped = sortMap[event.field] ?? 'date';
    const dir: ProfileApprovalListFilter['sortDirection'] = event.order === 1 ? 'asc' : 'desc';

    this.filters.set({ ...this.filters(), sortBy: mapped, sortDirection: dir, pageNumber: 1 });
    this.loadList();
  }

  onPageChanged(page: number): void {
    this.setPage(page);
  }

  onPageSizeChanged(size: number): void {
    this.filters.set({ ...this.filters(), pageSize: size, pageNumber: 1 });
    this.loadList();
  }

  private setPage(pageNumber: number): void {
    this.filters.set({ ...this.filters(), pageNumber });
    this.loadList();
  }

  statusClass(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved: return 'pill soft';
      case ReviewStatus.Rejected: return 'pill danger';
      case ReviewStatus.ChangesRequested: return 'pill warning';
      default: return 'pill';
    }
  }

  statusLabel(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved: return 'profileApproval.status.approved';
      case ReviewStatus.Rejected: return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested: return 'profileApproval.status.changes';
      case ReviewStatus.Pending:
      default: return 'profileApproval.status.pending';
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
