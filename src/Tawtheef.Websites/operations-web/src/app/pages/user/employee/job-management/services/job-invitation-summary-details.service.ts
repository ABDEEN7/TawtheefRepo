import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesRowsFilters,
  JobInvitesStatsVM,
  InvitationAttachmentVM,
} from '../models/job-invitation-summary-details.model';
import { GUID } from '../../../../../shared/types/guid.type';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
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

  getStats(payload: { jobId: GUID }): Observable<JobInvitesStatsVM> {
    return this.httpService.post<JobInvitesStatsVM>(this.endpoints.JobInvitationSummary.details.stats, payload);
  }

  getRows(filters: JobInvitesRowsFilters): Observable<PaginatedResult<InviteRowVM>> {
    return this.httpService.post<PaginatedResult<InviteRowVM>>(
      this.endpoints.JobInvitationSummary.details.rows,
      filters
    );
  }

  getInvitationAttachments(invitationId: GUID): Observable<InvitationAttachmentVM[]> {
    return this.httpService.get<InvitationAttachmentVM[]>(this.endpoints.JobInvitationSummary.details.getAttachments(invitationId));
  }

  reviewAttachment(payload: { invitationId: GUID; attachmentId: GUID; isApproved: boolean; reviewNote?: string }): Observable<void> {
    return this.httpService.post<void>(this.endpoints.JobInvitationSummary.details.reviewAttachment(payload.invitationId, payload.attachmentId), payload);
  }
  getAttachmentBlob(url: string): Observable<Blob> {
    return this.httpService.get<Blob>(url, undefined, { responseType: 'blob' });
  }
}
