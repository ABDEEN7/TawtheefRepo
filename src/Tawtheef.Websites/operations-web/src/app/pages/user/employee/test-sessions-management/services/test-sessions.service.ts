import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import {
  TestSessionFilters,
  TestSessionEditDto,
  TestSessionListItemDto,
  TestSessionLookupsDto,
} from '../models/test-session-list-item.dto';
import { TestSessionExamDetailsDto } from '../models/test-session-exam-details.dto';
import {
  TestSessionCandidateFilters,
  TestSessionCandidatesDto,
} from '../models/test-session-candidate.dto';
import { ReadyTestSlotFilters, ReadyTestSlotsDto } from '../models/ready-test-slot.dto';
import { SaveTestSessionSetupDto, TestSessionSetupDto } from '../models/test-session-setup.dto';
import { HDR } from '../../../../../core/utils/headers.flags';

@Injectable({ providedIn: 'root' })
export class TestSessionsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: TestSessionFilters): Observable<PaginatedResult<TestSessionListItemDto>> {
    return this.http.get<PaginatedResult<TestSessionListItemDto>>(this.endpoints.testSessions.list, filters);
  }

  lookups(language: string, roomId?: string): Observable<TestSessionLookupsDto> {
    return this.http.get<TestSessionLookupsDto>(this.endpoints.testSessions.lookups, { language, roomId });
  }

  examDetails(examId: string, language: string): Observable<TestSessionExamDetailsDto> {
    return this.http.get<TestSessionExamDetailsDto>(this.endpoints.testSessions.examDetails(examId), { language });
  }

  candidates(filters: TestSessionCandidateFilters): Observable<TestSessionCandidatesDto> {
    return this.http.get<TestSessionCandidatesDto>(this.endpoints.testSessions.candidates, filters);
  }

  readyTestSlots(filters: ReadyTestSlotFilters): Observable<ReadyTestSlotsDto> {
    return this.http.get<ReadyTestSlotsDto>(this.endpoints.testSessions.readyTestSlots, filters);
  }

  capacity(filters: { testSlotId: string; startTime: string; endTime: string; selectedCandidateCount: number }): Observable<{ availableSeats: number; isSufficient: boolean }> {
    return this.http.get<{ availableSeats: number; isSufficient: boolean }>(this.endpoints.testSessions.capacity, filters);
  }

  sessionSetup(examId: string, testSlotId: string): Observable<TestSessionSetupDto> {
    return this.http.get<TestSessionSetupDto>(this.endpoints.testSessions.sessionSetup, { examId, testSlotId });
  }

  edit(testSessionId: string, language: string): Observable<TestSessionEditDto> {
    return this.http.get<TestSessionEditDto>(
      this.endpoints.testSessions.edit(testSessionId),
      { language },
    );
  }

  view(testSessionId: string, language: string): Observable<TestSessionEditDto> {
    return this.http.get<TestSessionEditDto>(
      this.endpoints.testSessions.view(testSessionId),
      { language },
    );
  }

  saveSessionSetup(setup: SaveTestSessionSetupDto): Observable<TestSessionSetupDto> {
    return this.http.put<TestSessionSetupDto>(this.endpoints.testSessions.sessionSetup, setup, undefined, {
      headers: { [HDR.SkipError]: 'true' },
    });
  }

  approve(testSessionId: string): Observable<void> {
    return this.http.post<void>(this.endpoints.testSessions.approve(testSessionId), {});
  }
}
