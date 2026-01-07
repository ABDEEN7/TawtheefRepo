import {inject, Injectable, signal} from '@angular/core';
import {finalize, tap} from 'rxjs';

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

  loadJobDetails(invitationId: string): void {
    this.loading.set(true);

    this.http
      .get<JobDetailsModel>(this.endpoints.dashboard.candidateInvitationJobDetails(invitationId))
      .pipe(
        tap((response) => {
          this.job.set(response);
          this.loading.set(false);
        })
      )
      .subscribe({
        error: () => this.loading.set(false)
      });
  }

  applyInvitation(invitationId: string) {
    this.applying.set(true);
    return this.http
      .post<void>(this.endpoints.dashboard.applyCandidateInvitation(invitationId), {})
      .pipe(
        finalize(() => this.applying.set(false))
      );
  }
}
