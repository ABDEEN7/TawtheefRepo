import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { NotificationService } from '../../../core/services/notification.service';
import { DialogHelperService } from '../../../core/services/dialog-helper.service';
import { FileUtilsService } from '../../../core/utils/file-utils';
import { JobLookupService } from '../services/job-lookup.service';
import { JobCandidatesService } from '../services/job-candidates.service';
import { GUID } from '../../../shared/types/guid.type';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { JobCandidateListItem } from '../models/job-candidate.model';
import { JobCandidatesFilter } from '../models/job-candidates-filter.model';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { AuthService } from '../../../core/auth/auth.service';
import { Permissions } from '../../../core/constants/permissions';

@Component({
  selector: 'app-job-candidates.component',
  standalone: false,
  templateUrl: './job-candidates.component.html',
  styleUrl: './job-candidates.component.scss',
})
export class JobCandidatesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private jobCandidatesService = inject(JobCandidatesService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);
  private dialogHelperService = inject(DialogHelperService);
  private fileUtilsService = inject(FileUtilsService);
  private authService = inject(AuthService);
  lookupsService = inject(JobLookupService);

  paginationMetadata: PaginationMetadata | undefined;

  jobId!: GUID;

  totalCandidatesCount = 0;
  availableCandidatesCount = 0;
  abovePointsCandidatesCount = 0;
  pointsAverage = 0;
  currentPage = signal(1);
  itemsPerPage = 10;

  candidates: PaginatedResult<JobCandidateListItem> | undefined;
  selectedCandidates: JobCandidateListItem[] = [];
  searchQuery = signal<string>('');
  filterJobCategory = signal<GUID | null>(null);
  filterCandidateCategory = signal<GUID | null>(null);
  minimumPoints = signal<number | null>(null);

  minimumPointsOptions = [200, 400, 600, 800];

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('id') as GUID;
    this.lookupsService.loadJobCategories();
    this.lookupsService.loadCandidateTypes();
    this.loadCandidatesData();
  }

  private loadCandidatesData(): void {
    const filter = this.buildFilter();
    const pagination = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage,
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };

    forkJoin({
      overview: this.jobCandidatesService.getOverview(this.jobId, filter),
      list: this.jobCandidatesService.search(this.jobId, pagination as PaginatedRequest, filter),
    }).subscribe({
      next: ({ overview, list }) => {
        this.totalCandidatesCount = overview.totalCandidatesCount;
        this.availableCandidatesCount = overview.availableCandidatesCount;
        this.abovePointsCandidatesCount = overview.abovePointsCandidatesCount;
        this.pointsAverage = overview.pointsAverage;

        this.candidates = list;
        this.paginationMetadata = list.metadata;
      },
    });
  }

  private buildFilter(): JobCandidatesFilter {
    return {
      searchTerm: this.searchQuery() || undefined,
      jobCategoryId: this.filterJobCategory() || undefined,
      candidateTypeId: this.filterCandidateCategory() || undefined,
      minimumPoints: this.minimumPoints() || undefined,
    };
  }

  private resetSelection(): void {
    this.selectedCandidates = [];
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.resetSelection();
    this.loadCandidatesData();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterJobCategory.set(null);
    this.filterCandidateCategory.set(null);
    this.minimumPoints.set(null);
    this.currentPage.set(1);
    this.resetSelection();
    this.loadCandidatesData();
  }

  viewDetails(candidateId: GUID) {
    if (!this.canViewJobs()) return;
    void candidateId;
  }

  exportToExcel() {
    if (!this.canManageJobs()) return;
    const invitationIds =
      this.selectedCandidates.length > 0
        ? this.selectedCandidates.map((candidate) => candidate.invitationId)
        : undefined;

    this.jobCandidatesService
      .export({
        jobId: this.jobId,
        filter: this.buildFilter(),
        invitationIds,
      })
      .subscribe({
        next: (response) => {
          const fileName = this.extractFileName(response) ?? 'job-candidates.csv';
          const blob = response.body ?? new Blob();
          this.fileUtilsService.downloadBlob(blob, fileName);
          this.notificationService.success(
            this.translationService.instant('JOB_CANDIDATE_MESSAGES_EXPORT_SUCCESS')
          );
        },
        error: () => {
          this.notificationService.error(
            this.translationService.instant('JOB_CANDIDATE_MESSAGES_EXPORT_FAILED')
          );
        },
      });
  }

  sendInvitations() {
    if (!this.canManageJobs()) return;
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_CANDIDATE_CONFIRMATIONS_SEND_INVITATIONS_TITLE',
      description: 'JOB_CANDIDATE_CONFIRMATIONS_SEND_INVITATIONS_DESCRIPTION',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;

      const invitationIds =
        this.selectedCandidates.length > 0
          ? this.selectedCandidates.map((candidate) => candidate.invitationId)
          : undefined;

      this.jobCandidatesService
        .sendInvitations({
          jobId: this.jobId,
          filter: this.buildFilter(),
          invitationIds,
        })
        .subscribe({
          next: () => {
            this.notificationService.success(
              this.translationService.instant('JOB_CANDIDATE_MESSAGES_INVITES_SENT')
            );
            this.loadCandidatesData();
          },
          error: () => {
            this.notificationService.error(
              this.translationService.instant('JOB_CANDIDATE_MESSAGES_INVITES_FAILED')
            );
          },
        });
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadCandidatesData();
  }

  private extractFileName(response: { headers: { get(name: string): string | null } }): string | null {
    const contentDisposition = response.headers.get('content-disposition') || '';
    const match = /filename[*]?=(?:UTF-8''|\"|')?([^;\"']+)/i.exec(contentDisposition);
    if (!match?.[1]) return null;
    return decodeURIComponent(match[1].replace(/\"/g, ''));
  }

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.Jobs.View]);
  }
}
