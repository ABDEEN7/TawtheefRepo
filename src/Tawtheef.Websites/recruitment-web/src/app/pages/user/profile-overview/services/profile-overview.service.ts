import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from '../../../../core/http/http.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { ProfileOverview } from '../models/profile-overview.model';

@Injectable({ providedIn: 'root' })
export class ProfileOverviewService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getOverview(): Observable<ProfileOverview> {
    return this.http.get<ProfileOverview>(this.endpoints.profile.overview);
  }
}
