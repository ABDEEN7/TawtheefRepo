import {inject, Injectable, signal} from '@angular/core';
import {forkJoin, tap} from 'rxjs';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {HttpService} from '../../../../core/http/http.service';
import {CandidateInvitationFilters} from '../models/candidate-invitation-filters';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {CandidateInvitationModel} from '../models/candidate-invitation.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {CandidateInvitationStatistics} from '../models/candidate-invitation-statistics.model';
@Injectable({
  providedIn: 'root'
})
export class CandidateDashboardService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  loading = signal<boolean>(false);
  loaded = signal<boolean>(false);

  invitationStatuses   = signal<dropdownOptionsModel[]>([]);
  jobCategories   = signal<dropdownOptionsModel[]>([]);
  departments          = signal<dropdownOptionsModel[]>([]);

  invitationStatistics = signal<CandidateInvitationStatistics | null>(null);

  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _candidateInvitations = signal<CandidateInvitationModel[]>([]);

  public candidateInvitations = this._candidateInvitations.asReadonly();
  public paginationMetadata = this._paginationMetadata.asReadonly();

  // Simulate API call to get job records
  loadCandidateInvitations(filters?: CandidateInvitationFilters): void {
    this.http.get<PaginatedResult<CandidateInvitationModel>>(this.endpoints.dashboard.candidateInvitations, filters).pipe(
      tap(response => {
        this._candidateInvitations.set(response.items || []);
        this._paginationMetadata.set(response.metadata);
      }),
    ).subscribe();
  }

  loadCandidateInvitationStatistics(): void {
    this.http.get<CandidateInvitationStatistics>(this.endpoints.dashboard.candidateInvitationStatistics).pipe(
      tap((response) => this.invitationStatistics.set(response)),
    ).subscribe();
  }


  loadCandidateLookups() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      invitationStatuses:  this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.invitationStatuses),
      jobCategories:  this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.jobCategories),
      departments:    this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.departments),

    }).subscribe({
      next: (res) => {
        this.invitationStatuses.set(res.invitationStatuses);
        this.jobCategories.set(res.jobCategories);
        this.departments.set(res.departments);

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
      }
    });
  }
}
