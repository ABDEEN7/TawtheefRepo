import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { QuestionBankFilters } from '../models/question-bank-filters.dto';
import { QuestionBankListItemDto } from '../models/question-bank-list-item.dto';

@Injectable({ providedIn: 'root' })
export class QuestionBanksService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  list(filters: QuestionBankFilters): Observable<PaginatedResult<QuestionBankListItemDto>> {
    return this.http.get<PaginatedResult<QuestionBankListItemDto>>(
      this.endpoints.questionBanks.list,
      filters
    );
  }

  getQuestionBankTypes(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBanks.lookups.questionBankTypes);
  }

  getManagements(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBanks.lookups.managements);
  }

  getJobTitles(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBanks.lookups.jobTitles);
  }

}
