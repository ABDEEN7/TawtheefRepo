// candidate-dashboard.component.ts
import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { catchError, of } from 'rxjs';

import {
  CandidateDashboardService,
  JobRecord,
  ApiResponse,
  JOB_TYPES,
  JOB_TYPE_LABELS,
  JOB_STATUSES,
  JOB_STATUS_LABELS,
  STATUS_PILL_CLASSES,
  TYPE_BADGE_CLASSES,
  FILTER_OPTIONS,
  ACTION_CONFIGS,
  FilterOption,
  JobType,
  JobStatus
} from './services/candidate-dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule, TranslatePipe],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {
  private translate = inject(TranslateService);
  private candidateService = inject(CandidateDashboardService);

  // Constants exposed to template
  readonly JOB_TYPES = JOB_TYPES;
  readonly JOB_STATUSES = JOB_STATUSES;
  readonly FILTER_OPTIONS = FILTER_OPTIONS;

  // Loading states
  isLoading = signal(true);
  isWithdrawing = signal<number | null>(null);
  isRefreshing = signal(false);

  // Error state
  error = signal<string | null>(null);

  // Reactive signals
  allRecords = signal<JobRecord[]>([]);
  currentPage = signal(1);
  itemsPerPage = signal(10);

  // Filter signals
  statusFilter = signal<string>('');
  typeFilter = signal<string>('');
  entityFilter = signal<string>('');
  searchFilter = signal<string>('');

  // Computed filtered data
  filteredRecords = computed(() => {
    const status = this.statusFilter();
    const type = this.typeFilter();
    const entity = this.entityFilter().toLowerCase();
    const search = this.searchFilter().toLowerCase();

    return this.allRecords().filter(record => {
      if (status && record.status !== status) return false;
      if (type && record.type !== type) return false;
      if (entity && !record.entity.toLowerCase().includes(entity)) return false;
      if (search && !record.title.toLowerCase().includes(search)) return false;
      return true;
    });
  });

  // Computed paginated data
  paginatedRecords = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.itemsPerPage();
    const endIndex = startIndex + this.itemsPerPage();
    return this.filteredRecords().slice(startIndex, endIndex);
  });

  // Computed KPIs
  kpiInvited = computed(() =>
    this.filteredRecords().filter(r => r.status === JOB_STATUSES.INVITED).length
  );

  kpiUnderReview = computed(() =>
    this.filteredRecords().filter(r => r.status === JOB_STATUSES.UNDER_REVIEW).length
  );

  kpiWithdrawn = computed(() =>
    this.filteredRecords().filter(r => r.status === JOB_STATUSES.WITHDRAWN).length
  );

  kpiApplied = computed(() =>
    this.filteredRecords().filter(r => r.status === JOB_STATUSES.APPLIED).length
  );

  // Computed pagination info
  totalPages = computed(() =>
    Math.ceil(this.filteredRecords().length / this.itemsPerPage())
  );

  paginationStats = computed(() => {
    const total = this.filteredRecords().length;
    if (total === 0) return { start: 0, end: 0, total: 0 };

    const start = (this.currentPage() - 1) * this.itemsPerPage() + 1;
    const end = Math.min(start + this.itemsPerPage() - 1, total);

    return { start, end, total };
  });

  ngOnInit() {
    this.loadData();
  }

  // Load initial data
  loadData(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.candidateService.getJobRecords()
      .pipe(
        catchError(err => {
          this.error.set('Failed to load data. Please try again.');
          console.error('Error loading data:', err);
          return of({ data: [], message: 'Error', success: false } as ApiResponse<JobRecord[]>);
        })
      )
      .subscribe(response => {
        this.isLoading.set(false);
        if (response.success) {
          this.allRecords.set(response.data);
        }
      });
  }

  // Refresh data
  refreshData(): void {
    this.isRefreshing.set(true);
    this.candidateService.getJobRecords()
      .pipe(
        catchError(err => {
          this.error.set('Failed to refresh data.');
          console.error('Error refreshing data:', err);
          return of({ data: this.allRecords(), message: 'Error', success: false } as ApiResponse<JobRecord[]>);
        })
      )
      .subscribe(response => {
        this.isRefreshing.set(false);
        if (response.success) {
          this.allRecords.set(response.data);
          this.currentPage.set(1);
        }
      });
  }

  // Filter methods
  onStatusFilterChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.statusFilter.set(value);
    this.currentPage.set(1);
  }

  onTypeFilterChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.typeFilter.set(value);
    this.currentPage.set(1);
  }

  onEntityFilterChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.entityFilter.set(value);
    this.currentPage.set(1);
  }

  onSearchFilterChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchFilter.set(value);
    this.currentPage.set(1);
  }

  // Clear all filters
  clearFilters(): void {
    this.statusFilter.set('');
    this.typeFilter.set('');
    this.entityFilter.set('');
    this.searchFilter.set('');
    this.currentPage.set(1);
  }

  // Pagination methods
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(page => page - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(page => page + 1);
    }
  }

  // Action methods
  withdrawApplication(recordId: number): void {
    this.isWithdrawing.set(recordId);

    this.candidateService.withdrawApplication(recordId)
      .pipe(
        catchError(err => {
          this.error.set('Failed to withdraw application.');
          console.error('Error withdrawing application:', err);
          return of({ data: { id: recordId }, message: 'Error', success: false } as ApiResponse<{id: number}>);
        })
      )
      .subscribe(response => {
        this.isWithdrawing.set(null);
        if (response.success) {
          this.allRecords.update(records =>
            records.map(record =>
              record.id === recordId ? { ...record, status: JOB_STATUSES.WITHDRAWN } : record
            )
          );
          this.currentPage.set(1);
        }
      });
  }

  // Helper method for pagination numbers
  getPageNumbers(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      pages.push(i);
    }
    return pages;
  }

  // Helper methods for templates
  getTypeBadge(type: JobType): string {
    return TYPE_BADGE_CLASSES[type] || 'badge-soft';
  }

  getTypeText(type: JobType): string {
    return JOB_TYPE_LABELS[type] || type;
  }

  getStatusPill(status: JobStatus): { class: string, text: string } {
    return {
      class: STATUS_PILL_CLASSES[status] || 'status-closed',
      text: JOB_STATUS_LABELS[status] || status
    };
  }

  getActionButtons(record: JobRecord): {
    showApply: boolean;
    showView: boolean;
    showTrack: boolean;
    showDetails: boolean;
    showWithdraw: boolean;
  } {
    return ACTION_CONFIGS[record.status] || ACTION_CONFIGS[JOB_STATUSES.CLOSED];
  }

  // Retry loading data
  retry(): void {
    this.loadData();
  }

  // Get filter options with translation support - FIXED VERSION
  getTranslatedFilterOptions(): { status: FilterOption[], type: FilterOption[] } {
    return {
      status: [...FILTER_OPTIONS.STATUS],
      type: [...FILTER_OPTIONS.TYPE]
    };
  }

  // Alternative simpler approach - just remove the method if not needed
  // Since we're using FILTER_OPTIONS directly in the template
}
