import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';

import {
  CandidateDashboardService,
  JOB_INVITATION_STATUSES,
  ACTION_CONFIGS,
  InvitationStatus,
  STATUS_PILL_CLASSES,
  TYPE_BADGE_CLASSES,
} from './services/candidate-dashboard.service';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {Select} from 'primeng/select';
import {CandidateInvitationFilters} from './models/candidate-invitation-filters';
import {CandidateInvitationModel} from './models/candidate-invitation.model';
import {dropdownOptionsModel} from '../../../shared/models/dropdown-options.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';
import {TableModule} from 'primeng/table';
import {routes} from '../../../routes/routes';

type ActionConfig = {
  showApply: boolean;
  showView: boolean;
  showTrack: boolean;
  showDetails: boolean;
  showWithdraw: boolean;
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    PaginationComponent,
    TableModule,
    RouterLink
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {
  candidateService = inject(CandidateDashboardService);
  routes = routes;

  // Loading states
  isWithdrawing = signal<string | null>(null);
  isRefreshing = signal(false);

  // Reactive signals
  candidateInvitations = this.candidateService.candidateInvitations;
  paginationMetadata = this.candidateService.paginationMetadata;
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  totalPages = computed(() => this.paginationMetadata()?.totalPages || 0);

  // Filter signals
  currentPage = signal(1);
  itemsPerPage = signal(3);
  selectedCategory = signal<string>('');
  selectedDepartment = signal<string>('');
  selectedInvitationStatus = signal<string>('');
  jobTitle = signal<string>('');

  ngOnInit() {
    this.candidateService.loadCandidateLookups();
    this.candidateService.loadCandidateInvitationStatistics();
    this.loadCandidateInvitations();
  }

  // Refresh data
  refreshData(): void {
    this.isRefreshing.set(true);
    this.loadCandidateInvitations()
  }

  // Filter methods

  loadCandidateInvitations() {
    const searchFilters: CandidateInvitationFilters =  {
      jobCategoryId: this.selectedCategory() || '',
      departmentId: this.selectedDepartment() || '',
      invitationStatusId: this.selectedInvitationStatus() || '',
      jobTitle: this.jobTitle() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'CreatedDate',
      sortDirection: 'asc'
    }

    this.candidateService.loadCandidateInvitations(searchFilters);
  }

  // Clear all filters
  clearFilters(): void {
    this.selectedCategory.set('');
    this.selectedDepartment.set('');
    this.selectedInvitationStatus.set('');
    this.jobTitle.set('');
    this.currentPage.set(1);
  }

  // Action methods
  withdrawApplication(recordId: string): void {
    // this.isWithdrawing.set(recordId);
    //
    // this.candidateService.withdrawApplication(recordId)
    //   .subscribe(response => {
    //     this.isWithdrawing.set(null);
    //     if (response.success) {
    //       this.allRecords.update(records =>
    //         records.map(record =>
    //           record.id === recordId ? { ...record, status: JOB_INVITATION_STATUSES.REJECTED } : record
    //         )
    //       );
    //       this.currentPage.set(1);
    //     }
    //   });
  }

  // Helper methods for templates
  getStatusClass(status: string): string {
    return STATUS_PILL_CLASSES[status as InvitationStatus] ?? 'status-closed';
  }

  getJobCategoryClass(record: CandidateInvitationModel): string {
    return TYPE_BADGE_CLASSES[record.jobCategoryBackendName as keyof typeof TYPE_BADGE_CLASSES] ?? '';
  }

  getActionButtons(invitationStatus: dropdownOptionsModel): ActionConfig {
    const status = invitationStatus.backendName as InvitationStatus;

    return ACTION_CONFIGS[status] ?? ACTION_CONFIGS[JOB_INVITATION_STATUSES.CLOSED];
  }

  // Statistics helpers
  getNewInvitationCount(): number {
    return this.candidateService.invitationStatistics()?.newInvitations || 0;
  }

  getUnderReviewCount(): number {
    return this.candidateService.invitationStatistics()?.underReview || 0;
  }

  getWithdrawnCount(): number {
    return this.candidateService.invitationStatistics()?.withdrawn || 0;
  }

  getAppliedCount(): number {
    return this.candidateService.invitationStatistics()?.applied || 0;
  }

  // Retry loading data
  retry(): void {
    this.loadCandidateInvitations();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadCandidateInvitations();
  }

  onFilterChange() {
    this.loadCandidateInvitations();
  }
}
