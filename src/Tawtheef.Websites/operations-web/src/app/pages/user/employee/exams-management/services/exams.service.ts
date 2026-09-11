import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { ExamFilters, ExamListItemDto } from '../models/exam-list-item.dto';
import { ExamLookupsDto } from '../models/exam-lookups.dto';
import { ExamJobDto } from '../models/exam-job.dto';
import { ExamBankDto } from '../models/exam-bank.dto';
import { ExamConfigurationDto } from '../models/exam-configuration.dto';
import { SavedExamDto } from '../models/saved-exam.dto';

@Injectable({ providedIn: 'root' })
export class ExamsService {
  private readonly http = inject(HttpService);
  private readonly endpoints = inject(EndpointsService);

  wizardLookups(view = false): Observable<ExamLookupsDto> {
    return this.http.get<ExamLookupsDto>(
      view ? this.endpoints.exams.viewLookups : this.endpoints.exams.wizardLookups,
    );
  }

  jobs(search = '', includeJobId?: string, view = false): Observable<ExamJobDto[]> {
    return this.http.get<ExamJobDto[]>(
      view ? this.endpoints.exams.viewJobs : this.endpoints.exams.wizardJobs,
      { search, includeJobId },
    );
  }

  banks(jobId: string, view = false): Observable<ExamBankDto[]> {
    return this.http.get<ExamBankDto[]>(
      view ? this.endpoints.exams.viewBanks : this.endpoints.exams.wizardBanks,
      { jobId },
    );
  }

  existing(jobId: string): Observable<ExamConfigurationDto | null> {
    return this.http.get<ExamConfigurationDto | null>(this.endpoints.exams.existing, { jobId });
  }

  configuration(id: string, view = false): Observable<ExamConfigurationDto> {
    return this.http.get<ExamConfigurationDto>(
      view ? this.endpoints.exams.viewConfiguration(id) : this.endpoints.exams.configuration(id),
    );
  }

  save(exam: ExamConfigurationDto, draftId: string | null, submit: boolean): Observable<SavedExamDto> {
    return draftId
      ? this.http.put<SavedExamDto>(this.endpoints.exams.update(draftId), exam, { submit })
      : this.http.post<SavedExamDto>(this.endpoints.exams.list, exam, { submit });
  }

  returnForEdit(id: string, note: string): Observable<void> {
    return this.http.post<void>(this.endpoints.exams.return(id), { note });
  }

  reject(id: string, note: string): Observable<void> {
    return this.http.post<void>(this.endpoints.exams.reject(id), { note });
  }

  approve(id: string): Observable<void> {
    return this.http.post<void>(this.endpoints.exams.approve(id), {});
  }

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
