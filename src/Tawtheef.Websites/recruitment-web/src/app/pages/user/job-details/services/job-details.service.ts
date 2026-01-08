import {inject, Injectable, signal} from '@angular/core';
import {finalize, Observable, tap} from 'rxjs';

import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {JobDetailsModel} from '../models/job-details.model';

@Injectable({providedIn: 'root'})
export class JobDetailsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  job = signal<JobDetailsModel | null>(null);
  loading = signal(false);
  applying = signal(false);

  loadJobDetails(invitationId: string): Observable<JobDetailsModel> {
  this.loading.set(true);

  return this.http
    .get<JobDetailsModel>(this.endpoints.dashboard.candidateInvitationJobDetails(invitationId))
    .pipe(
      tap((response) => this.job.set(response)),
      finalize(() => this.loading.set(false))
    );
}

  applyInvitation(invitationId: string) {
    this.applying.set(true);
    return this.http
      .post<void>(this.endpoints.dashboard.applyCandidateInvitation(invitationId), {})
      .pipe(
        finalize(() => this.applying.set(false))
      );
  }

  changeInvitationStatus(invitationId: string, statusCode: string) {
    this.applying.set(true);
    return this.http
      .post<void>(this.endpoints.dashboard.changeStatusCandidateInvitation(invitationId), {statusCode})
      .pipe(
        finalize(() => this.applying.set(false))
      );
  }
}
