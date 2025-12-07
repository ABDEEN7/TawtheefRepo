import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import {
  CandidateDashboardService,
  JOB_INVITATION_STATUSES,
  ACTION_CONFIGS, InvitationStatus,
} from './services/candidate-dashboard.service';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {Select} from 'primeng/select';
import {CandidateInvitationFilters} from './models/candidate-invitation-filters';
import {CandidateInvitationModel} from './models/candidate-invitation.model';
import {dropdownOptionsModel} from '../../../shared/models/dropdown-options.model';
import {PaginationComponent} from '../../../shared/components/pagination/pagination.component';

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
    Select,
    TranslatePipe
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {
  candidateService = inject(CandidateDashboardService);

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
  getStatusClasses(status: string): string[] {
    const map: Record<string, string[]> = {
      NewInvitation: ['bg-info-subtle', 'text-info'],             // new item = info
      Closed: ['bg-secondary-subtle', 'text-secondary'],          // closed = grey
      UnderReview: ['bg-warning-subtle', 'text-warning'],         // pending review
      Approved: ['bg-success-subtle', 'text-success'],            // approved = success
      Readed: ['bg-primary-subtle', 'text-primary'],              // read = primary
      Rejected: ['bg-danger-subtle', 'text-danger'],              // rejected = danger
      Cancelled: ['bg-dark-subtle', 'text-dark'],                 // cancelled = dark
      RequiresUpdate: ['bg-warning-subtle', 'text-warning'],      // needs update = warning
      Submitted: ['bg-info-subtle', 'text-info'],                 // submitted = info
    };

    return map[status] || ['bg-secondary-subtle', 'text-secondary']; // fallback style
  }

  getActionButtons(invitationStatus: dropdownOptionsModel): ActionConfig {
    const status = invitationStatus.backendName as InvitationStatus;

    return ACTION_CONFIGS[status] ?? ACTION_CONFIGS[JOB_INVITATION_STATUSES.CLOSED];
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
