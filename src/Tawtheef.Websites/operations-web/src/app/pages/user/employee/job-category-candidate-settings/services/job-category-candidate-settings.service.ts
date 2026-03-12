import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { JobCategoryCandidateSettings } from '../models/job-category-candidate-settings.model';

@Injectable({ providedIn: 'root' })
export class JobCategoryCandidateSettingsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getSettings(): Observable<JobCategoryCandidateSettings> {
    return this.http.get<JobCategoryCandidateSettings>(this.endpoints.jobCandidates.categorySettings);
  }

  saveSettings(settings: JobCategoryCandidateSettings): Observable<JobCategoryCandidateSettings> {
    return this.http.post<JobCategoryCandidateSettings>(this.endpoints.jobCandidates.categorySettings, {
      request: settings,
    });
  }
}
