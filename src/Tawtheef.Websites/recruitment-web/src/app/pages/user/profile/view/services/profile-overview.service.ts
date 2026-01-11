import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {MyProfileReviewSummaryDto, ProfileChangeRequestDto, ProfileOverview} from '../models/profile-overview.model';
import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';

@Injectable({ providedIn: 'root' })
export class ProfileOverviewService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getOverview(): Observable<ProfileOverview> {
    return this.http.get<ProfileOverview>(this.endpoints.profile.overview);
  }
  getMyProfileReviewSummary() {
    return this.http.get<MyProfileReviewSummaryDto>(this.endpoints.user.profile.reviewSummary);
  }

  getMyChangeRequests() {
    return this.http.get<ProfileChangeRequestDto[]>(this.endpoints.user.profile.changeRequests);
  }
}
