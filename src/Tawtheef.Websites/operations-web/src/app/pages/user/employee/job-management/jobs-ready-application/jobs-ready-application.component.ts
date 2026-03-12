import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { JobLookupService } from '../services/job-lookup.service';
import { JobService } from '../services/job.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../../../core/enums/lookups.enum';
import { GUID } from '../../../../../shared/types/guid.type';
import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { take } from 'rxjs';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { routes } from '../../../../../routes/routes';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';

@Component({
  selector: 'app-jobs-ready-application.component',
  standalone: false,
  templateUrl: './jobs-ready-application.component.html',
  styleUrl: './jobs-ready-application.component.scss',
})
export class JobsReadyApplicationComponent {
  private jobService = inject(JobService);
  private router = inject(Router);
  private authService = inject(AuthService);
  lookupsService = inject(JobLookupService);

  jobs: PaginatedResult<JobResponse> | undefined;
  paginationMetadata: PaginationMetadata | undefined;

  cancelledCount = 0;
  pendingApprovalCount = 0;
  approvedCount = 0;
  draftCount = 0;
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);

  readonly jobStatus = JobStatus;

  ngOnInit(): void {
    this.lookupsService.loadJobStatus().pipe(take(1)).subscribe(() => {
      this.loadJobsWithFilters();
    });
    this.lookupsService.loadJobCategories();
  }

  loadJobsWithFilters() {
    const pagination: PaginatedRequest = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage,
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };

    const filter: JobQueryFilter = {
      searchTerm: this.searchQuery() || undefined,
      jobCategoryId: this.filterType() || undefined,
      statusId: this.lookupsService.getStatusIdByEnum(JobStatus.Published),
    };

    this.jobService.getAll(pagination, filter).subscribe({
      next: (paginatedData) => {
        this.jobs = paginatedData;
        this.paginationMetadata = paginatedData.metadata;
      },
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadJobsWithFilters();
  }

  sendInvitation() {
    if (!this.canManageJobs()) return;
  }

  viewJobCandidate(jobId: GUID) {
    if (!this.canViewJobs()) return;
    const url = routes.portal.jobCandidates(jobId);
    this.router.navigate([url]);
  }

  getJobCategoryBadgeClass(categoryName: string): string {
    switch (categoryName?.toLowerCase()) {
      case 'academic':
        return 'bg-primary';
      case 'administrative':
        return 'bg-info';
      case 'labor':
        return 'bg-warning text-dark';
      default:
        return 'bg-secondary';
    }
  }

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.Jobs.View]);
  }
}
