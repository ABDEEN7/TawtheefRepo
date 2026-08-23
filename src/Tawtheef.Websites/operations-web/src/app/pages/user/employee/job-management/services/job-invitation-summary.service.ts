import { inject, Injectable, isDevMode, signal } from '@angular/core';
import { JobInvitationSummaryModel } from '../models/job-invitation-summary.model';
import { JobSummaryExportRequest, JobSummaryFilters } from '../models/job-invitation-summary-filters.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { forkJoin, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';


@Injectable({ providedIn: 'root' })
export class JobInvitationSummaryService {

  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private httpClient = inject(HttpClient);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _jobInvitationSummary = signal<JobInvitationSummaryModel[]>([]);

  public jobInvitationSummary = this._jobInvitationSummary.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  loading = signal(false);
  loaded = signal(false);

  jobCategories = signal<dropdownOptionsModel[]>([]);
  departments = signal<dropdownOptionsModel[]>([]);
  jobStatuses = signal<dropdownOptionsModel[]>([]);

  loadLookups(): void {
    if (this.loaded()) return;

    this.loading.set(true);

    forkJoin({
      jobStatuses: this.http.get<dropdownOptionsModel[]>(this.endpoints.JobInvitationSummary.lookups.jobStatuses),
      jobCategories: this.http.get<dropdownOptionsModel[]>(this.endpoints.JobInvitationSummary.lookups.jobCategories),
      departments: this.http.get<dropdownOptionsModel[]>(this.endpoints.JobInvitationSummary.lookups.departments)
    })
      .subscribe({
        next: (res) => {
          this.jobStatuses.set(res.jobStatuses);
          this.jobCategories.set(res.jobCategories);
          this.departments.set(res.departments);
          this.loaded.set(true);
          this.loading.set(false);
        },
        error: (err) => {
          if (isDevMode())
            console.error('Failed to load JobInvitationSummary lookups', err);
          this.loading.set(false);
        }
      });
  }

  getInvitationSummaries(filters?: JobSummaryFilters): void {
    this.http.get<PaginatedResult<JobInvitationSummaryModel>>(this.endpoints.JobInvitationSummary.invitationsSummary, filters).pipe(
      tap(response => {
        this._jobInvitationSummary.set(response.items || []);
        this._paginationMetadata.set(response.metadata);
      }),
    ).subscribe();
  }

  exportInvitationSummaries(request: JobSummaryExportRequest): Observable<HttpResponse<Blob>> {
    let params = new HttpParams();
    if (request.year) params = params.set('year', request.year);
    if (request.search) params = params.set('search', request.search);
    if (request.jobCategoryId) params = params.set('jobCategoryId', request.jobCategoryId);
    if (request.departmentId) params = params.set('departmentId', request.departmentId);
    if (request.jobStatusId) params = params.set('jobStatusId', request.jobStatusId);
    if (request.sortBy) params = params.set('sortBy', request.sortBy);
    if (request.sortDirection) params = params.set('sortDirection', request.sortDirection);
    return this.httpClient.get(this.endpoints.JobInvitationSummary.export, {
      params,
      responseType: 'blob',
      observe: 'response',
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }
}
