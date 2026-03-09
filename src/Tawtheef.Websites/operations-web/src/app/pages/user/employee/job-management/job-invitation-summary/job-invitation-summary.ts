import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { JobSummaryFilters } from '../models/job-invitation-summary.model';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { Select } from 'primeng/select';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import { JobInvitationSummaryService } from '../services/job-invitation-summary.service';
import { routes } from '../../../../../routes/routes';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
@Component({
  selector: 'app-job-invitation-summary',
  templateUrl: './job-invitation-summary.html',
  styleUrls: ['./job-invitation-summary.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    PaginationComponent,
    FaDirArrowDirective
  ]
})
export class JobInvitationSummary implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);

  jobInvitationSummaryService = inject(JobInvitationSummaryService);

  // Signals
  currentPage = signal(1);
  itemsPerPage = signal(10);
  selectedCategory = signal<string>('');
  selectedDepartment = signal<string>('');
  selectedStatus = signal<string>('');
  searchText = signal<string>('');

  jobInvitationSummary = this.jobInvitationSummaryService.jobInvitationSummary;
  paginationMetadata = this.jobInvitationSummaryService.paginationMetadata;

  pagedInvitation = computed(() => this.jobInvitationSummary());

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  ngOnInit(): void {
    this.jobInvitationSummaryService.loadLookups();
    this.loadSummaries();
  }

  loadSummaries() {
    const filters: JobSummaryFilters = {
      jobCategoryId: this.selectedCategory() || '',
      departmentId: this.selectedDepartment() || '',
      jobStatusId: this.selectedStatus() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'title',
      sortDirection: 'asc'
    };

    this.jobInvitationSummaryService.getInvitationSummaries(filters);
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadSummaries();
  }

  clearFilters(): void {
    this.searchText.set('');
    this.selectedCategory.set('');
    this.selectedDepartment.set('');
    this.selectedStatus.set('');
    this.currentPage.set(1);
    this.loadSummaries();
  }

  getStatusPillClass(backendName: string): string {
    const key = (backendName || '').toLowerCase();

    if (key.includes('appl') || key.includes('submitted')) return 'status-applied';
    if (key.includes('new') || key.includes('invite')) return 'status-new';
    if (key.includes('cancel') || key.includes('closed')) return 'status-cancelled';

    return 'status-default';
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadSummaries();
  }

  onPageSizeChange(size: number) {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.loadSummaries();
  }

  navigateTo() {
    this.router.navigate([routes.employee.dashboard]);
  }

  canViewInvitations(): boolean {
    return this.authService.hasPermission(Permissions.JobInvitations.View);
  }

  protected readonly routes = routes;
}
