import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { HDR } from '../../../../../../core/utils/headers.flags';

import { AppointmentModel } from '../../interview-schedule/models/appointment.model';
import { ScheduleListItemModel, ScheduleModel } from '../../interview-schedule/models/schedule.model';

import { OperationalIssueModel } from '../models/operational-issue.model';
import { OperationalIssueType } from '../models/enums';
import {
  AppointmentEvaluationContextModel,
  AppointmentEvaluationSummaryModel,
  MemberEvaluationFormModel,
} from '../models/evaluation.model';

// Context/summary are optional, permission-scoped calls (403 for a plain evaluator is expected,
// not an error worth a toast) - the caller decides what "no access" means for that section.
const SKIP_ERROR = { headers: { [HDR.SkipError]: 'true' } };

export interface CriterionScoreInputPayload {
  criterionId: string;
  score: number;
  notes: string | null;
}

export interface SaveMemberEvaluationDraftPayload {
  appointmentId: string;
  scores: CriterionScoreInputPayload[];
  generalNotes: string | null;
}

export interface CreateOperationalIssuePayload {
  appointmentId: string;
  issueType: OperationalIssueType;
  description: string | null;
  isBlocking: boolean;
}

@Injectable({ providedIn: 'root' })
export class InterviewEvaluationService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // ---- Sessions ("Start Interview" browsing, scoped to the caller's own committees) ----
  listMySessions(): Observable<ScheduleListItemModel[]> {
    return this.http.get<ScheduleListItemModel[]>(this.endpoints.interviewEvaluationSession.list);
  }

  getMySession(id: string): Observable<ScheduleModel> {
    return this.http.get<ScheduleModel>(this.endpoints.interviewEvaluationSession.details(id));
  }

  listMySessionAppointments(id: string): Observable<AppointmentModel[]> {
    return this.http.get<AppointmentModel[]>(this.endpoints.interviewEvaluationSession.appointments(id));
  }

  // ---- Member evaluation ----
  getForm(appointmentId: string): Observable<MemberEvaluationFormModel> {
    return this.http.get<MemberEvaluationFormModel>(this.endpoints.interviewMemberEvaluation.form, { appointmentId });
  }

  getContext(appointmentId: string): Observable<AppointmentEvaluationContextModel> {
    return this.http.get<AppointmentEvaluationContextModel>(
      this.endpoints.interviewMemberEvaluation.context(appointmentId),
      undefined,
      SKIP_ERROR,
    );
  }

  getSummary(appointmentId: string): Observable<AppointmentEvaluationSummaryModel> {
    return this.http.get<AppointmentEvaluationSummaryModel>(
      this.endpoints.interviewMemberEvaluation.summary(appointmentId),
      undefined,
      SKIP_ERROR,
    );
  }

  saveDraft(payload: SaveMemberEvaluationDraftPayload): Observable<string> {
    return this.http.put<string>(this.endpoints.interviewMemberEvaluation.draft, payload);
  }

  submit(appointmentId: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewMemberEvaluation.submit, { appointmentId });
  }

  // ---- Operational issues ----
  listOperationalIssues(appointmentId: string): Observable<OperationalIssueModel[]> {
    return this.http.get<OperationalIssueModel[]>(this.endpoints.interviewOperationalIssue.list(appointmentId));
  }

  createOperationalIssue(payload: CreateOperationalIssuePayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewOperationalIssue.create, payload);
  }

  resolveOperationalIssue(issueId: string, resolutionNotes: string | null): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewOperationalIssue.resolve, { issueId, resolutionNotes });
  }

  waiveOperationalIssue(issueId: string, resolutionNotes: string | null): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewOperationalIssue.waive, { issueId, resolutionNotes });
  }

  updateOperationalIssueBlocking(issueId: string, isBlocking: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewOperationalIssue.blocking, { issueId, isBlocking });
  }
}
