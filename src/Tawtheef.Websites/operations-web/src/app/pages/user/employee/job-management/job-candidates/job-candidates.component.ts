import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { NotificationService } from '../../../../../core/services/notification.service';
import { DialogHelperService } from '../../../../../core/services/dialog-helper.service';
import { FileUtilsService } from '../../../../../core/utils/file-utils';
import { JobLookupService } from '../services/job-lookup.service';
import { JobCandidatesService } from '../services/job-candidates.service';
import { GUID } from '../../../../../shared/types/guid.type';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { JobCandidateListItem } from '../models/job-candidate.model';
import { JobCandidatesFilter } from '../models/job-candidates-filter.model';
import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { JobResponse } from '../models/job-response-model';
import { JobService } from '../services/job.service';
import { JobCandidatesNationalityFilterModalComponent } from '../modals/job-candidates-nationality-filter-modal/job-candidates-nationality-filter-modal.component';
import { DialogService } from 'primeng/dynamicdialog';
import {
  JobCandidateNationalityPercentage,
  JobCandidatesFilterSettings,
  JobCandidateTypePercentage,
} from '../models/job-candidates-filter-settings.model';
import { JobCandidateProfileDialogComponent } from '../modals/job-candidate-profile-dialog/job-candidate-profile.dialog.component';
import { Permissions } from '../../../../../core/constants/permissions';
import { AuthService } from '../../../../../core/auth/auth.service';
import { JobCandidatesResponse } from '../models/job-candidates-response';
import { JobCandidatesSpecializationFilterModalComponent } from '../modals/job-candidates-specialization-filter-modal/job-candidates-specialization-filter-modal.component';
import { CandidateEligibilityCheckDialogComponent } from '../modals/candidate-eligibility-check-dialog/candidate-eligibility-check-dialog.component';
import { CandidatePointsBreakdownDialogComponent } from '../modals/candidate-points-breakdown-dialog/candidate-points-breakdown-dialog.component';
import { SystemRoles } from '../../../../../core/constants/systemRoles';

