import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { JobPointConfiguration } from '../../job-management/models/job-points-config';

@Injectable({ providedIn: 'root' })
export class JobPointsConfigurationService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getConfiguration(): Observable<JobPointConfiguration> {
    return this.http.get<JobPointConfiguration>(this.endpoints.job.getJobPointsConfig);
  }

  saveConfiguration(payload: JobPointConfiguration): Observable<JobPointConfiguration> {
    return this.http.post<JobPointConfiguration>(this.endpoints.job.saveJobPointsConfig, {
      request: payload,
    });
  }
}
