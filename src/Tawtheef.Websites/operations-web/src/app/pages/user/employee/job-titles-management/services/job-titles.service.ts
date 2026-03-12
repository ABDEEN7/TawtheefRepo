import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {JobTitleDto} from '../models/job-title.dto';

@Injectable({providedIn: 'root'})
export class JobTitlesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getJobTitles() {
    return this.http.get<JobTitleDto[]>(this.endpoints.jobTitles.list);
  }

  createJobTitle(payload: JobTitleDto) {
    return this.http.post(this.endpoints.jobTitles.create, payload);
  }

  updateJobTitle(id: string, payload: JobTitleDto) {
    return this.http.put(this.endpoints.jobTitles.update(id), payload);
  }

  deleteJobTitle(id: string) {
    return this.http.delete(this.endpoints.jobTitles.delete(id));
  }
}
