import {inject, Injectable, isDevMode, signal} from '@angular/core';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import {JobInvitationSummary, JobSummaryFilters} from '../models/job-invitation-summary.model';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {forkJoin} from 'rxjs';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {tap} from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class JobInvitationSummaryService {

  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _jobInvitationSummary = signal<JobInvitationSummary[]>([]);

  public jobInvitationSummary = this._jobInvitationSummary.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  loading = signal(false);
  loaded = signal(false);

  jobCategories = signal<dropdownOptionsModel[]>([]);
  departments   = signal<dropdownOptionsModel[]>([]);
  jobStatuses   = signal<dropdownOptionsModel[]>([]);

  // response storage
  summaries = signal<JobInvitationSummary[]>([]);

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
          if(isDevMode())
            console.error('Failed to load JobInvitationSummary lookups', err);
          this.loading.set(false);
        }
      });
  }

  getInvitationSummaries(filters?: JobSummaryFilters): void {
    this.http.get<PaginatedResult<JobInvitationSummary>>(this.endpoints.JobInvitationSummary.invitationsSummary, filters).pipe(
      tap(response => {
        this._jobInvitationSummary.set(response.items || []);
        this._paginationMetadata.set(response.metadata);
      }),
    ).subscribe();
  }
}