@Component({
  selector: 'app-job-candidates.component',
  standalone: false,
  templateUrl: './job-candidates.component.html',
  styleUrl: './job-candidates.component.scss',
})
export class JobCandidatesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private jobCandidatesService = inject(JobCandidatesService);
  private jobService = inject(JobService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);
  private dialogHelperService = inject(DialogHelperService);
  private fileUtilsService = inject(FileUtilsService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  lookupsService = inject(JobLookupService);

  paginationMetadata: PaginationMetadata | undefined;
  isLoading = signal(false);

  jobId!: GUID;
  jobInfo?: JobResponse;

  totalCandidatesCount = 0;
  availableCandidatesCount = 0;
  abovePointsCandidatesCount = 0;
  pointsAverage = 0;
  currentPage = signal(1);
  itemsPerPage = signal(10);

  candidates: PaginatedResult<JobCandidateListItem> | undefined;
  selectedCandidates: JobCandidateListItem[] = [];
  searchQuery = signal<string>('');
  filterGender = signal<GUID | null>(null);
  minimumPoints = signal<number | null>(null);

  candidateTypePercentages: JobCandidateTypePercentage[] = [];
  nationalityPercentages: JobCandidateNationalityPercentage[] = [];
  selectedSpecializations: { majorId: GUID; subMajorId: GUID }[] = [];
  filterSettingsLoaded = false;

  get activeSpecializationsCount(): number {
    return this.selectedSpecializations.length;
  }

  get activeNationalitiesCount(): number {
    return this.nationalityPercentages.length;
  }

  get hasActiveFilters(): boolean {
    return !!this.searchQuery() || !!this.minimumPoints() || !!this.filterGender() || this.selectedSpecializations.length > 0 || this.nationalityPercentages.length > 0;
  }

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('id') as GUID;
    this.jobService.getById(this.jobId).subscribe((job) => {
      if (!job) return;
      this.jobInfo = job;
      this.lookupsService.loadGenders().subscribe();
      this.lookupsService.loadNationalities().subscribe();
      this.loadFilterSettings();
    });
  }

  private loadFilterSettings(): void {
    this.jobCandidatesService.getFilterSettings(this.jobId).subscribe({
      next: (settings) => {
        this.filterGender.set(settings.genderId ?? null);
        this.minimumPoints.set(settings.minimumPoints ?? null);
        this.candidateTypePercentages = settings.candidateTypePercentages ?? [];
        this.nationalityPercentages = settings.nationalityPercentages ?? [];
        this.selectedSpecializations = settings.selectedSpecializations ?? [];
        this.filterSettingsLoaded = true;
        this.loadCandidatesData();
      },
      error: () => {
        this.filterSettingsLoaded = true;
      },
    });
  }

  private loadCandidatesData(): void {
    if (!this.filterSettingsLoaded) return;
    this.isLoading.set(true);

    const filter = this.buildFilter();
    const pagination = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };

    this.jobCandidatesService
      .getCandidates(this.jobId, pagination as PaginatedRequest, filter)
      .subscribe({
        next: (res: JobCandidatesResponse) => {
          const { overview, list } = res;

          this.totalCandidatesCount = overview.totalCandidatesCount;
          this.availableCandidatesCount = overview.availableCandidatesCount;
          this.abovePointsCandidatesCount = overview.abovePointsCandidatesCount;
          this.pointsAverage = overview.pointsAverage;

          this.candidates = list;
          this.paginationMetadata = list.metadata;
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false),
      });
  }

  private buildFilter(): JobCandidatesFilter {
    return {
      searchTerm: this.searchQuery() || undefined,
      minimumPoints: this.minimumPoints() || undefined,
      genderId: this.filterGender() || undefined,
      selectedSpecializations: this.selectedSpecializations.length > 0 ? this.selectedSpecializations : undefined,
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
    this.filterGender.set(null);
    this.minimumPoints.set(null);
    this.currentPage.set(1);
    this.resetSelection();
    this.loadCandidatesData();
  }

  refresh() {
    this.loadFilterSettings();
  }

  viewDetails(candidateId: GUID) {
    if (!this.canViewJobs()) return;
    this.dialogService.open(JobCandidateProfileDialogComponent, {
      data: {
        jobId: this.jobId,
        candidateId,
      },
      header: this.translationService.instant('JOB_CANDIDATE_PROFILE_TITLE'),
      modal: true,
      closable: true,
      dismissableMask: true,
      width: 'min(960px, 95vw)',
      draggable: false,   // ✅ disables dragging
      contentStyle: { 'max-height': '85vh', overflow: 'auto' },
    });
  }

  viewPointsBreakdown(candidate: JobCandidateListItem) {
    if (!this.canViewJobs()) return;
    this.dialogService.open(CandidatePointsBreakdownDialogComponent, {
      data: {
        jobId: this.jobId,
        candidateId: candidate.candidateId,
        candidateName: candidate.candidateName,
      },
      header: this.translationService.instant('CANDIDATE_POINTS_BREAKDOWN_TITLE'),
      modal: true,
      closable: true,
      dismissableMask: true,
      draggable: false,
      width: 'min(1100px, 95vw)',
      contentStyle: { 'max-height': '85vh', overflow: 'auto' },
    });
  }

  openEligibilityCheckDialog(): void {
    if (!this.canViewJobs()) return;
    this.dialogService.open(CandidateEligibilityCheckDialogComponent, {
      data: {
        jobId: this.jobId
      },
      header: this.translationService.instant('jobs.candidateEligibility.title'),
      modal: true,
      closable: true,
      dismissableMask: true,
      width: 'min(1100px, 95vw)',
      draggable: false,
      contentStyle: { 'max-height': '85vh', overflow: 'auto' },
    });
  }

  exportToExcel() {
    if (!this.canManageJobs()) return;
    const invitationIds =
      this.selectedCandidates.length > 0
        ? this.selectedCandidates.map((candidate) => candidate.candidateId)
        : undefined;

    this.jobCandidatesService
      .export({
        jobId: this.jobId,
        filter: this.buildFilter(),
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

      const applicantIds =
        this.selectedCandidates.length > 0
          ? this.selectedCandidates.map((candidate) => candidate.candidateId)
          : undefined;

      this.jobCandidatesService
        .sendInvitations({
          jobId: this.jobId,
          filter: this.buildFilter(),
          applicantIds,
        })
        .subscribe({
          next: () => {
            this.notificationService.success(
              this.translationService.instant('JOB_CANDIDATE_MESSAGES_INVITES_SENT')
            );
            this.loadCandidatesData();
          },
        });
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.resetSelection();
    this.loadCandidatesData();
  }

  onPageSizeChange(size: number) {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.resetSelection();
    this.loadCandidatesData();
  }

  openNationalityFilter(): void {
    const ref = this.dialogService.open(JobCandidatesNationalityFilterModalComponent, {
      header: this.translationService.instant('JOB_CANDIDATE_FILTERS_PERCENTAGE_TITLE'),
      width: '60vw',
      draggable: false,   // ✅ disables dragging
      data: {
        candidateTypePercentages: this.candidateTypePercentages,
        nationalityPercentages: this.nationalityPercentages,
      },
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.candidateTypePercentages = result.candidateTypePercentages ?? [];
      this.nationalityPercentages = result.nationalityPercentages ?? [];
      this.saveFilterSettings();
    });
  }

  openSpecializationFilter(): void {
    const ref = this.dialogService.open(JobCandidatesSpecializationFilterModalComponent, {
      header: this.translationService.instant('JOB_CANDIDATE_FILTERS_SPECIALIZATION_TITLE'),
      width: '40rem',
      draggable: false,   // ✅ disables dragging
      data: {
        mainMajor: this.jobInfo?.major,
        mainSubMajor: this.jobInfo?.subMajor,
        specializations: this.jobInfo?.jobSpecializations,
        selectedSpecializations: this.selectedSpecializations,
      },
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.selectedSpecializations = result;
      this.saveFilterSettings();
    });
  }

  private saveFilterSettings(): void {
    const payload = this.buildFilterSettings();
    this.jobCandidatesService.saveFilterSettings(payload).subscribe({
      next: (settings) => {
        this.candidateTypePercentages = settings.candidateTypePercentages ?? [];
        this.nationalityPercentages = settings.nationalityPercentages ?? [];
        this.filterGender.set(settings.genderId ?? null);
        this.minimumPoints.set(settings.minimumPoints ?? null);
        this.loadCandidatesData();
      },
      error: () => {
        this.notificationService.error(
          this.translationService.instant('JOB_CANDIDATE_MESSAGES_FILTER_SAVE_FAILED')
        );
      },
    });
  }

  applyFilters() {
    this.currentPage.set(1);
    this.resetSelection();
    this.saveFilterSettings();
  }

  private buildFilterSettings(): JobCandidatesFilterSettings {
    return {
      jobId: this.jobId,
      genderId: this.filterGender() || undefined,
      minimumPoints: this.minimumPoints() ?? undefined,
      candidateTypePercentages: this.candidateTypePercentages.filter((item) => item.percentage > 0),
      nationalityPercentages: this.nationalityPercentages.filter((item) => item.percentage > 0),
      selectedSpecializations: this.selectedSpecializations,
    };
  }

  onFilterSettingsChange() {
    this.currentPage.set(1);
    this.resetSelection();
    this.saveFilterSettings();
  }

  canSelectCandidates(): boolean {
    return this.authService.hasRole(SystemRoles.HrManager);
  }

  private extractFileName(response: {
    headers: { get(name: string): string | null };
  }): string | null {
    const contentDisposition = response.headers.get('content-disposition') || '';
    const match = /filename[*]?=(?:UTF-8''|\"|')?([^;\"']+)/i.exec(contentDisposition);
    if (!match?.[1]) return null;
    return decodeURIComponent(match[1].replace(/\"/g, ''));
  }

  canManageJobs(): boolean {
    return this.authService.hasRole(SystemRoles.HrManager);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Edit, Permissions.Jobs.View]);
  }
}
