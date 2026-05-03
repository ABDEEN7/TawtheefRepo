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

  translateAction(log: any): string {
    if (!log.actionType) return '-';

    const key = 'PROFILE_LOGS.ACTIONS.' + log.actionType;
    const translated = this.translate.instant(key);

    let finalAction = '';
    if (translated !== key) {
      finalAction = translated;
    } else {
      // Fallback: Format the raw action name
      finalAction = log.actionType
        .replace(/([A-Z])/g, ' $1')
        .replace(/^./, (str: string) => str.toUpperCase())
        .trim();
    }

    // Append target name if found
    const target = this.extractPrimaryIdentifier(log);
    if (target) {
      return `${finalAction}: ${target}`;
    }

    return finalAction;
  }

  private extractPrimaryIdentifier(log: any): string | null {
    // 1. Check direct property
    if (log.userProfileOwnerName) return log.userProfileOwnerName;

    // 2. Check JSON notes
    if (log.notes) {
      try {
        const parsed = JSON.parse(log.notes);
        return this.findNameInObject(parsed);
      } catch { }
    }
    return null;
  }

  private findNameInObject(obj: any): string | null {
    if (!obj || typeof obj !== 'object') return null;
    const primaryKeys = ['nameEn', 'fullNameEn', 'adminNameEn', 'titleEn', 'nameAr', 'fullNameAr', 'email'];
    for (const key of primaryKeys) {
      if (obj[key] && typeof obj[key] !== 'object') return String(obj[key]);
    }
    for (const value of Object.values(obj)) {
      if (value && typeof value === 'object' && !Array.isArray(value)) {
        const found = this.findNameInObject(value);
        if (found) return found;
      }
    }
    return null;
  }

  translateSection(section?: string | null): string {
    if (!section) return '-';
    const key = 'PROFILE_LOGS.SECTIONS.' + section;
    const translated = this.translate.instant(key);
    if (translated !== key) return translated;
    return section.replace(/([A-Z])/g, ' $1').trim();
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

  formatNotes(log: any): string {
    if (!log.notes) return '-';

    // Handle the enriched format: "Human Note | Details: JSON"
    const parts = log.notes.split('| Details:');
    const humanAndChanges = parts[0].trim();
    const jsonPart = parts.length > 1 ? parts[1].trim() : null;

    // Split human note and changes
    const changeParts = humanAndChanges.split('--- Changes ---');
    const humanNote = changeParts[0].trim();
    const changes = changeParts.length > 1 ? changeParts[1].trim() : null;

    // If we have a human-readable part or changes, use them
    if (humanNote || changes) {
      let summary = humanNote;
      if (changes) {
        summary += (summary ? ' • ' : '') + changes.replace(/\n/g, ' • ');
      }
      return summary;
    }

    try {
      const jsonString = jsonPart || log.notes;
      const parsed = JSON.parse(jsonString);
      const results: string[] = [];
      this.extractReadableFields(parsed, results);
      return results.length > 0 ? results.join(' • ') : '-';
    } catch {
      return log.notes;
    }
  }

  private extractReadableFields(obj: any, results: string[]): void {
    if (!obj || typeof obj !== 'object') return;

    const displayKeys: Record<string, string> = {
      nameEn: 'Name (EN)',
      nameAr: 'Name (AR)',
      fullNameEn: 'Full Name (EN)',
      fullNameAr: 'Full Name (AR)',
      adminNameEn: 'Admin Name (EN)',
      adminNameAr: 'Admin Name (AR)',
      adminEmail: 'Admin Email',
      phoneNumber: 'Phone',
      phoneCountryCode: 'Phone Code',
      email: 'Email',
      isBlocked: 'Blocked',
      isActive: 'Active',
      status: 'Status',
      questionEn: 'Question (EN)',
      questionAr: 'Question (AR)',
      answerEn: 'Answer (EN)',
      answerAr: 'Answer (AR)',
      titleEn: 'Title (EN)',
      titleAr: 'Title (AR)',
      descriptionEn: 'Description (EN)',
      descriptionAr: 'Description (AR)',
      code: 'Code',
      value: 'Value',
      order: 'Sort Order',
      type: 'Type',
      roleIds: 'Roles',
      permissionKeys: 'Permissions'
    };

    const skipPatterns = /^(id|countryId|cityId|officeId|userId|userProfileId)$/i;
    const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

    for (const [key, value] of Object.entries(obj)) {
      if (skipPatterns.test(key)) continue;
      if (typeof value === 'string' && guidPattern.test(value)) continue;
      if (value === null || value === undefined) continue;

      if (typeof value === 'object' && !Array.isArray(value)) {
        this.extractReadableFields(value, results);
        continue;
      }

      let label = displayKeys[key];
      if (!label) {
        label = key
          .replace(/([A-Z])/g, ' $1')
          .replace(/^./, (str) => str.toUpperCase())
          .trim();
      }

      let displayVal = '';
      if (Array.isArray(value)) {
        const filteredArr = value.filter(v => typeof v !== 'string' || !guidPattern.test(v));
        if (filteredArr.length > 0) {
          displayVal = filteredArr.join(', ');
        } else if (value.length > 0) {
          displayVal = `(${value.length} items)`;
        }
      } else {
        displayVal = typeof value === 'boolean'
          ? (value ? 'Yes' : 'No')
          : String(value);
      }

      if (displayVal && displayVal !== 'null' && displayVal !== 'undefined') {
        results.push(`${label}: ${displayVal}`);
      }
    }
  }

}
