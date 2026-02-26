import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {SystemAdminLogsService} from './services/system-admin-logs.service';
import {SystemAdminLogDto} from './models/system-admin-log.dto';
import {SystemAdminLogFilters} from './models/system-admin-log-filters.dto';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';
import {ReviewStatus} from '../../employee/profile-managment/approval-list/models/profile-approval.models';

@Component({
  selector: 'app-system-admin-logs',
  standalone: true,
  templateUrl: './system-admin-logs.html',
  styleUrls: ['./system-admin-logs.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
  ]
})
export class SystemAdminLogsComponent implements OnInit {
  private systemAdminLogsService = inject(SystemAdminLogsService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _logs = signal<SystemAdminLogDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);

  logs = this._logs.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();

  filters = signal<SystemAdminLogFilters>({
    pageNumber: 1,
    pageSize: 10,
  });

  profileIdFilter = '';
  userIdFilter = '';
  actionTypeFilter = '';
  searchFilter = '';
  reviewStatusFilter: ReviewStatus | null = null;
  sourceFilter: string | null = 'ActionLog';
  fromDate: Date | null = null;
  toDate: Date | null = null;

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  sourceOptions = [
    { id: 'ActionLog', label: 'PROFILE_LOGS.SOURCE_AUDIT_TRAIL' }
  ];

  reviewStatusOptions = [
    { id: ReviewStatus.NotReviewed, label: 'PROFILE_LOGS.REVIEW_STATUS.NOT_REVIEWED' },
    { id: ReviewStatus.Pending, label: 'PROFILE_LOGS.REVIEW_STATUS.PENDING' },
    { id: ReviewStatus.Approved, label: 'PROFILE_LOGS.REVIEW_STATUS.APPROVED' },
    { id: ReviewStatus.Rejected, label: 'PROFILE_LOGS.REVIEW_STATUS.REJECTED' },
    { id: ReviewStatus.NeedsCorrection, label: 'PROFILE_LOGS.REVIEW_STATUS.NEEDS_CORRECTION' },
  ];

  ngOnInit(): void {
    this.loadLogs();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadLogs() {
    const updatedFilters: SystemAdminLogFilters = {
      ...this.filters(),
      userProfileId: this.profileIdFilter || null,
      userId: this.userIdFilter || null,
      actionType: this.actionTypeFilter || null,
      source: this.sourceFilter,
      reviewStatus: this.reviewStatusFilter,
      from: this.fromDate ? this.fromDate.toISOString() : null,
      to: this.toDate ? this.toDate.toISOString() : null,
      search: this.searchFilter || null,
    };

    this.filters.set(updatedFilters);

    this.systemAdminLogsService.getLogs(this.filters()).subscribe({
      next: (response: PaginatedResult<SystemAdminLogDto>) => {
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

  onFromDateChange(value: string) {
    this.fromDate = value ? new Date(value) : null;
    this.onFiltersChanged();
  }

  onToDateChange(value: string) {
    this.toDate = value ? new Date(value) : null;
    this.onFiltersChanged();
  }

  sourceLabel(source: string) {
    switch (source) {
      case 'UserProfileLogger':
        return this.translate.instant('PROFILE_LOGS.SOURCE_USER_PROFILE');
      case 'ActionLog':
        return this.translate.instant('PROFILE_LOGS.SOURCE_AUDIT_TRAIL');
      default:
        return source;
    }
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

  toDateInputValue(date: Date | null) {
    if (!date) {
      return '';
    }

    const pad = (value: number) => value.toString().padStart(2, '0');

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}` +
      `T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}
