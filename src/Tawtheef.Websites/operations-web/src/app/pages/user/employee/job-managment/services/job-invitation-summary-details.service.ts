import { Injectable, signal } from '@angular/core';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesRowsFilters,
  JobInvitesStatsVM,
  LookupOption,
  PaginationMetadata,
} from '../models/job-invitation-summary-details.model';

// اربط هنا HttpClient و Endpoints الخاصة بكم
@Injectable({ providedIn: 'root' })
export class JobInvitationSummaryDetailsService {
  // state
  jobInfo = signal<JobInfoVM | null>(null);
  stats = signal<JobInvitesStatsVM | null>(null);
  rows = signal<InviteRowVM[]>([]);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  // lookups
  private _yearsOptions = signal<LookupOption[]>([]);
  private _statusOptions = signal<LookupOption[]>([]);

  yearsOptions() {
    return this._yearsOptions();
  }

  statusOptions() {
    return this._statusOptions();
  }

  loadLookups(): void {
    // TODO: API call
    // this.http.get(...).subscribe(res => this._yearsOptions.set(res))
    // this.http.get(...).subscribe(res => this._statusOptions.set(res))
  }

  getJobInfo(jobId: string): void {
    // TODO: API call
    // this.http.get<JobInfoVM>(`.../${jobId}`).subscribe(res => this.jobInfo.set(res))
  }

  getStats(payload: { jobId: string; academicYear: string }): void {
    // TODO: API call
    // this.http.post<JobInvitesStatsVM>(`...`, payload).subscribe(res => this.stats.set(res))
  }

  getRows(filters: JobInvitesRowsFilters): void {
    // TODO: API call
    // this.http.post<{ items: InviteRowVM[]; metadata: PaginationMetadata }>(`...`, filters)
    //   .subscribe(res => { this.rows.set(res.items); this.paginationMetadata.set(res.metadata); })
  }
}
