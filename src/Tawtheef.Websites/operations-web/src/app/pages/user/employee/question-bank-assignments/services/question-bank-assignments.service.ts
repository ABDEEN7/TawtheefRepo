import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
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

  image(blobKey: string): Observable<Blob> {
    const encoded = this.toBase64Url(blobKey);
    return this.http.get<Blob>(this.endpoints.questionBankAssignments.image(encoded), undefined, {
      responseType: 'blob',
      headers: { 'X-Skip-Loading': 'true' },
    });
  }

  add(id: string, input: QuestionInput): Observable<string> {
    return this.withUploadedImage(input).pipe(
      switchMap((question) => this.http.post<string>(this.endpoints.questionBankAssignments.questions(id), question)),
    );
  }

  edit(id: string, itemId: string, input: QuestionInput): Observable<void> {
    return this.withUploadedImage(input).pipe(
      switchMap((question) => this.http.put<void>(
        this.endpoints.questionBankAssignments.question(id, itemId), question,
      )),
    );
  }

  private withUploadedImage(input: QuestionInput): Observable<QuestionInput> {
    if (!input.imageFile) return of(input);
    const data = new FormData(); data.append('file', input.imageFile);
    return this.http.post<{ resourceId: string }>(
      this.endpoints.questionBankAssignments.images,
      data,
    ).pipe(map((uploaded) => ({ ...input, resourceId: uploaded.resourceId, imageFile: undefined })));
  }

  remove(id: string, itemId: string): Observable<void> {
    return this.http.delete<void>(
      this.endpoints.questionBankAssignments.question(id, itemId),
    );
  }

  finish(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.questionBankAssignments.finish(id), {});
  }

  finishModifications(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.questionBankAssignments.finishModifications(id), {});
  }

  private toBase64Url(value: string): string {
    const bytes = new TextEncoder().encode(value);
    let binary = '';
    bytes.forEach((byte) => (binary += String.fromCharCode(byte)));
    return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  }
}
