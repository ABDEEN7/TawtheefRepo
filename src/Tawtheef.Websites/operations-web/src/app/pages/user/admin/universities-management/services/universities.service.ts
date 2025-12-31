import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {UniversityDto} from '../models/university.dto';
import {UniversityFilters} from '../models/university-filters.dto';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {UniversityFormPayload} from '../models/university-form.payload';

@Injectable({ providedIn: 'root' })
export class UniversitiesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getUniversities(filters: UniversityFilters): Observable<PaginatedResult<UniversityDto>> {
    return this.http.get<PaginatedResult<UniversityDto>>(this.endpoints.universities.listUniversities, filters);
  }

  getUniversityDetails(id: string): Observable<UniversityDto> {
    return this.http.get<UniversityDto>(this.endpoints.universities.universityDetails(id));
  }

  createUniversity(payload: UniversityFormPayload): Observable<UniversityDto> {
    const formData = this.buildFormData(payload);
    return this.http.post<UniversityDto>(this.endpoints.universities.createUniversity, formData);
  }

  updateUniversity(id: string, payload: UniversityFormPayload): Observable<UniversityDto> {
    const formData = this.buildFormData({...payload, id});
    return this.http.put<UniversityDto>(this.endpoints.universities.updateUniversity(id), formData);
  }

  updateStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.universities.updateStatus(id), {isActive});
  }

  getCountries(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.universities.lookups.countries);
  }

  getCities(countryId: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.universities.lookups.cities(countryId));
  }

  private buildFormData(payload: UniversityFormPayload): FormData {
    const formData = new FormData();
    const files: File[] = [];

    const logoArIndex = payload.logoArFile ? files.push(payload.logoArFile) - 1 : null;
    const logoEnIndex = payload.logoEnFile ? files.push(payload.logoEnFile) - 1 : null;

    files.forEach(file => formData.append('Files', file));

    if (logoArIndex !== null) {
      formData.append('LogoArFileIndex', logoArIndex.toString());
    }
    if (logoEnIndex !== null) {
      formData.append('LogoEnFileIndex', logoEnIndex.toString());
    }

    if (payload.id) {
      formData.append('Id', payload.id);
    }
    formData.append('NameAr', payload.nameAr);
    formData.append('NameEn', payload.nameEn);
    formData.append('CountryId', payload.countryId);
    formData.append('CityId', payload.cityId);
    formData.append('IsActive', payload.isActive.toString());

    formData.append('DescriptionAr', payload.descriptionAr ?? '');
    formData.append('DescriptionEn', payload.descriptionEn ?? '');
    formData.append('WebSite', payload.webSite ?? '');
    formData.append('Phone', payload.phone ?? '');
    formData.append('Email', payload.email ?? '');
    formData.append('Code', payload.code ?? '');
    formData.append('OriginalName', payload.originalName ?? '');

    return formData;
  }
}
