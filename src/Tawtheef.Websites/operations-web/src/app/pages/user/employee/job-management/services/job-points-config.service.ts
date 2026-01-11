import { inject, Injectable } from '@angular/core';
import { GUID } from '../../../../../shared/types/guid.type';
import { Observable } from 'rxjs';
import { JobPointsResponse } from '../models/job-points-response';
import { JobPointConfiguration } from '../models/job-points-config';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';

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
    return this.httpService.get<JobPointsResponse>(this.endpoints.job.getJobPoints(jobId));
  }

  getJobPointsConfiguration(): Observable<JobPointConfiguration> {
    return this.httpService.get<JobPointConfiguration>(
      `${this.endpoints.job.getJobPointsConfig}`
    );
  }

  approveJobPoints(jobId: GUID): Observable<boolean> {
    return this.httpService.post<boolean>(this.endpoints.job.approveJobPoints(jobId), {});
  }
}
