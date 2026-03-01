import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {SystemAdminLogDto} from '../models/system-admin-log.dto';
import {SystemAdminLogFilters} from '../models/system-admin-log-filters.dto';

@Injectable({ providedIn: 'root' })
export class SystemAdminLogsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getLogs(filters: SystemAdminLogFilters): Observable<PaginatedResult<SystemAdminLogDto>> {
    return this.http.get<PaginatedResult<SystemAdminLogDto>>(this.endpoints.systemAdminLogs.list, filters);
  }
}
