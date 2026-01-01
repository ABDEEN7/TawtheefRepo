import { Injectable, inject, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesRowsFilters,
  JobInvitesStatsVM,
  LookupOption,
  PaginationMetadata,
} from '../models/job-invitation-summary-details.model';
import { GUID } from '../../../../../shared/types/guid.type';
@Injectable({ providedIn: 'root' })
export class JobInvitationSummaryDetailsService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);

  jobInfo = signal<JobInfoVM | null>(null);
  stats = signal<JobInvitesStatsVM | null>(null);
  rows = signal<InviteRowVM[]>([]);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  private _statusOptions = signal<LookupOption[]>([]);

  statusOptions() {
    return this._statusOptions();
  }

  loadLookups(jobId: string): void {
    forkJoin({
      
      statuses: this.httpService.get<dropdownOptionsModel[]>(
        this.endpoints.job.lookups.jobInvitesStatus
      ),
    }).subscribe({
      next: (response) => {
        this._statusOptions.set(response.statuses);
      },
    });
  }

  getJobInvites(jobId: GUID): void {
    this.httpService
      .get<JobInfoVM>(this.endpoints.JobInvitationSummary.details.jobInfo(jobId))
      .subscribe((res) => this.jobInfo.set(res));
  }

  getStats(payload: { jobId: GUID}): void {
    this.httpService
      .post<JobInvitesStatsVM>(this.endpoints.JobInvitationSummary.details.stats, payload)
      .subscribe((res) => this.stats.set(res));
  }

  getRows(filters: JobInvitesRowsFilters): void {
    this.httpService
      .post<PaginatedResult<InviteRowVM>>(this.endpoints.JobInvitationSummary.details.rows, filters)
      .subscribe((res) => {
        this.rows.set(res.items ?? []);
        this.paginationMetadata.set(res.metadata);
      });
  }
}
