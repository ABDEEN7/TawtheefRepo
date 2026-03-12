import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {ProfileLogDto} from '../models/profile-log.dto';
import {ProfileLogFilters} from '../models/profile-log-filters.dto';

@Injectable({ providedIn: 'root' })
export class ProfileLogsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getLogs(filters: ProfileLogFilters): Observable<PaginatedResult<ProfileLogDto>> {
    return this.http.get<PaginatedResult<ProfileLogDto>>(this.endpoints.profileLogs.list, filters);
  }
}
