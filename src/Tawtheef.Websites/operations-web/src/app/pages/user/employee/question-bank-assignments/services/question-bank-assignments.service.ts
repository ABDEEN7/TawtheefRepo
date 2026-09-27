import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import {
  AssignmentWorkspace,
  MyAssignment,
  QuestionInput,
} from '../models/question-bank-assignment.models';

@Injectable({ providedIn: 'root' })
export class QuestionBankAssignmentsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(pageNumber = 1, pageSize = 10): Observable<PaginatedResult<MyAssignment>> {
    return this.http.get<PaginatedResult<MyAssignment>>(
      this.endpoints.questionBankAssignments.list,
      { pageNumber, pageSize },
    );
  }

  workspace(id: string): Observable<AssignmentWorkspace> {
    return this.http.get<AssignmentWorkspace>(
      this.endpoints.questionBankAssignments.workspace(id),
    );
  }

  add(id: string, input: QuestionInput): Observable<string> {
    return this.http.post<string>(this.endpoints.questionBankAssignments.questions(id), input);
  }

  edit(id: string, itemId: string, input: QuestionInput): Observable<void> {
    return this.http.put<void>(
      this.endpoints.questionBankAssignments.question(id, itemId),
      input,
    );
  }

  remove(id: string, itemId: string): Observable<void> {
    return this.http.delete<void>(
      this.endpoints.questionBankAssignments.question(id, itemId),
    );
  }

  finish(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.questionBankAssignments.finish(id), {});
  }
}
