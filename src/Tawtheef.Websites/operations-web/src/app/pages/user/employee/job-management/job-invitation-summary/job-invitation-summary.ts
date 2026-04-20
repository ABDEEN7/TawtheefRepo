import { Component, DestroyRef, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { JobInvitationSummaryModel, JobSummaryFilters } from '../models/job-invitation-summary.model';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { Select } from 'primeng/select';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import { JobInvitationSummaryService } from '../services/job-invitation-summary.service';
import { routes } from '../../../../../routes/routes';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StatusBadgeComponent } from '../../../../../shared/components/status-badge/status-badge.component';
import { PipelineBarComponent, PipelineSegment } from '../../../../../shared/components/pipeline-bar/pipeline-bar.component';
import { MetricChipComponent } from '../../../../../shared/components/metric-chip/metric-chip.component';
import { DialogService } from 'primeng/dynamicdialog';
import { PipelineChartDialogComponent, PipelineChartData } from './pipeline-chart-dialog/pipeline-chart-dialog.component';
import { Menu } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { Tooltip } from 'primeng/tooltip';
import { Lang, LanguageService } from '../../../../../core/services/language.service';

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
    FaDirArrowDirective,
    StatusBadgeComponent,
    PipelineBarComponent,
    MetricChipComponent,
    Menu,
    Tooltip
  ],
  providers: [DialogService]
})
export class JobInvitationSummary implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  private dialogService = inject(DialogService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  jobInvitationSummaryService = inject(JobInvitationSummaryService);
  private searchChanges$ = new Subject<string>();

  currentLang = signal<Lang>(this.language.get());
  // Signals
  currentPage = signal(1);
  itemsPerPage = signal(10);
  selectedCategory = signal<string>('');
  selectedDepartment = signal<string>('');
  selectedStatus = signal<string>('');
  searchText = signal<string>('');

  // Sorting
  sortColumn = signal<string>('CreateDate');
  sortDirection = signal<'asc' | 'desc'>('desc');

  // Expandable rows
  expandedRows = signal<Set<string>>(new Set());

  // Context menu
  activeActions: MenuItem[] = [];

  jobInvitationSummary = this.jobInvitationSummaryService.jobInvitationSummary;
  paginationMetadata = this.jobInvitationSummaryService.paginationMetadata;

  pagedInvitation = computed(() => this.jobInvitationSummary());

  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  ngOnInit(): void {
    this.setupSearchListener();
    this.jobInvitationSummaryService.loadLookups();
    this.loadSummaries();
  }

  loadSummaries() {
    const filters: JobSummaryFilters = {
      jobCategoryId: this.selectedCategory() || '',
      departmentId: this.selectedDepartment() || '',
      jobStatusId: this.selectedStatus() || '',
      search: this.searchText() || '',
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: this.sortColumn(),
      sortDirection: this.sortDirection()
    };

    this.jobInvitationSummaryService.getInvitationSummaries(filters);
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadSummaries();
  }

  onSearchChange(search: string) {
    this.searchText.set(search ?? '');
    this.searchChanges$.next(this.searchText());
  }

  clearFilters(): void {
    this.searchText.set('');
    this.selectedCategory.set('');
    this.selectedDepartment.set('');
    this.selectedStatus.set('');
    this.currentPage.set(1);
    this.loadSummaries();
  }

  // ─── Sorting ───────────────────────────────────────

  onSort(column: string): void {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
    this.loadSummaries();
  }

  getSortIcon(column: string): string {
    if (this.sortColumn() !== column) return 'pi pi-sort-alt';
    return this.sortDirection() === 'asc' ? 'pi pi-sort-amount-up' : 'pi pi-sort-amount-down';
  }

  // ─── Expandable Rows ──────────────────────────────

  toggleRow(jobId: string): void {
    this.expandedRows.update(set => {
      const newSet = new Set(set);
      if (newSet.has(jobId)) {
        newSet.delete(jobId);
      } else {
        newSet.add(jobId);
      }
      return newSet;
    });
  }

  isExpanded(jobId: string): boolean {
    return this.expandedRows().has(jobId);
  }

  // ─── Pipeline Bar ─────────────────────────────────

  getPipelineSegments(summary: JobInvitationSummaryModel): PipelineSegment[] {
    return [
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.INVITATIONS'), value: summary.invitationCount, color: '#4e80ea' },
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.APPLICANTS'), value: summary.applicantsCount, color: '#2b9d76' },
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.READ'), value: summary.readCount, color: '#f5b342' },
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.DECLINED'), value: summary.refusedCount, color: '#df6d4e' },
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.PENDING_ATTACHMENT'), value: summary.pendingAttachmentApprovalCount, color: '#e67e22' },
      { label: this.translate.instant('JOB_INVITATION_SUMMARY.RETURNED_ATTACHMENT'), value: summary.returnedAttachmentCount, color: '#c0392b' },
    ];
  }

  // ─── Chart Dialog ─────────────────────────────────

  openChartDialog(summary: JobInvitationSummaryModel): void {
    const data: PipelineChartData = {
      jobName: summary.jobName,
      metrics: [
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.INVITATIONS'), value: summary.invitationCount, color: '#4e80ea' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.APPLICANTS'), value: summary.applicantsCount, color: '#2b9d76' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.READ'), value: summary.readCount, color: '#f5b342' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.DECLINED'), value: summary.refusedCount, color: '#df6d4e' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.UNSEEN'), value: summary.notSeenCount, color: '#6c757d' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.EXPIRED'), value: summary.expiredCount, color: '#9a6bff' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.CANCELLED'), value: summary.cancelledCount, color: '#343a40' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.PENDING_ATTACHMENT'), value: summary.pendingAttachmentApprovalCount ?? 0, color: '#e67e22' },
        { label: this.translate.instant('JOB_INVITATION_SUMMARY.RETURNED_ATTACHMENT'), value: summary.returnedAttachmentCount ?? 0, color: '#c0392b' },
      ]
    };

    this.dialogService.open(PipelineChartDialogComponent, {
      header: summary.jobName + ' — ' + this.translate.instant('JOB_INVITATION_SUMMARY.PIPELINE_CHART'),
      width: '720px',
      data,
      closable: true,
      closeOnEscape: true,
      dismissableMask: true,
      styleClass: 'pipeline-chart-dialog'
    });
  }

  // ─── Context Menu ─────────────────────────────────

  showContextMenu(event: Event, summary: JobInvitationSummaryModel, menu: Menu): void {
    this.activeActions = this.getContextActions(summary);
    menu.toggle(event);
  }

  private getContextActions(summary: JobInvitationSummaryModel): MenuItem[] {
    const actions: MenuItem[] = [];

    if (this.canViewInvitations()) {
      actions.push({
        label: this.translate.instant('JOB_INVITATION_SUMMARY.VIEW_DETAILS'),
        icon: 'hgi hgi-stroke hgi-view',
        command: () => this.router.navigate([routes.portal.jobInvitationSummaryDetails(summary.jobId)])
      });
    }

    actions.push({
      label: this.translate.instant('JOB_INVITATION_SUMMARY.VIEW_CHART'),
      icon: 'pi pi-chart-bar',
      command: () => this.openChartDialog(summary)
    });

    return actions;
  }

  // ─── Pagination ───────────────────────────────────

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
    this.router.navigate([routes.portal.dashboard]);
  }

  canViewInvitations(): boolean {
    return this.authService.hasPermission(Permissions.JobInvitations.View);
  }

  trackByJobId(_index: number, item: JobInvitationSummaryModel): string {
    return item.jobId;
  }

  private setupSearchListener() {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.currentPage.set(1);
        this.loadSummaries();
      });
  }

  protected readonly routes = routes;
}
