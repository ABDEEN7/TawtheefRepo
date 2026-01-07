import {inject, Injectable, signal} from '@angular/core';
import {finalize, tap} from 'rxjs';

import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {CandidateInvitationModel} from '../../dashboard/models/candidate-invitation.model';

@Injectable({providedIn: 'root'})
export class CandidateInvitationDetailsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  invitation = signal<CandidateInvitationModel | null>(null);
  loading = signal(false);
  applying = signal(false);

  loadInvitation(invitationId: string): void {
    this.loading.set(true);

    this.http
      .get<CandidateInvitationModel>(this.endpoints.dashboard.candidateInvitationDetails(invitationId))
      .pipe(
        tap((response) => {
          this.invitation.set(response);
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
