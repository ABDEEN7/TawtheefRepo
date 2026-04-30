import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { DatePicker } from 'primeng/datepicker';
import { ProfileLogsService } from './services/profile-logs.service';
import { ProfileLogDto } from './models/profile-log.dto';
import { ProfileLogFilters } from './models/profile-log-filters.dto';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { UsersService } from '../users-management/services/users.service';
import { ReviewStatus } from '../profile-managment/approval-list/models/profile-approval.models';

@Component({
  selector: 'app-profile-logs',
  standalone: true,
  templateUrl: './profile-logs.html',
  styleUrls: ['./profile-logs.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    DatePicker,
  ]
})
export class ProfileLogsComponent implements OnInit {
  private profileLogsService = inject(ProfileLogsService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _logs = signal<ProfileLogDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _userOptions = signal<{ id: string; label: string }[]>([]);

  logs = this._logs.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();
  userOptions = this._userOptions.asReadonly();

  filters = signal<ProfileLogFilters>({
    pageNumber: 1,
    pageSize: 10,
  });

  profileIdFilter = '';
  userIdFilter: string | null = null;
  actionTypeFilter = '';
  searchFilter = '';
  reviewStatusFilter: ReviewStatus | null = null;
  fromDate: Date | null = null;
  toDate: Date | null = null;

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  reviewStatusOptions = [
    { id: ReviewStatus.NotReviewed, label: 'PROFILE_LOGS.REVIEW_STATUS.NOT_REVIEWED' },
    { id: ReviewStatus.Pending, label: 'PROFILE_LOGS.REVIEW_STATUS.PENDING' },
    { id: ReviewStatus.Approved, label: 'PROFILE_LOGS.REVIEW_STATUS.APPROVED' },
    { id: ReviewStatus.Rejected, label: 'PROFILE_LOGS.REVIEW_STATUS.REJECTED' },
    { id: ReviewStatus.NeedsCorrection, label: 'PROFILE_LOGS.REVIEW_STATUS.NEEDS_CORRECTION' },
  ];

  ngOnInit(): void {
    this.loadUserOptions();
    this.loadLogs();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUserOptions() {
    this.profileLogsService.getUsersLookup().subscribe({
      next: (users) => {
        const options = (users || []).map(u => ({
          id: u.id,
          label: u.name
        }));
        this._userOptions.set(options.sort((a, b) => a.label.localeCompare(b.label)));
      }
    });
  }

  loadLogs() {
    const updatedFilters: ProfileLogFilters = {
      ...this.filters(),
      userProfileId: this.profileIdFilter.trim() || null,
      userId: this.userIdFilter || null,
      actionType: this.actionTypeFilter.trim() || null,
      source: 'UserProfileLogger',
      reviewStatus: this.reviewStatusFilter,
      from: this.fromDate ? this.fromDate.toISOString() : null,
      to: this.toDate ? this.toDate.toISOString() : null,
      search: this.searchFilter.trim() || null,
    };

    this.filters.set(updatedFilters);

    this.profileLogsService.getLogs(this.filters()).subscribe({
      next: (response: PaginatedResult<ProfileLogDto>) => {
        this._logs.set(response.items);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize,
          }));
        }
      }
    });
  }

  onFiltersChanged() {
    this.filters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadLogs();
  }

  onPageChange(page: number) {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadLogs();
  }

  onPageSizeChange(size: number) {
    this.filters.update(f => ({ ...f, pageSize: size, pageNumber: 1 }));
    this.loadLogs();
  }

  onFromDateChange(value: Date | null) {
    this.fromDate = value;
    this.onFiltersChanged();
  }

  onToDateChange(value: Date | null) {
    this.toDate = value;
    this.onFiltersChanged();
  }

  clearFilters() {
    this.profileIdFilter = '';
    this.userIdFilter = null;
    this.actionTypeFilter = '';
    this.searchFilter = '';
    this.reviewStatusFilter = null;
    this.fromDate = null;
    this.toDate = null;
    this.onFiltersChanged();
  }

  reviewStatusLabel(status: ReviewStatus | null | undefined) {
    if (status === null || status === undefined) {
      return '';
    }

    switch (status) {
      case ReviewStatus.NotReviewed:
        return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.NOT_REVIEWED');
      case ReviewStatus.Pending:
        return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.PENDING');
      case ReviewStatus.Approved:
        return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.APPROVED');
      case ReviewStatus.Rejected:
        return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.REJECTED');
      case ReviewStatus.NeedsCorrection:
        return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.NEEDS_CORRECTION');
      default:
        return '';
    }
  }

}
