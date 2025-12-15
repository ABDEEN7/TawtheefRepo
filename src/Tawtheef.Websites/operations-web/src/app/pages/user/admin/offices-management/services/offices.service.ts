import {inject, Injectable, signal} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {OfficeDto} from '../models/office.dto';
import {CountryLookupDto} from '../models/country-lookup.dto';
import {OfficeFilters} from '../models/office-filters.dto';
import {CreateOfficeRequest} from '../models/create-office-request.dto';
import {UpdateOfficeRequest} from '../models/update-office-request.dto';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {map, tap} from 'rxjs/operators';
import {Observable} from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OfficesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  private _offices = signal<OfficeDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  public offices = this._offices.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  getOffices(filters: OfficeFilters): Observable<void> {
    return this.http.get<PaginatedResult<OfficeDto>>(this.endpoints.offices.listOffices, filters)
      .pipe(
        tap(res => {
          this._offices.set(res.items || []);
          this._paginationMetadata.set(res.metadata);
        }),
        map(() => void 0)
      );
  }

  getCountries(): Observable<CountryLookupDto[]> {
    return this.http.get<CountryLookupDto[]>(this.endpoints.offices.countries)
      .pipe(map(res => res || []));
  }

  createOffice(payload: CreateOfficeRequest): Observable<OfficeDto> {
    return this.http.post<OfficeDto>(this.endpoints.offices.createOffice, payload);
  }

  updateOffice(id: string, payload: UpdateOfficeRequest): Observable<OfficeDto> {
    return this.http.put<OfficeDto>(this.endpoints.offices.updateOffice(id), payload);
  }

  deleteOffice(id: string): Observable<void> {
    return this.http.delete<void>(this.endpoints.offices.deleteOffice(id));
  }
}
