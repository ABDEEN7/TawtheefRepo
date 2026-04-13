import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {CountryDto} from '../models/country.dto';
import {CountryFilters} from '../models/country-filters.dto';

@Injectable({ providedIn: 'root' })
export class CountriesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getCountries(filters: CountryFilters): Observable<PaginatedResult<CountryDto>> {
    return this.http.get<PaginatedResult<CountryDto>>(this.endpoints.countries.listCountries, filters);
  }

  createCountry(country: any): Observable<string> {
    return this.http.post<string>(this.endpoints.countries.createCountry, country);
  }

  updateCountry(id: string, country: any): Observable<void> {
    return this.http.put<void>(this.endpoints.countries.updateCountry(id), country);
  }

  updateStatus(countryId: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.countries.updateStatus(countryId), { isActive });
  }
}
