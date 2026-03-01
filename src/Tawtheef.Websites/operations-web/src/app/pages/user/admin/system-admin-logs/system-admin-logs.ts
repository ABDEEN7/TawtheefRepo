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

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  ngOnInit(): void {
    this.loadLogs();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
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
        this.mergeUserOptions(response.items);
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

  clearFilters() {
    this.userIdFilter = null;
    this.actionTypeFilter = '';
    this.searchFilter = '';
    this.fromDate = null;
    this.toDate = null;
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

  toDateInputValue(date: Date | null) {
    if (!date) {
      return '';
    }

    const pad = (value: number) => value.toString().padStart(2, '0');

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}` +
      `T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }

  private mergeUserOptions(logs: SystemAdminLogDto[]) {
    const existingOptions = new Map(this._userOptions().map(option => [option.id, option]));

    logs.forEach(log => {
      if (!log.userId) {
        return;
      }

      existingOptions.set(log.userId, {
        id: log.userId,
        label: log.userName || log.userId,
      });
    });

    this._userOptions.set(Array.from(existingOptions.values()).sort((a, b) => a.label.localeCompare(b.label)));
  }
}
