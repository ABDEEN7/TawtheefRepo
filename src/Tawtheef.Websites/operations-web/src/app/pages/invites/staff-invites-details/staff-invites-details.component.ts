import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {StaffInvitesService} from '../services/staff-invites.service';
import {NotificationService} from '../../../core/services/notification.service';
import  {FilterOptionsEnum} from '../enums/filter-options.enum';
import {Job} from '../../job/models/job.model';
import {InviteDetails} from '../models/invite-details.model';
import {InviteStatusEnum} from '../enums/invite-status.enum';

import {KPIs} from '../models/kpis.model';
import {JobStatusEnum} from '../../job/enums/job-status.enum';
@Component({
  selector: 'app-staff-invites-details',
  standalone: false,
  templateUrl: './staff-invites-details.component.html',
  styleUrl: './staff-invites-details.component.scss',
})
export class StaffInvitesDetailsComponent implements OnInit { // Added OnInit
  private route = inject(ActivatedRoute);
  private staffInvitesService = inject(StaffInvitesService);
  private notificationService = inject(NotificationService);

  readonly FilterOptionsEnum = FilterOptionsEnum;
  readonly InviteStatusEnum = InviteStatusEnum;
  jobId=0;

  job = signal<Job | null>(null);
  allInvites = this.staffInvitesService.invites;

  statusFilter = signal<FilterOptionsEnum>(FilterOptionsEnum.All);
  searchFilter = signal<string>('');

  currentPage = signal<number>(1);
  itemsPerPage = 10;

  filteredInvites = computed<InviteDetails[]>(() => {
    let filtered = this.allInvites();

    const status = this.statusFilter();
    if (status !== FilterOptionsEnum.All) {
      filtered = filtered.filter((i) => i.status === status);
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
    const applied = invites.filter((i) => i.status === InviteStatusEnum.Applied).length;
    const declined = invites.filter((i) => i.status === InviteStatusEnum.Declined).length;
    const viewed = invites.filter((i) => i.status === InviteStatusEnum.Viewed).length;
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
    if (total === 0) return 'staff_invites_details.pagination.no_results';

    const start = (this.currentPage() - 1) * this.itemsPerPage + 1;
    const end = Math.min(total, start + this.itemsPerPage - 1);
    return `staff_invites_details.pagination.showing ${start}–${end} staff_invites_details.pagination.of ${total}`;
  });

  pageNumbers = computed<number[]>(() => {
    const total = this.totalPages();
    return Array.from({ length: total }, (_, i) => i + 1);
  });

  filterOptions = computed(() => {
    return [
      { value: FilterOptionsEnum.All, label: FilterOptionsEnum.All },
      { value: FilterOptionsEnum.Applied, label: FilterOptionsEnum.Applied },
      { value: FilterOptionsEnum.Declined, label: FilterOptionsEnum.Declined },
      { value: FilterOptionsEnum.Viewed, label: FilterOptionsEnum.Viewed },
      { value: FilterOptionsEnum.New, label: FilterOptionsEnum.New }
    ];
  });

  private readonly statusMap = {
  [InviteStatusEnum.New]: 'status-new',
  [InviteStatusEnum.Viewed]: 'status-viewed',
  [InviteStatusEnum.Applied]: 'status-applied',
  [InviteStatusEnum.Declined]: 'status-declined',
};

  ngOnInit(): void {
    this.jobId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.jobId) {
      this.loadJobDetails(this.jobId);
      this.staffInvitesService.getInvitesForJob(this.jobId).subscribe();
    }
  }

  private loadJobDetails(jobId: number): void {
    this.staffInvitesService.getJobDetails(jobId).subscribe({
      next: (job) => {
        this.job.set(job);
      },
      error: (error) => {
         this.notificationService.error(error);
      }
    });
  }

  onStatusFilterChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.statusFilter.set(target.value as FilterOptionsEnum);
    this.currentPage.set(1);
  }

  onSearchChange(value: string): void {
    this.searchFilter.set(value);
    this.currentPage.set(1);
  }

  clearFilters(): void {
    this.statusFilter.set(FilterOptionsEnum.All);
    this.searchFilter.set('');
    this.currentPage.set(1);
  }

  getStatusClass(status: string) {
  return this.statusMap[status as InviteStatusEnum] ?? 'status-viewed';
  }
  getStatusLabel(status: string): string {
    return  'staff_invites_details.status.' + (status as InviteStatusEnum || status);
  }

  getTypeClass(type: string): string {
    return `badge-soft ${type}`;
  }

  getJobStatusLabel(status?: JobStatusEnum): string {
    return 'staff_invites_details.Job_status.' + (status as JobStatusEnum || status || '');
  }

  private calculatePercentage(value: number, total: number): number {
    return total ? Math.round((value * 100) / total) : 0;
  }

   onPageChange(page: number) {
    this.currentPage.set(page);
  }
}
