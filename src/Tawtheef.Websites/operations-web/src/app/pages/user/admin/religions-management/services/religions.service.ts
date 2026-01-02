import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {ReligionDto} from '../models/religion.dto';
import {ReligionFilters} from '../models/religion-filters.dto';

@Injectable({ providedIn: 'root' })
export class ReligionsService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getReligions(filters: ReligionFilters): Observable<PaginatedResult<ReligionDto>> {
    return this.http.get<PaginatedResult<ReligionDto>>(this.endpoints.religions.listReligions, filters);
  }

  getReligionDetails(id: string): Observable<ReligionDto> {
    return this.http.get<ReligionDto>(this.endpoints.religions.religionDetails(id));
  }

  createReligion(payload: ReligionDto): Observable<ReligionDto> {
    return this.http.post<ReligionDto>(this.endpoints.religions.createReligion, payload);
  }

  updateReligion(id: string, payload: ReligionDto): Observable<ReligionDto> {
    return this.http.put<ReligionDto>(this.endpoints.religions.updateReligion(id), payload);
  }

  updateStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.religions.updateStatus(id), {isActive});
  }
}
