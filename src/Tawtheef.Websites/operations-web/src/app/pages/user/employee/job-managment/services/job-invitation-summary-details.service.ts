import { Injectable, inject } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesRowsFilters,
  JobInvitesStatsVM,
} from '../models/job-invitation-summary-details.model';
import { GUID } from '../../../../../shared/types/guid.type';
@Injectable({ providedIn: 'root' })
export class JobInvitationSummaryDetailsService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);

  loadLookups(): Observable<dropdownOptionsModel[]> {
    return this.httpService.get<dropdownOptionsModel[]>(this.endpoints.job.lookups.jobInvitesStatus)
  }

  getJobInvites(jobId: GUID): Observable<JobInfoVM> {
    return this.httpService.get<JobInfoVM>(this.endpoints.JobInvitationSummary.details.jobInfo(jobId));
  }

  getStats(payload: { jobId: GUID}): Observable<JobInvitesStatsVM> {
    return this.httpService.post<JobInvitesStatsVM>(this.endpoints.JobInvitationSummary.details.stats, payload);
  }

  getRows(filters: JobInvitesRowsFilters): Observable<PaginatedResult<InviteRowVM>> {
    return this.httpService.post<PaginatedResult<InviteRowVM>>(
      this.endpoints.JobInvitationSummary.details.rows,
      filters
    );
  }
}
