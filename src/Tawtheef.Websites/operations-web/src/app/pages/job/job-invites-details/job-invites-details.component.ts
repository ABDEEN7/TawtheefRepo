import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { Job } from '../models/job.model';
import { GuidUtils } from '../../../core/utils/guid-utils';
import { GUID } from '../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { InviteDetails } from '../models/invite-details.model';
import { KPIs } from '../models/kpis.model';
import { JobInvitesService } from '../services/job-invites.service';

@Component({
  selector: 'app-job-invites-details',
  standalone: false,
  templateUrl: './job-invites-details.component.html',
  styleUrl: './job-invites-details.component.scss',
})
export class JobInvitesDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private jobInvitesService = inject(JobInvitesService);
  private notificationService = inject(NotificationService);
  lookupsService = inject(JobLookupService);

  jobId: GUID | null = GuidUtils.emptyGuid;

  job = signal<Job | null>(null);
  allInvites = this.jobInvitesService.invites;

  statusFilter = signal<string[]>([]);
  searchFilter = signal<string>('');

  currentPage = signal<number>(1);
  itemsPerPage = 10;

  filteredInvites = computed<InviteDetails[]>(() => {
    let filtered = this.allInvites();

    const statusIds = this.statusFilter();
    if (statusIds?.length > 0) {
      filtered = filtered.filter((i) => statusIds.includes(i.status));
    }

    const search = this.searchFilter().trim().toLowerCase();
    if (search) {
      filtered = filtered.filter((i) => i.profile.name.toLowerCase().includes(search));
    }

    return filtered;
  });

  kpis = computed<KPIs>(() => {
    const jobId = this.jobId;
    const invites = this.allInvites();
    const total = invites.length;
    
    const statusLookups = this.lookupsService.jobInvitesStatus();
    
    const getBackendName = (statusGuid: string): string => {
      const lookup = statusLookups.find(l => l.id === statusGuid);
      return lookup?.name || '';
    };

    const applied = invites.filter((i) => getBackendName(i.status) === 'applied').length;
    const declined = invites.filter((i) => getBackendName(i.status) === 'declined').length;
    const viewed = invites.filter((i) => getBackendName(i.status) === 'viewed').length;
    const unseen = Math.max(0, total - applied - declined - viewed);

    return {
      jobId,
      total,
      applied,
      declined,
      viewed,
      unseen,
      appliedPct: this.calculatePercentage(applied, total),
      declinedPct: this.calculatePercentage(declined, total),
      viewedPct: this.calculatePercentage(viewed, total),
      unseenPct: this.calculatePercentage(unseen, total),
    };
  });

  totalPages = computed<number>(() =>
    Math.max(1, Math.ceil(this.filteredInvites().length / this.itemsPerPage))
  );

  paginatedInvites = computed<InviteDetails[]>(() => {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredInvites().slice(start, end);
  });

  paginationInfo = computed<string>(() => {
    const total = this.filteredInvites().length;
    if (total === 0) return 'job_invites_details.pagination.no_results';

    const start = (this.currentPage() - 1) * this.itemsPerPage + 1;
    const end = Math.min(total, start + this.itemsPerPage - 1);
    return `job_invites_details.pagination.showing ${start}–${end} job_invites_details.pagination.of ${total}`;
  });

  pageNumbers = computed<number[]>(() => {
    const total = this.totalPages();
    return Array.from({ length: total }, (_, i) => i + 1);
  });

  private readonly statusMap = {
    'applied': 'status-applied',
    'declined': 'status-declined',
    'viewed': 'status-viewed',
    'new': 'status-new',
  };

  ngOnInit(): void {
    this.jobId = this.route.snapshot.paramMap.get('id') as GUID | null;
    this.lookupsService.loadAll();
    if (this.jobId) {
      this.loadJobDetails(this.jobId);
      this.jobInvitesService.getInvitesForJob(this.jobId).subscribe();
    }
  }

  private loadJobDetails(jobId: GUID): void {
    this.jobInvitesService.getJobDetails(jobId).subscribe({
      next: (job) => {
        this.job.set(job);
      },
      error: (error) => {
        this.notificationService.error(error);
      }
    });
  }

  onStatusFilterChange(selectedStatusIds: string[]): void {
    this.statusFilter.set(selectedStatusIds);
    this.currentPage.set(1);
  }

  onSearchChange(value: string): void {
    this.searchFilter.set(value);
    this.currentPage.set(1);
  }

  clearFilters(): void {
    this.statusFilter.set([]);
    this.searchFilter.set('');
    this.currentPage.set(1);
  }

  getStatusClass(statusGuid: string) {
    const backendName = this.getBackendName(statusGuid);
    return this.statusMap[backendName as keyof typeof this.statusMap] ?? 'status-viewed';
  }

  getStatusLabel(statusGuid: string): string {
    const backendName = this.getBackendName(statusGuid);
    return 'job_invites_details.status.' + (backendName || 'unknown');
  }

  getTypeClass(type: string): string {
    return `badge-soft ${type}`;
  }

  private calculatePercentage(value: number, total: number): number {
    return total ? Math.round((value * 100) / total) : 0;
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
  }

  private getBackendName(statusGuid: string): string {
    const statusLookup = this.lookupsService.jobInvitesStatus().find(
      lookup => lookup.id === statusGuid
    );
    return statusLookup?.backendName || '';
  }

  getStatusDisplayName(statusGuid: string): string {
    const statusLookup = this.lookupsService.jobInvitesStatus().find(
      lookup => lookup.id === statusGuid
    );
    return statusLookup?.name || statusGuid;
  }
}