import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ProfileApprovalDetail,
  ProfileApprovalListFilter,
  ProfileApprovalListItem,
  ReviewStatus,
} from '../models/profile-approval.models';
import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';

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

  reviewItem(reviewItemId: string, status: ReviewStatus, note?: string): Observable<void> {
    return this.http.request<void>('PATCH', this.endpoints.approvals.reviewItem(reviewItemId), {
      body: {
        status,
        note,
      },
    });
  }

  finalizeProfile(profileId: string, body: FormData): Observable<void> {
    return this.http.request<void>('POST', this.endpoints.approvals.finalize(profileId), {
      body,
    });
  }
}
