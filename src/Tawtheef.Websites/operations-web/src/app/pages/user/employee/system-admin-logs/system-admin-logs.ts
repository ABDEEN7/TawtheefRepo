import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { DatePicker } from 'primeng/datepicker';
import { Dialog } from 'primeng/dialog';
import { SystemAdminLogsService } from './services/system-admin-logs.service';
import { SystemAdminLogDto } from './models/system-admin-log.dto';
import { SystemAdminLogFilters } from './models/system-admin-log-filters.dto';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { UsersService } from '../users-management/services/users.service';

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
    DatePicker,
    Dialog,
  ]
})
export class SystemAdminLogsComponent implements OnInit {
  private systemAdminLogsService = inject(SystemAdminLogsService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  private _logs = signal<SystemAdminLogDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _userOptions = signal<{ id: string; label: string }[]>([]);

  logs = this._logs.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();
  userOptions = this._userOptions.asReadonly();

  filters = signal<SystemAdminLogFilters>({
    pageNumber: 1,
    pageSize: 10,
  });

  userIdFilter: string | null = null;
  actionTypeFilter = '';
  searchFilter = '';
  fromDate: Date | null = null;
  toDate: Date | null = null;

  selectedLogDetails: string | null = null;
  detailsDialogVisible = false;

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  ngOnInit(): void {
    this.loadUserOptions();
    this.loadLogs();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUserOptions() {
    this.systemAdminLogsService.getUsersLookup().subscribe({
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
    const updatedFilters: SystemAdminLogFilters = {
      ...this.filters(),
      userId: this.userIdFilter || null,
      actionType: this.actionTypeFilter || null,
      source: 'ActionLog',
      reviewStatus: null,
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
    this.userIdFilter = null;
    this.actionTypeFilter = '';
    this.searchFilter = '';
    this.fromDate = null;
    this.toDate = null;
    this.onFiltersChanged();
  }

  viewDetails(log: any) {
    if (!log.notes) return;
    
    try {
      const parts = log.notes.split('| Details:');
      const humanAndChanges = parts[0].trim();
      const jsonString = parts.length > 1 ? parts[1].trim() : null;

      const changeParts = humanAndChanges.split('--- Changes ---');
      const humanNote = changeParts[0].trim();
      const changes = changeParts.length > 1 ? changeParts[1].trim() : null;

      let formattedText = '';
      if (humanNote) formattedText += `### ${humanNote}\n\n`;
      if (changes) formattedText += `**Changes:**\n${changes}\n\n`;
      
      if (jsonString) {
        try {
          const parsed = JSON.parse(jsonString);
          formattedText += `**Full Payload:**\n${this.formatAllFields(parsed)}`;
        } catch {
          formattedText += `**Payload:**\n${jsonString}`;
        }
      }

      this.selectedLogDetails = formattedText || log.notes;
    } catch {
      this.selectedLogDetails = log.notes;
    }
    this.detailsDialogVisible = true;
  }

  private formatAllFields(obj: any): string {
    const results: string[] = [];
    this.extractReadableFields(obj, results);
    return results.length > 0 ? results.join('\n') : '-';
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

  translateAction(log: any): string {
    if (!log.actionType) return '-';

    const isProfileLog = log.source === 'UserProfileLogger';
    const prefix = isProfileLog ? 'PROFILE_LOGS.ACTIONS.' : 'ADMIN_ACTIONS.';
    
    const key = prefix + log.actionType;
    const translated = this.translate.instant(key);

    let finalAction = '';
    if (translated !== key) {
      finalAction = translated;
    } else {
      // Fallback 1: Try just the action part (split by dot for admin actions)
      const parts = log.actionType.split('.');
      const actionPart = parts[parts.length - 1];
      const fallbackKey = prefix + actionPart;
      const fallbackTranslated = this.translate.instant(fallbackKey);

      if (fallbackTranslated !== fallbackKey) {
        finalAction = fallbackTranslated;
      } else {
        // Fallback 2: Format the raw action name
        finalAction = actionPart
          .replace(/([A-Z])/g, ' $1')
          .replace(/^./, (str: string) => str.toUpperCase())
          .trim();
      }
    }

    // Append target name if found in notes (only for admin actions, profile logs already have it in summary)
    if (!isProfileLog && log.notes) {
      try {
        const parsed = JSON.parse(log.notes);
        const target = this.extractPrimaryIdentifier(parsed);
        if (target) {
          return `${finalAction}: ${target}`;
        }
      } catch { }
    }

    return finalAction;
  }

  translateSection(section?: string | null): string {
    if (!section) return '-';

    const key = 'ADMIN_SECTIONS.' + section;
    const translated = this.translate.instant(key);

    if (translated !== key) return translated;

    // Fallback: Format the raw section name
    return section
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, (str) => str.toUpperCase())
      .trim();
  }

  formatNotes(log: any): string {
    const summaryParts: string[] = [];

    // 1. Include Profile Owner if available
    if (log.userProfileOwnerName) {
      summaryParts.push(log.userProfileOwnerName);
    }

    // 2. Extract from Notes field
    if (log.notes) {
      // Check for the enriched format: "Human Note | Details: JSON"
      const parts = log.notes.split('| Details:');
      const humanPart = parts[0].trim();
      const jsonPart = parts.length > 1 ? parts[1].trim() : null;

      // If we have a human-readable part from the backend, use it
      if (humanPart && !humanPart.startsWith('{') && !humanPart.startsWith('[')) {
        if (!summaryParts.includes(humanPart)) {
          summaryParts.push(humanPart);
        }
      } else {
        // Fallback to old JSON parsing logic for older logs
        try {
          const parsed = JSON.parse(log.notes);
          const jsonIdentifier = this.extractPrimaryIdentifier(parsed);
          if (jsonIdentifier && !summaryParts.includes(jsonIdentifier)) {
            summaryParts.push(jsonIdentifier);
          }
        } catch {
          // Final fallback: show raw note if it's not a huge JSON
          if (log.notes.length < 200) {
            summaryParts.push(log.notes);
          }
        }
      }
    }

    return summaryParts.length > 0 ? summaryParts.join(' - ') : '-';
  }

  private extractPrimaryIdentifier(obj: any): string | null {
    if (!obj || typeof obj !== 'object') return null;

    // Prioritized list of keys that represent a "main name"
    const primaryKeys = [
      'nameEn', 'fullNameEn', 'adminNameEn', 'titleEn', 'questionEn', 'email',
      'nameAr', 'fullNameAr', 'adminNameAr', 'titleAr', 'questionAr', 'code'
    ];

    // Check current level
    for (const key of primaryKeys) {
      if (obj[key] && typeof obj[key] !== 'object' && String(obj[key]).length > 0) {
        return String(obj[key]);
      }
    }

    // Check nested objects (like 'command')
    for (const value of Object.values(obj)) {
      if (value && typeof value === 'object' && !Array.isArray(value)) {
        const found = this.extractPrimaryIdentifier(value);
        if (found) return found;
      }
    }

    return null;
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
