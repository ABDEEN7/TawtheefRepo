import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';

import { FinalDecision } from '../models/enums';
import { ResultReportListItemModel, ResultReportModel } from '../models/result-report.model';

// Mirrors CandidateDecisionInputDto.
export interface CandidateDecisionPayload {
  candidateId: string;
  decision: FinalDecision;
  reason: string | null;
}

// Mirrors ApproveInterviewResultReportCommand.
export interface ApproveResultReportPayload {
  reportId: string;
  decisions: CandidateDecisionPayload[];
}

@Injectable({ providedIn: 'root' })
export class InterviewResultReportService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  list(): Observable<ResultReportListItemModel[]> {
    return this.http.get<ResultReportListItemModel[]>(this.endpoints.interviewResultReport.list);
  }

  getBySchedule(scheduleId: string): Observable<ResultReportModel> {
    return this.http.get<ResultReportModel>(this.endpoints.interviewResultReport.bySchedule(scheduleId));
  }

  approve(payload: ApproveResultReportPayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewResultReport.approve, payload);
  }
}
