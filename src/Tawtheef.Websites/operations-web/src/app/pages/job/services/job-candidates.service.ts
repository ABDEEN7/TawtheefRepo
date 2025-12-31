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

@Injectable({ providedIn: 'root' })
export class JobCandidatesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getOverview(jobId: GUID, filter?: JobCandidatesFilter): Observable<JobCandidatesOverview> {
    return this.http.get<JobCandidatesOverview>(this.endpoints.jobCandidates.overview, {
      jobId,
      searchTerm: filter?.searchTerm,
      jobCategoryId: filter?.jobCategoryId,
      candidateTypeId: filter?.candidateTypeId,
      minimumPoints: filter?.minimumPoints,
    });
  }

  search(
    jobId: GUID,
    pagination: PaginatedRequest,
    filter?: JobCandidatesFilter
  ): Observable<PaginatedResult<JobCandidateListItem>> {
    return this.http.post<PaginatedResult<JobCandidateListItem>>(
      this.endpoints.jobCandidates.search,
      {
        jobId,
        pagination,
        filter,
      }
    );
  }

  export(request: JobCandidatesExportRequest): Observable<HttpResponse<Blob>> {
    return this.http.post<HttpResponse<Blob>>(this.endpoints.jobCandidates.export, request, undefined, {
      observe: 'response',
      responseType: 'blob',
    });
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
