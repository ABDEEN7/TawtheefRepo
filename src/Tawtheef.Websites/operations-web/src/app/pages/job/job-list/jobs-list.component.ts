import { Component, computed, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import {JobService} from '../services/job.service';
import {Job} from '../models/job.model';
import {PointsConfigModalComponent} from '../modals/points-config-modal/points-config-modal.component';
import { JobLookupService } from '../services/job-lookup.service';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { JobResponseDto } from '../models/job-response-Dto';

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

  jobs = this.jobService.jobs;
  paginationMetadata = this.jobService.paginationMetadata;
  
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<string>('');
  filterStatus = signal<string>('');

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  totalPages = computed(() => this.paginationMetadata()?.totalPages || 0);

  pagedJobs = computed(() => {
    return this.jobs();
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

    this.jobService.loadJobs(pagination, filter);
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set('');
    this.filterStatus.set('');
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadJobsWithFilters();
  }

  editJob(job: JobResponseDto) {
    this.router.navigate([`jobs/edit/${job.id}`]).then();
  }

  openPointsModal(job: JobResponseDto) {
    const ref: DynamicDialogRef | null = this.dialogService.open(PointsConfigModalComponent, {
      data: { jobId: job.id, jobTitle: job.title },
      width: '80%',
    });

    ref?.onClose.subscribe((saved: boolean) => {
      if (saved) {
        this.loadJobsWithFilters();
      }
    });
  }
}
