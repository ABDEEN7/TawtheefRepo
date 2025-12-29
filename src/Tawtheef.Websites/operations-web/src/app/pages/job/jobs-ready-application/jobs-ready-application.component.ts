import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DialogHelperService } from '../../../core/services/dialog-helper.service';
import { NotificationService } from '../../../core/services/notification.service';
import { JobLookupService } from '../services/job-lookup.service';
import { JobService } from '../services/job.service';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../core/enums/lookups.enum';
import { GUID } from '../../../shared/types/guid.type';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { take } from 'rxjs';
import { JobQueryFilter } from '../models/job-query-filter.model';

@Component({
  selector: 'app-jobs-ready-application.component',
  standalone : false,
  templateUrl: './jobs-ready-application.component.html',
  styleUrl: './jobs-ready-application.component.scss',
})
export class JobsReadyApplicationComponent {
  private jobService = inject(JobService);
   private router = inject(Router);
   private notificationService = inject(NotificationService);
   private translateService = inject(TranslateService);
   private dialogHelperService = inject(DialogHelperService);
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
     this.loadJobsWithFilters();
     this.lookupsService.loadJobCategories();
 
     this.lookupsService
       .loadJobStatus()
       .pipe(take(1))
       .subscribe();
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
 
   sendInvitaion(job:JobResponse){

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
}
