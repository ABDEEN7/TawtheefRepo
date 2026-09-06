import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { LocationDto, LocationFilters, SaveLocationRequest } from '../models/location.dto';

@Injectable({ providedIn: 'root' })
export class LocationsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: LocationFilters): Observable<PaginatedResult<LocationDto>> {
    return this.http.get<PaginatedResult<LocationDto>>(this.endpoints.locations.list, filters);
  }

  create(request: SaveLocationRequest): Observable<string> {
    return this.http.post<string>(this.endpoints.locations.create, request);
  }

  update(id: string, request: SaveLocationRequest): Observable<void> {
    return this.http.put<void>(this.endpoints.locations.update(id), request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(this.endpoints.locations.delete(id));
  }
}
