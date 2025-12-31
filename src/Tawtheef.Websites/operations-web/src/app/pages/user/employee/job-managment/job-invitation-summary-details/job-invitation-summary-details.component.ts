import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { Select } from 'primeng/select';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import { JobInvitationSummaryDetailsService } from '../services/job-invitation-summary-details.service';

@Component({
  selector: 'app-job-invitation-summary-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    PaginationComponent,
  ],
  templateUrl: './job-invitation-summary-details.component.html',
  styleUrl: './job-invitation-summary-details.component.scss',
})
export class JobInvitationSummaryDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  detailsService = inject(JobInvitationSummaryDetailsService);

  // route param
  jobId = signal<string>('');

  // filters (UI)
  selectedYear = signal<string>('');
  selectedStatus = signal<string>('');
  searchText = signal<string>('');

  // paging
  currentPage = signal(1);
  itemsPerPage = signal(7);

  // data from service
  jobInfo = this.detailsService.jobInfo;                 // { jobName, ... }
  stats = this.detailsService.stats;                     // { total, applied, new, declined, cancelled }
  rows = this.detailsService.rows;                       // table rows
  paginationMetadata = this.detailsService.paginationMetadata;

  totalItems = computed(() => this.paginationMetadata()?.totalCount ?? 0);

  // lookups
  yearsOptions = computed(() => this.detailsService.yearsOptions());
  statusOptions = computed(() => this.detailsService.statusOptions());

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('jobId') ?? '';
    this.jobId.set(id);

    this.detailsService.loadLookups();
    this.loadAll();
  }

  loadAll(): void {
    this.detailsService.getJobInfo(this.jobId());

    this.detailsService.getStats({
      jobId: this.jobId(),
      academicYear: this.selectedYear() || '',
    });

    this.detailsService.getRows({
      jobId: this.jobId(),
      academicYear: this.selectedYear() || '',
      statusId: this.selectedStatus() || '',
      search: this.searchText() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'sentDate',
      sortDirection: 'desc',
    });
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadAll();
  }

  clearFilters(): void {
    this.selectedStatus.set('');
    this.searchText.set('');
    this.currentPage.set(1);
    this.loadAll();
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadAll();
  }

  // status pills classes (adjust backend names to your API)
  getStatusPillClass(backendName: string): string {
    const map: Record<string, string> = {
      Applied: 'status-pill status-applied',
      New: 'status-pill status-new',
      Cancelled: 'status-pill status-cancelled',
      Declined: 'status-pill status-declined',
      Refused: 'status-pill status-declined',
    };

    return map[backendName] ?? 'status-pill status-default';
  }
}
