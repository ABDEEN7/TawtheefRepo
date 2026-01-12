import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { Select } from 'primeng/select';

import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import {
  InviteRowVM,
  JobInfoVM,
  JobInvitesStatsVM,
  LookupOption
} from '../models/job-invitation-summary-details.model';
import { JobInvitationSummaryDetailsService } from '../services/job-invitation-summary-details.service';
import { GUID } from '../../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../../core/utils/guid-utils';
import { routes } from '../../../../../routes/routes';
import { TableModule } from 'primeng/table';
import {PaginationMetadata} from '../../../../../core/models/pagination-metadata.model';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive'; 


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
    TableModule,
    FaDirArrowDirective
  ],
  templateUrl: './job-invitation-summary-details.component.html',
  styleUrl: './job-invitation-summary-details.component.scss',
})
export class JobInvitationSummaryDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  detailsService = inject(JobInvitationSummaryDetailsService);

  jobId = signal<GUID>(GuidUtils.emptyGuid);

  selectedStatus = signal<string | null>(null);
  searchText = signal<string>('');
  batchNumber = signal<string>('');
  currentPage = signal(1);
  itemsPerPage = signal(7);

  jobInfo = signal<JobInfoVM | null>(null);
  stats = signal<JobInvitesStatsVM | null>(null);
  rows = signal<InviteRowVM[]>([]);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  totalItems = computed(() => this.paginationMetadata()?.totalCount ?? 0);

  statusOptions = signal<LookupOption[]>([]);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('jobId') ?? '';
    this.jobId.set(id as GUID);

    this.detailsService.loadLookups().subscribe({
      next: (response) => {
        this.statusOptions.set(response);
      }
    });
    this.loadAll();
  }

  loadAll(): void {
    this.detailsService.getJobInvites(this.jobId()).subscribe((res) => this.jobInfo.set(res));

    this.detailsService.getStats({
      jobId: this.jobId(),
    }).subscribe((res) => this.stats.set(res));


    this.detailsService.getRows({
      jobId: this.jobId(),
      statusId: this.selectedStatus() || null,
      search: this.searchText() || '',
      batchNumber: this.getValidBatchNumber(),
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'createdDate',
      sortDirection: 'desc',
    }).subscribe((res) => {
      this.rows.set(res.items ?? []);
      this.paginationMetadata.set(res.metadata);
    });
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadAll();
  }

  clearFilters(): void {
    this.selectedStatus.set(null);
    this.searchText.set('');
    this.batchNumber.set('');
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
      Applied: 'pill success',
      New: 'pill info',
      Cancelled: 'pill danger',
      Declined: 'pill warning',
      Refused: 'pill warning',
    };

    return map[backendName] ?? 'pill neutral';
  }

  navigateTo() {
    this.router.navigate([routes.employee.jobInvitationSummary]);
  }

  private getValidBatchNumber(): string | null {
    const value = this.batchNumber().trim();
    return value && GuidUtils.isValid(value) ? value : null;
  }
}
