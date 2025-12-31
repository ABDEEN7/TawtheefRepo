import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DialogHelperService } from '../../../core/services/dialog-helper.service';
import { NotificationService } from '../../../core/services/notification.service';
import { JobLookupService } from '../services/job-lookup.service';
import { JobService } from '../services/job.service';
import { GUID } from '../../../shared/types/guid.type';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { JobResponse } from '../models/job-response-model';

@Component({
  selector: 'app-job-candidates.component',
  standalone:false,
  templateUrl: './job-candidates.component.html',
  styleUrl: './job-candidates.component.scss',
})
export class JobCandidatesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  lookupsService = inject(JobLookupService);

  paginationMetadata: PaginationMetadata | undefined;
  
  jobId!: GUID;

  totalCandidatesCount = 0;
  availableCandidatesCount = 0;
  abovePointsCandidatesCount = 0;
  pointsAverage = 0;
  currentPage = signal(1);
  itemsPerPage = 10;

  candidates :PaginatedResult<any>  | undefined;
  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('id') as GUID;
  }

  private loadJobCandidatesByJobId(): void {
   //TODO :: Load Job Candidates based on Job Id
  }

   onFilterChange() {
    this.currentPage.set(1);
    //TODO :: Load Candidates here
  }

   clearFilters() {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.filterStatus.set(null);
    this.currentPage.set(1);
    //TODO :: Load Candidates here
  }

  viewDetails(candidateId :GUID)
  {
  }
  exportToExcel() {
  }

  sendInvitations() {
  }

    onPageChange(page: number) {
    this.currentPage.set(page);
    //TODO :: Load Candidates here
  }
}
