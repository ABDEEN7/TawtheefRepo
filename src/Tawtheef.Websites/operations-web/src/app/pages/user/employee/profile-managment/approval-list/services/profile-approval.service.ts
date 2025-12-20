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

  finalizeProfile(profileId: string, request: FinalizeProfileApprovalRequest): Observable<void> {
    const body = new FormData();
    if (request.summary) body.append('Summary', request.summary);
    if (request.note) body.append('Notes', request.note);
    if (request.exceptionalFile) body.append('ExceptionalFile', request.exceptionalFile);

    return this.http.post<void>(this.endpoints.approvals.finalize(profileId), { body });
  }
  decideSection(userProfileId: string, section: string, body: { status: ReviewStatus; note: string | null }) {
    return this.http.put(this.endpoints.approvals.decision(userProfileId, section), body);
  }
  startReview(profileId: string): Observable<void> {
    return this.http.post<void>(this.endpoints.approvals.startReview(profileId), null);
  }
}
