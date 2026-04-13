import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {CityDto, CityFilters} from '../models/city.dto';

@Injectable({ providedIn: 'root' })
export class CitiesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getCities(filters: CityFilters): Observable<PaginatedResult<CityDto>> {
    return this.http.get<PaginatedResult<CityDto>>(this.endpoints.cities.listCities, filters);
  }

  createCity(city: Partial<CityDto>): Observable<string> {
    return this.http.post<string>(this.endpoints.cities.createCity, city);
  }

  updateCity(id: string, city: Partial<CityDto>): Observable<void> {
    return this.http.put<void>(this.endpoints.cities.updateCity(id), city);
  }

  updateStatus(cityId: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.cities.updateStatus(cityId), { isActive });
  }
}
