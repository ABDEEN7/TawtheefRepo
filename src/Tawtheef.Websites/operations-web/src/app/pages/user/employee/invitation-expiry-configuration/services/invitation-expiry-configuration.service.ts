import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { InvitationExpiryConfiguration } from '../models/invitation-expiry-configuration.model';

@Injectable({ providedIn: 'root' })
export class InvitationExpiryConfigurationService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getConfiguration(): Observable<InvitationExpiryConfiguration> {
    return this.http.get<InvitationExpiryConfiguration>(this.endpoints.jobCandidates.invitationExpiryConfiguration);
  }

  saveConfiguration(configuration: InvitationExpiryConfiguration): Observable<InvitationExpiryConfiguration> {
    return this.http.post<InvitationExpiryConfiguration>(this.endpoints.jobCandidates.invitationExpiryConfiguration, {
      request: configuration,
    });
  }
}
