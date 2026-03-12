import {inject, Injectable} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {LanguageDto} from '../models/language.dto';
import {LanguageFilters} from '../models/language-filters.dto';

@Injectable({ providedIn: 'root' })
export class LanguagesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getLanguages(filters: LanguageFilters): Observable<PaginatedResult<LanguageDto>> {
    return this.http.get<PaginatedResult<LanguageDto>>(this.endpoints.languages.listLanguages, filters);
  }

  getLanguageDetails(id: string): Observable<LanguageDto> {
    return this.http.get<LanguageDto>(this.endpoints.languages.languageDetails(id));
  }

  createLanguage(payload: LanguageDto): Observable<LanguageDto> {
    return this.http.post<LanguageDto>(this.endpoints.languages.createLanguage, payload);
  }

  updateLanguage(id: string, payload: LanguageDto): Observable<LanguageDto> {
    return this.http.put<LanguageDto>(this.endpoints.languages.updateLanguage(id), payload);
  }

  updateStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.languages.updateStatus(id), {isActive});
  }
}
