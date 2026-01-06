import { inject, Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpService } from '../../../core/http/http.service';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { GUID } from '../../../shared/types/guid.type';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { JobCandidateListItem } from '../models/job-candidate.model';
import { JobCandidatesFilter } from '../models/job-candidates-filter.model';
import { JobCandidatesOverview } from '../models/job-candidates-overview.model';
import { JobCandidatesExportRequest } from '../models/job-candidates-export-request.model';
import {
  SendJobCandidateInvitationsRequest,
  SendJobCandidateInvitationsResult,
} from '../models/job-candidates-invitations.model';
import { JobCandidatesFilterSettings } from '../models/job-candidates-filter-settings.model';
import { JobCandidatesResponse } from '../models/job-candidates-response';

@Injectable({ providedIn: 'root' })
export class JobCandidatesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getFilterSettings(jobId: GUID): Observable<JobCandidatesFilterSettings> {
    return this.http.get<JobCandidatesFilterSettings>(this.endpoints.jobCandidates.filters, { jobId });
  }

  saveFilterSettings(settings: JobCandidatesFilterSettings): Observable<JobCandidatesFilterSettings> {
    return this.http.post<JobCandidatesFilterSettings>(this.endpoints.jobCandidates.filters, {
      request: settings,
    });
  }

  getCandidates(
    jobId: GUID,
    pagination: PaginatedRequest,
    filter?: JobCandidatesFilter
  ): Observable<JobCandidatesResponse> {
    return this.http.post<JobCandidatesResponse>(
      this.endpoints.jobCandidates.search,
      {
        jobId,
        pagination,
        filter,
      }
    );
  }

  export(request: JobCandidatesExportRequest): Observable<HttpResponse<Blob>> {
    return this.http.post<HttpResponse<Blob>>(
      this.endpoints.jobCandidates.export,
      request,
      undefined,
      {
        observe: 'response',
        responseType: 'blob',
      }
    );
  }

  sendInvitations(
    request: SendJobCandidateInvitationsRequest
  ): Observable<SendJobCandidateInvitationsResult> {
    return this.http.post<SendJobCandidateInvitationsResult>(
      this.endpoints.jobCandidates.sendInvitations,
      request
    );
  }
}
