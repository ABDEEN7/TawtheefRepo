import { Component, computed, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { JobService } from '../services/job.service';
import { PointsConfigModalComponent } from '../modals/points-config-modal/points-config-modal.component';
import { JobLookupService } from '../services/job-lookup.service';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { JobResponse } from '../models/job-response-model';
import { GUID } from '../../../shared/types/guid.type';
import { Job } from '../models/job.model';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../core/models/paginated-result.model';

@Component({
  selector: 'app-job-list',
  standalone : false,
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);
  private dialogService = inject(DialogService);
  lookupsService = inject(JobLookupService)

  jobs:PaginatedResult<JobResponse> | undefined;
  paginationMetadata : PaginationMetadata | undefined;
  
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);

  totalItems = 0
  totalPages = 0

  pagedJobs = computed(() => {
    return this.jobs;
  });

  ngOnInit() {
    this.loadJobsWithFilters();
    this.lookupsService.loadAll();
  }

  loadJobsWithFilters() {
    const pagination: PaginatedRequest = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage,
      sortBy: 'title',
      sortDirection: 'asc'
    };

    const filter: JobQueryFilter = {
      searchTerm: this.searchQuery() || undefined,
      jobCategoryId: this.filterType() || undefined,
      statusId: this.filterStatus() || undefined
    };

    this.jobService.getAll(pagination, filter).subscribe(paginatedData=>{
      this.jobs = paginatedData
      this.paginationMetadata = paginatedData.metadata;
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.filterStatus.set(null);
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadJobsWithFilters();
  }

  editJob(job: JobResponse) {
    this.router.navigate([`/jobs`, job.id, 'wizard']).then();
  }

  viewJob(job: JobResponse) {
    this.router.navigate([`/jobs/view`, job.id]).then();
  }

  createNewJob() {
    this.router.navigate(['/jobs/create']).then();
  }

  openPointsModal(job: JobResponse) {
    const ref: DynamicDialogRef | null = this.dialogService.open(PointsConfigModalComponent, {
      data: { jobId: job.id, jobTitle: job.titleAr },
      width: '80%',
    });

    ref?.onClose.subscribe((saved: boolean) => {
      if (saved) {
        this.loadJobsWithFilters();
      }
    });
  }

  getStatusBadgeClass(statusName: string): string {
    switch(statusName?.toLowerCase()) {
      case 'draft':
        return 'bg-secondary';
      case 'pending_approval':
      case 'pending':
        return 'bg-warning text-dark';
      case 'approved':
        return 'bg-success';
      case 'published':
        return 'bg-info';
      case 'closed':
        return 'bg-dark';
      case 'rejected':
        return 'bg-danger';
      case 'cancelled':
        return 'bg-secondary';
      default:
        return 'bg-light text-dark';
    }
  }

  getJobCategoryBadgeClass(categoryName: string): string {
    switch(categoryName?.toLowerCase()) {
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
}