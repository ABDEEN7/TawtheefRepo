import { HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { HttpService } from '../../../../core/http/http.service';
import {
  CandidateExamCurrentQuestionResponse,
  NavigateCandidateExamResponse,
  SaveCandidateExamAnswerResponse,
  SubmitCandidateExamPartResponse,
  TimeoutCandidateExamPartResponse,
} from '../models/candidate-exam-execution.model';
import {
  CandidateExamActiveExecutionResponse,
  CandidateExamAssignment,
  StartCandidateExamResponse,
  ValidateCandidateExamAccessCodeResponse,
} from '../models/candidate-exam.model';
import {
  NavigateCandidateExamRequest,
  SaveCandidateExamAnswerRequest,
  ValidateCandidateExamAccessCodeRequest,
} from '../models/candidate-exam-request.model';

@Injectable({ providedIn: 'root' })
export class CandidateExamService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  getAssignments(): Observable<CandidateExamAssignment[]> {
    return this.http.get<CandidateExamAssignment[]>(this.endpoints.candidateExams.list, undefined, {
      headers: new HttpHeaders({
        'X-Skip-Loading': 'true',
      }),
    });
  }

  validateAccessCode(
    testSessionCandidateId: string,
    request: ValidateCandidateExamAccessCodeRequest,
  ): Observable<ValidateCandidateExamAccessCodeResponse> {
    return this.http.post<ValidateCandidateExamAccessCodeResponse>(
      this.endpoints.candidateExams.validateAccessCode(testSessionCandidateId),
      request,
    );
  }

  start(
    testSessionCandidateId: string,
    request: ValidateCandidateExamAccessCodeRequest,
    activeSessionId: string,
  ): Observable<StartCandidateExamResponse> {
    return this.http.post<StartCandidateExamResponse>(
      this.endpoints.candidateExams.start(testSessionCandidateId),
      request,
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  getActiveExecution(
    testAttemptId: string,
  ): Observable<CandidateExamActiveExecutionResponse> {
    return this.http.get<CandidateExamActiveExecutionResponse>(
      this.endpoints.candidateExams.attempts.activeExecution(testAttemptId),
      undefined,
      {
        headers: new HttpHeaders({
          'X-Skip-Loading': 'true',
        }),
      },
    );
  }

  getCurrentQuestion(
    testAttemptId: string,
    activeSessionId: string,
  ): Observable<CandidateExamCurrentQuestionResponse> {
    return this.http.get<CandidateExamCurrentQuestionResponse>(
      this.endpoints.candidateExams.attempts.currentQuestion(testAttemptId),
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  saveAnswer(
    testAttemptId: string,
    testAttemptQuestionId: string,
    activeSessionId: string,
    request: SaveCandidateExamAnswerRequest,
  ): Observable<SaveCandidateExamAnswerResponse> {
    return this.http.put<SaveCandidateExamAnswerResponse>(
      this.endpoints.candidateExams.attempts.answer(testAttemptId, testAttemptQuestionId),
      request,
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  navigate(
    testAttemptId: string,
    activeSessionId: string,
    request: NavigateCandidateExamRequest,
  ): Observable<NavigateCandidateExamResponse> {
    return this.http.post<NavigateCandidateExamResponse>(
      this.endpoints.candidateExams.attempts.navigate(testAttemptId),
      request,
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  submitPart(
    testAttemptId: string,
    testAttemptPartId: string,
    activeSessionId: string,
  ): Observable<SubmitCandidateExamPartResponse> {
    return this.http.post<SubmitCandidateExamPartResponse>(
      this.endpoints.candidateExams.attempts.submit(testAttemptId, testAttemptPartId),
      null,
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  timeoutPart(
    testAttemptId: string,
    testAttemptPartId: string,
    activeSessionId: string,
  ): Observable<TimeoutCandidateExamPartResponse> {
    return this.http.post<TimeoutCandidateExamPartResponse>(
      this.endpoints.candidateExams.attempts.timeout(testAttemptId, testAttemptPartId),
      null,
      undefined,
      this.examSessionOptions(activeSessionId),
    );
  }

  private examSessionOptions(activeSessionId: string): { headers: HttpHeaders } {
    return {
      headers: new HttpHeaders({
        'X-Exam-Session-Id': activeSessionId,
        'X-Skip-Loading': 'true',
      }),
    };
  }
}
