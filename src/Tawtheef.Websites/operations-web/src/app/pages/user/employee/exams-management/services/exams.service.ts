import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { ExamFilters, ExamListItemDto } from '../models/exam-list-item.dto';

@Injectable({ providedIn: 'root' })
export class ExamsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: ExamFilters): Observable<PaginatedResult<ExamListItemDto>> {
    return this.http.get<PaginatedResult<ExamListItemDto>>(this.endpoints.exams.list, filters);
  }

  getStatuses(language: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.exams.statuses, { language });
  }

  getSpecializations(language: string): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.exams.specializations, { language });
  }
}
