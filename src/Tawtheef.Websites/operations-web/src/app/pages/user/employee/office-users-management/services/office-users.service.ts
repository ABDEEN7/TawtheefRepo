import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {OfficeUserDto} from '../models/office-user.dto';
import {OfficeUserFilters} from '../models/office-user-filters.dto';
import {OfficeUserUpsertDto} from '../models/office-user-upsert.dto';

@Injectable({ providedIn: 'root' })
export class OfficeUsersService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getOfficeUsers(filters: OfficeUserFilters): Observable<PaginatedResult<OfficeUserDto>> {
    return this.http.get<PaginatedResult<OfficeUserDto>>(this.endpoints.officeUsers.list, filters);
  }

  createOfficeUser(payload: OfficeUserUpsertDto): Observable<void> {
    return this.http.post<void>(this.endpoints.officeUsers.create, payload);
  }

  updateOfficeUser(userId: string, payload: OfficeUserUpsertDto): Observable<void> {
    return this.http.put<void>(this.endpoints.officeUsers.update(userId), payload);
  }

  updateBlockStatus(userId: string, isBlocked: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.officeUsers.blockStatus(userId), { isBlocked });
  }
}
