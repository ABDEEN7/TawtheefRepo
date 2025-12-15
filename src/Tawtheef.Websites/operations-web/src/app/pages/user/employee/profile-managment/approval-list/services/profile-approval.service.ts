import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ProfileApprovalDetail,
  ProfileApprovalListFilter,
  ProfileApprovalListItem,
  ReviewStatus,
} from '../models/profile-approval.models';
import {EndpointsService} from '../../../../../../core/http/endpoints.service';
import {HttpService} from '../../../../../../core/http/http.service';
import {FinalizeProfileApprovalRequest} from '../models/profile-approval-finalize.model';

@Injectable({ providedIn: 'root' })
export class ProfileApprovalService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getProfiles(filters?: ProfileApprovalListFilter): Observable<ProfileApprovalListItem[]> {
    return this.http.get<ProfileApprovalListItem[]>(this.endpoints.approvals.list, filters);
  }

  getProfile(profileId: string): Observable<ProfileApprovalDetail> {
    return this.http.get<ProfileApprovalDetail>(this.endpoints.approvals.detail(profileId));
  }

  getProfileChanges(profileId: string): Observable<ProfileApprovalDetail> {
    return this.http.get<ProfileApprovalDetail>(this.endpoints.approvals.changesDetail(profileId));
  }

  reviewItem(reviewItemId: string, status: ReviewStatus, note?: string): Observable<void> {
    return this.http.request<void>('PATCH', this.endpoints.approvals.reviewItem(reviewItemId), {
      body: {
        status,
        note,
      },
    });
  }

  finalizeProfile(profileId: string, request: FinalizeProfileApprovalRequest): Observable<void> {
    const body = new FormData();
    body.append('Action', request.action);
    if (request.summary) body.append('Summary', request.summary);
    if (request.note) body.append('Notes', request.note);
    (request.needsCorrectionItems ?? []).forEach(id => body.append('NeedsCorrectionItems', id));
    if (request.rejectionDocument) body.append('RejectionDocument', request.rejectionDocument);
    if (request.exceptionalFile) body.append('ExceptionalFile', request.exceptionalFile);

    return this.http.request<void>('POST', this.endpoints.approvals.finalize(profileId), { body });
  }
}
