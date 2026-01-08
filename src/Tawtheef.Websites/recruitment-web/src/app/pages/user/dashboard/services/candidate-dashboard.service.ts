import {inject, Injectable, isDevMode, signal} from '@angular/core';
import {forkJoin, tap} from 'rxjs';
import {EndpointsService} from '../../../../core/http/endpoints.service';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {HttpService} from '../../../../core/http/http.service';
import {CandidateInvitationFilters} from '../models/candidate-invitation-filters';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {CandidateInvitationModel} from '../models/candidate-invitation.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {CandidateInvitationStatistics} from '../models/candidate-invitation-statistics.model';

export type InvitationStatus =
  typeof JOB_INVITATION_STATUSES[keyof typeof JOB_INVITATION_STATUSES];

type ActionConfig = {
  showApply: boolean;
  showView: boolean;
  showTrack: boolean;
  showDetails: boolean;
  showWithdraw: boolean;
};

// Constants for job types
export const JOB_TYPES = {
  ACADEMIC: 'Academic',
  ADMINISTRATIVE: 'Administrative',
  LABOR: 'Labor'
} as const;


// Constants for job statuses
export const JOB_INVITATION_STATUSES = {
  NEW_INVITATION: 'NewInvitation',
  CLOSED: 'Closed',
  UNDER_REVIEW: 'UnderReview',
  APPROVED: 'Approved',
  READED: 'Readed',
  REJECTED: 'Rejected',
  CANCELLED: 'Cancelled',
  REQUIRES_UPDATE: 'RequiresUpdate',
  SUBMITTED: 'Submitted'
} as const;

// Constants for status pill classes
export const STATUS_PILL_CLASSES: Record<JobStatus, string> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: 'status-invited',
  [JOB_INVITATION_STATUSES.CLOSED]: 'status-closed',
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: 'status-underreview',
  [JOB_INVITATION_STATUSES.APPROVED]: 'status-applied',
  [JOB_INVITATION_STATUSES.READED]: 'status-withdrawn',
  [JOB_INVITATION_STATUSES.REJECTED]: 'status-closed',
  [JOB_INVITATION_STATUSES.CANCELLED]: 'status-closed',
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: 'status-withdrawn',
  [JOB_INVITATION_STATUSES.SUBMITTED]: 'status-applied'
} as const;

// Constants for type badge classes
export const TYPE_BADGE_CLASSES = {
  [JOB_TYPES.ACADEMIC]: 'badge-soft academic',
  [JOB_TYPES.ADMINISTRATIVE]: 'badge-soft administrative',
  [JOB_TYPES.LABOR]: 'badge-soft labor'
} as const;

// Action configurations
export const ACTION_CONFIGS: Record<InvitationStatus, ActionConfig> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: {
    showApply: true,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.SUBMITTED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: true
  },
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: {
    showApply: false,
    showView: false,
    showTrack: true,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REJECTED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CLOSED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.APPROVED]: {
    showApply: false,
    showView: true,
    showTrack: true,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CANCELLED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.READED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
} as const;

// Type definitions
export type JobType = typeof JOB_TYPES[keyof typeof JOB_TYPES];
export type JobStatus = typeof JOB_INVITATION_STATUSES[keyof typeof JOB_INVITATION_STATUSES];

export interface FilterOption {
  value: string;
  label: string;
}

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

  // withdrawApplication(recordId: number): Observable<ApiResponse<{id: number}>> {
  //
  // }


  // applyForJob(jobId: number): Observable<ApiResponse<{jobId: number, applicationId: string}>> {
  //
  // }


  loadCandidateLookups() {
    if (this.loaded()) return;
    this.loading.set(true);

    forkJoin({
      invitationStatuses:  this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.invitationStatuses),
      jobCategories:  this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.jobCategories),
      departments:         this.http.get<dropdownOptionsModel[]>(this.endpoints.dashboard.lookups.departments),

    }).subscribe({
      next: (res) => {
        this.invitationStatuses.set(res.invitationStatuses);
        this.jobCategories.set(res.jobCategories);
        this.departments.set(res.departments);

        this.loaded.set(true);
        this.loading.set(false);
      },
      error: (err) => {
        if(isDevMode())
          console.error('Failed to load profile lookups', err);
        this.loading.set(false);
      }
    });
  }
}
