import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { HDR } from '../../../../../../core/utils/headers.flags';

import {
  CommitteeReviewListItemModel,
  CommitteeReviewModel,
  SaveCommitteeReviewPayload,
  SchoolStageOption,
} from '../models/committee-review.model';

// The list only decides whether the Committee Review action is enabled - a user without the View
// permission (403) simply gets no rows, not a toast.
const SKIP_ERROR = { headers: { [HDR.SkipError]: 'true' } };

@Injectable({ providedIn: 'root' })
export class CommitteeReviewService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  list(): Observable<CommitteeReviewListItemModel[]> {
    return this.http.get<CommitteeReviewListItemModel[]>(this.endpoints.interviewCommitteeReview.list, undefined, SKIP_ERROR);
  }

  getBySchedule(scheduleId: string): Observable<CommitteeReviewModel> {
    return this.http.get<CommitteeReviewModel>(this.endpoints.interviewCommitteeReview.bySchedule(scheduleId));
  }

  save(payload: SaveCommitteeReviewPayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommitteeReview.save, payload);
  }

  schoolStages(): Observable<SchoolStageOption[]> {
    return this.http.get<SchoolStageOption[]>(this.endpoints.interviewCommitteeReview.schoolStages);
  }
}
