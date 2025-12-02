import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { JobSummaryFilters } from './job-invitation-summary.model';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {JobInvitationSummaryService} from './job-invitation-summary.service';
import {Select} from 'primeng/select';
import { PaginatedRequest } from '../../core/models/paginated-request.model';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-job-invitation-summary',
  templateUrl: './job-invitation-summary.html',
  styleUrls: ['./job-invitation-summary.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslatePipe, I18nNamespaceDirective, Select, TranslatePipe, Select, TranslatePipe, Select, TranslatePipe, PaginationComponent]
})
export class JobInvitationSummary implements OnInit {
  private translate = inject(TranslateService);
  jobInvitationSummaryService = inject(JobInvitationSummaryService);

  readonly TYPE_LABEL = {
    academic: { en: 'Academic', ar: 'أكاديمية' },
    administrative: { en: 'Administrative', ar: 'إدارية' },
    labor: { en: 'Labor', ar: 'عمالية' }
  };

  // Signals
  currentPage = signal(1);
  itemsPerPage = signal(3);
  selectedCategory = signal<string>('');
  selectedDepartment = signal<string>('');
  selectedStatus = signal<string>('');

  jobInvitationSummary = this.jobInvitationSummaryService.jobInvitationSummary;
  paginationMetadata = this.jobInvitationSummaryService.paginationMetadata;

  pagedInvitation = computed(() => {
    return this.jobInvitationSummary();
  });

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  totalPages = computed(() => this.paginationMetadata()?.totalPages || 0);

  ngOnInit(): void {
    this.jobInvitationSummaryService.loadLookups();
    this.loadSummaries();
  }

  loadSummaries() {
    const searchFilters: JobSummaryFilters =  {
      jobCategoryId: this.selectedCategory() || '',
      departmentId: this.selectedDepartment() || '',
      jobStatusId: this.selectedStatus() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'title',
      sortDirection: 'asc'
    }

    this.jobInvitationSummaryService.getInvitationSummaries(searchFilters);
  }

// when a filter changes
  onFilterChange() {
    this.loadSummaries();
  }

  clearFilters(): void {
    this.selectedCategory.set('');
    this.selectedDepartment.set('');
    this.selectedStatus.set('');
    this.loadSummaries();
  }

  getTypeLabel(type: keyof typeof this.TYPE_LABEL): string {
    const currentLang = this.translate.currentLang;
    return this.TYPE_LABEL[type][currentLang as 'en' | 'ar'] || type;
  }

  getStatusClasses(status: string): string[] {
    const map: Record<string, string[]> = {
      Active: ['bg-success-subtle', 'text-success'],
      Closed: ['bg-secondary-subtle', 'text-secondary'],
      Draft: ['bg-warning-subtle', 'text-warning'],
      Cancelled: ['bg-danger-subtle', 'text-danger']
    };

    // fallback for future statuses
    return map[status] ?? ['bg-light', 'text-dark'];
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadSummaries();
  }
}
