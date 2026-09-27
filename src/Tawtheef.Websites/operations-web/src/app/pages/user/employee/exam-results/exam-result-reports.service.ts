import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { HttpService } from '../../../../core/http/http.service';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { HDR } from '../../../../core/utils/headers.flags';
import {
  ExamResultCandidateAttemptDetails,
  ExamResultReportDetails,
  ExamResultReportFilters,
  ExamResultReportListItem,
  ExamResultRescheduleOption,
} from './models/exam-result-report.models';
import {
  AcceptExamResultCandidateCalculatedScoreRequest,
  ApproveExamResultReportRequest,
  RescheduleExamResultCandidateRequest,
  ResolveExamResultCandidateAsNotCompletedRequest,
  ReturnExamResultReportForProcessingRequest,
} from './models/exam-result-report.requests';

@Injectable({ providedIn: 'root' })
export class ExamResultReportsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService).examResultReports;

  list(filters: ExamResultReportFilters): Observable<PaginatedResult<ExamResultReportListItem>> {
    return this.http.get<PaginatedResult<ExamResultReportListItem>>(this.endpoints.list, filters);
  }

  details(reportId: string): Observable<ExamResultReportDetails> {
    return this.http.get<ExamResultReportDetails>(this.endpoints.details(reportId));
  }

  candidateAttempt(reportId: string, candidateResultId: string): Observable<ExamResultCandidateAttemptDetails> {
    return this.http.get<ExamResultCandidateAttemptDetails>(
      this.endpoints.candidateAttempt(reportId, candidateResultId),
    );
  }

  rescheduleOptions(reportId: string, candidateResultId: string): Observable<ExamResultRescheduleOption[]> {
    return this.http.get<ExamResultRescheduleOption[]>(
      this.endpoints.rescheduleOptions(reportId, candidateResultId),
      undefined,
      { headers: { [HDR.SkipError]: 'true' } },
    );
  }

  approve(reportId: string, request: ApproveExamResultReportRequest): Observable<void> {
    return this.http.post<void>(this.endpoints.approve(reportId), request, undefined, {
      headers: { [HDR.SkipError]: 'true' },
    });
  }

  returnForProcessing(reportId: string, request: ReturnExamResultReportForProcessingRequest): Observable<void> {
    return this.http.post<void>(this.endpoints.returnForProcessing(reportId), request, undefined, {
      headers: { [HDR.SkipError]: 'true' },
    });
  }

  resolveNotCompleted(
    reportId: string,
    candidateResultId: string,
    request: ResolveExamResultCandidateAsNotCompletedRequest,
  ): Observable<void> {
    return this.http.post<void>(this.endpoints.resolveNotCompleted(reportId, candidateResultId),
      request, undefined, { headers: { [HDR.SkipError]: 'true' } });
  }

  acceptCalculatedScore(
    reportId: string,
    candidateResultId: string,
    request: AcceptExamResultCandidateCalculatedScoreRequest,
  ): Observable<void> {
    return this.http.post<void>(this.endpoints.acceptCalculatedScore(reportId, candidateResultId),
      request, undefined, { headers: { [HDR.SkipError]: 'true' } });
  }

  reschedule(
    reportId: string,
    candidateResultId: string,
    request: RescheduleExamResultCandidateRequest,
  ): Observable<string> {
    return this.http.post<string>(this.endpoints.reschedule(reportId, candidateResultId),
      request, undefined, { headers: { [HDR.SkipError]: 'true' } });
  }
}
