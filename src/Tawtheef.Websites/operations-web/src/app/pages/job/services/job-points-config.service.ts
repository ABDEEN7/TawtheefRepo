import { inject, Injectable } from '@angular/core';
import { GUID } from '../../../shared/types/guid.type';
import { TranslateService } from '@ngx-translate/core';
import { EndpointsService } from '../../../core/http/endpoints.service';
import { HttpService } from '../../../core/http/http.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Observable } from 'rxjs';
import { JobPointsResponse } from '../models/job-points-response';
import { JobPointConfiguration } from '../models/job-points-config';

@Injectable({
  providedIn: 'root',
})
export class JobPointsConfigService {
  private httpService = inject(HttpService);
  private endpoints = inject(EndpointsService);

  saveJobPoints(payload: any): Observable<GUID> {
    return this.httpService.post<GUID>(this.endpoints.job.jobPoints, payload, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  getJobPoints(jobId: GUID): Observable<JobPointsResponse> {
    return this.httpService.get<JobPointsResponse>(`${this.endpoints.job.jobPoints}/${jobId}`);
  }

  getJobPointsConfiguration(jobId: GUID): Observable<JobPointConfiguration> {
    return this.httpService.get<JobPointConfiguration>(
      `${this.endpoints.job.jobPoints}/${jobId}/config`
    );
  }

  approveJobPoints(jobId: GUID): Observable<boolean> {
    return this.httpService.post<boolean>(`${this.endpoints.job.jobPoints}/${jobId}/approve`, {});
  }
}
