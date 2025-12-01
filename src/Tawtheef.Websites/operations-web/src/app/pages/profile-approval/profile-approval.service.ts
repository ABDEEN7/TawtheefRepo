import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../core/http/http.service';
import { EndpointsService } from '../../core/http/endpoints.service';
import {
  ProfileApprovalDetail,
  ProfileApprovalListItem,
  ReviewStatus,
} from './profile-approval.models';

@Injectable({ providedIn: 'root' })
export class ProfileApprovalService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getProfiles(): Observable<ProfileApprovalListItem[]> {
    return this.http.get<ProfileApprovalListItem[]>(this.endpoints.approvals.list);
  }

  getProfile(profileId: string): Observable<ProfileApprovalDetail> {
    return this.http.get<ProfileApprovalDetail>(this.endpoints.approvals.detail(profileId));
  }

  reviewItem(reviewItemId: string, status: ReviewStatus, note?: string): Observable<void> {
    return this.http.request<void>('PATCH', this.endpoints.approvals.reviewItem(reviewItemId), {
      body: {
        status,
        note,
      },
    });
  }
}
