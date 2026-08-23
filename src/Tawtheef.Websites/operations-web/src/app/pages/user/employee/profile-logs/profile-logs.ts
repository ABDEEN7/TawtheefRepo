import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { DatePicker } from 'primeng/datepicker';
import { MultiSelect } from 'primeng/multiselect';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { debounceTime, distinctUntilChanged, map, Subject, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProfileLogsService } from './services/profile-logs.service';
import { ProfileLogDto } from './models/profile-log.dto';
import { ProfileLogFilters } from './models/profile-log-filters.dto';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { ReviewStatus } from '../profile-managment/approval-list/models/profile-approval.models';
import { ProfileStatusNumber } from '../../../../core/enums/lookups.enum';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';

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
    MultiSelect,
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PageFiltersComponent,
  ]
})
export class ProfileLogsComponent implements OnInit {
  private profileLogsService = inject(ProfileLogsService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);

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

  private profileId: string | null = null;
  userIdFilter: string | null = null;
  candidateSearch = '';
  notesSearch = '';
  selectedSections: string[] = [];
  selectedProfileStatuses: ProfileStatusNumber[] = [];
  fromDate: Date | null = null;
  toDate: Date | null = null;
  showAdvancedFilters = signal(false);
  invalidProfileId = signal(false);
  invalidDateRange = signal(false);

  private candidateSearchChanges$ = new Subject<string>();
  private notesSearchChanges$ = new Subject<string>();
  private queryChanges$ = new Subject<boolean>();

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);
  readonly sectionOptions = [
    'Prerequisites', 'Personal', 'Contact', 'Qualifications', 'Experience',
    'TrainingCourses', 'CertificatesAndAwards', 'Skills', 'Languages', 'Attachments', 'Assignment',
  ].map(value => ({ value, label: `PROFILE_LOGS.SECTIONS.${value}` }));

  readonly profileStatusOptions = [
    { value: ProfileStatusNumber.InCreation, label: 'PROFILE_LOGS.PROFILE_STATUS.IN_CREATION' },
    { value: ProfileStatusNumber.Submitted, label: 'PROFILE_LOGS.PROFILE_STATUS.SUBMITTED' },
    { value: ProfileStatusNumber.UnderReview, label: 'PROFILE_LOGS.PROFILE_STATUS.UNDER_REVIEW' },
    { value: ProfileStatusNumber.RequiresUpdate, label: 'PROFILE_LOGS.PROFILE_STATUS.REQUIRES_UPDATE' },
    { value: ProfileStatusNumber.Approved, label: 'PROFILE_LOGS.PROFILE_STATUS.APPROVED' },
  ];

  ngOnInit(): void {
    this.setupRequestOrchestration();
    this.loadUserOptions();
    const routeProfileId = this.route.snapshot.queryParamMap.get('profileId');
    if (routeProfileId) {
      if (this.isGuid(routeProfileId)) {
        this.profileId = routeProfileId;
      } else {
        this.invalidProfileId.set(true);
      }
    }
    if (!this.invalidProfileId()) this.loadLogs();
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));
  }

  loadUserOptions() {
    this.profileLogsService.getUsersLookup().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (users) => {
        const options = (users || []).map(u => ({
          id: u.id,
          label: u.name
        }));
        this._userOptions.set(options.sort((a, b) => a.label.localeCompare(b.label)));
      }
    });
  }

  private setupRequestOrchestration(): void {
    this.setupTextFilter(this.candidateSearchChanges$, value => {
      if (this.candidateSearch.trim() === value) return false;
      this.candidateSearch = value;
      return true;
    });
    this.setupTextFilter(this.notesSearchChanges$, value => {
      if (this.notesSearch.trim() === value) return false;
      this.notesSearch = value;
      return true;
    });

    this.queryChanges$
      .pipe(
        map(force => {
          const filters = this.buildFilters();
          return { filters, key: JSON.stringify(filters), force };
        }),
        distinctUntilChanged((previous, current) => !current.force && previous.key === current.key),
        switchMap(({ filters }) => this.profileLogsService.getLogs(filters)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({ next: response => this.applyResponse(response) });
  }

  private setupTextFilter(changes$: Subject<string>, apply: (value: string) => boolean): void {
    changes$.pipe(
      map(value => (value ?? '').trim()),
      debounceTime(400),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(value => {
      if (!apply(value)) return;
      this.filters.update(filters => ({ ...filters, pageNumber: 1 }));
      this.loadLogs();
    });
  }

  private buildFilters(): ProfileLogFilters {
    const updatedFilters: ProfileLogFilters = {
      ...this.filters(),
      userProfileId: this.profileId,
      userId: this.userIdFilter || null,
      source: 'UserProfileLogger',
      from: this.fromDate ? this.fromDate.toISOString() : null,
      to: this.toDate ? this.toDate.toISOString() : null,
      candidateSearch: this.candidateSearch.trim() || null,
      notesSearch: this.notesSearch.trim() || null,
      sections: this.selectedSections.length ? [...this.selectedSections] : null,
      profileStatuses: this.selectedProfileStatuses.length ? [...this.selectedProfileStatuses] : null,
    };
    this.filters.set(updatedFilters);
    return updatedFilters;
  }

  private applyResponse(response: PaginatedResult<ProfileLogDto>): void {
    this._logs.set(response.items);
    this._paginationMetadata.set(response.metadata);
    if (response.metadata) {
      this.filters.update(filters => ({ ...filters,
        pageNumber: response.metadata.currentPage, pageSize: response.metadata.pageSize }));
    }
  }

  loadLogs(force = false): void {
    if (!this.invalidProfileId() && !this.invalidDateRange()) this.queryChanges$.next(force);
  }

  onFiltersChanged(): void {
    this.filters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadLogs();
  }

  onCandidateSearchChange(value: string): void {
    this.candidateSearchChanges$.next(value);
  }

  onNotesSearchChange(value: string): void {
    this.notesSearchChanges$.next(value);
  }

  onPerformedByChange(value: string | null): void {
    this.userIdFilter = value;
    this.onFiltersChanged();
  }

  onSectionsChange(value: string[] | null): void {
    this.selectedSections = value ?? [];
    this.onFiltersChanged();
  }

  onProfileStatusesChange(value: ProfileStatusNumber[] | null): void {
    this.selectedProfileStatuses = value ?? [];
    this.onFiltersChanged();
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters.update(value => !value);
  }

  activeAdvancedFilterCount(): number {
    return [
      this.fromDate,
      this.toDate,
      this.notesSearch.trim() || null,
      this.selectedSections.length ? true : null,
      this.selectedProfileStatuses.length ? true : null,
    ].filter(Boolean).length;
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
    this.onDateRangeChange();
  }

  onToDateChange(value: Date | null) {
    this.toDate = value;
    this.onDateRangeChange();
  }

  private onDateRangeChange(): void {
    const invalid = !!this.fromDate && !!this.toDate && this.fromDate > this.toDate;
    this.invalidDateRange.set(invalid);
    if (!invalid) this.onFiltersChanged();
  }

  alignDatePickerOverlay(datePicker: DatePicker): void {
    if (!this.isRtl() || !datePicker.overlay || !datePicker.inputfieldViewChild) return;

    const input = datePicker.inputfieldViewChild.nativeElement as HTMLElement;
    const inputBounds = input.getBoundingClientRect();
    const overlayWidth = datePicker.overlay.offsetWidth;
    const viewportStart = window.scrollX;
    const rightAlignedPosition = inputBounds.right + window.scrollX - overlayWidth;

    datePicker.overlay.style.left = `${Math.max(viewportStart, rightAlignedPosition)}px`;
  }

  clearFilters() {
    this.userIdFilter = null;
    this.candidateSearch = '';
    this.notesSearch = '';
    this.candidateSearchChanges$.next('');
    this.notesSearchChanges$.next('');
    this.selectedSections = [];
    this.selectedProfileStatuses = [];
    this.fromDate = null;
    this.toDate = null;
    this.invalidDateRange.set(false);
    const pageSize = this.filters().pageSize;
    this.filters.set({ pageNumber: 1, pageSize });
    this.loadLogs(true);
  }

  private isGuid(value: string): boolean {
    return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value);
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

    // If we have enriched human-readable note format, use it.
    // Do not return early when the note itself looks like JSON.
    const looksLikeJson = humanAndChanges.startsWith('{') || humanAndChanges.startsWith('[');
    if ((changes || humanNote) && !looksLikeJson) {
      let summary = humanNote;
      if (changes) {
        summary += (summary ? ' | ' : '') + changes.replace(/\n/g, ' | ');
      }
      return summary;
    }

    try {
      const jsonString = jsonPart || log.notes;
      const parsed = JSON.parse(jsonString);
      const assignmentSummary = this.tryFormatAssignmentReassigned(parsed);
      if (assignmentSummary) return assignmentSummary;
      if (parsed?.eventType === 'ReviewItemDecision') {
        return this.formatReviewItemDecision(parsed);
      }
      if (parsed?.eventType === 'ReviewSectionDecision') {
        return this.formatReviewSectionDecision(parsed);
      }
      if (parsed?.eventType === 'ProfileChangeRequested' || parsed?.eventType === 'ProfileChangeUpdated') {
        return this.formatProfileChangeEvent(parsed);
      }
      if (parsed?.eventType === 'AssignmentCreated') {
        const assignedTo = parsed.newAssignedUserName || '-';
        return `${this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileAssigned')} | ${this.translate.instant('PROFILE_LOGS.NOTES.TARGET')}: ${assignedTo}`;
      }
      if (parsed?.eventType === 'OpenProfile') {
        const section = this.translateSection(log?.section || parsed.section);
        return `${this.translate.instant('PROFILE_LOGS.ACTIONS.OpenProfile')} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section}`;
      }
      if (parsed?.eventType === 'OpenProfileChangeReview') {
        const section = this.translateSection(log?.section || parsed.section);
        return `${this.translate.instant('PROFILE_LOGS.ACTIONS.OpenProfileChangeReview')} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section}`;
      }
      if (parsed?.eventType === 'ProfileReviewStarted') {
        return this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileReviewStarted');
      }
      if (parsed?.eventType === 'ProfileReviewFinalized') {
        const result = this.mapStatusLabel(parsed.result);
        return `${this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileReviewFinalized')} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${result}`;
      }
      if (parsed?.eventType === 'AssignmentClosed') {
        return this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileUnassigned');
      }
      if (parsed?.message && typeof parsed.message === 'string') {
        return parsed.message;
      }
      const results: string[] = [];
      this.extractReadableFields(parsed, results);
      return results.length > 0 ? results.join(' | ') : '-';
    } catch {
      const assignmentSummary = this.tryFormatAssignmentReassigned(log.notes);
      if (assignmentSummary) return assignmentSummary;
      return this.formatLegacyNote(log.notes);
    }
  }

  private tryFormatAssignmentReassigned(source: any): string | null {
    if (!source) return null;

    if (typeof source === 'object' && source.eventType === 'AssignmentReassigned') {
      const oldName = source.oldAssignedUserName || '-';
      const newName = source.newAssignedUserName || '-';
      return `${this.translate.instant('PROFILE_LOGS.NOTES.REASSIGNED_FROM')}: ${oldName} | ${this.translate.instant('PROFILE_LOGS.NOTES.REASSIGNED_TO')}: ${newName}`;
    }

    if (typeof source !== 'string') return null;
    if (!source.includes('AssignmentReassigned')) return null;

    const oldNameMatch = source.match(/oldAssignedUserName"\s*:\s*"([^"]*)"/i);
    const newNameMatch = source.match(/newAssignedUserName"\s*:\s*"([^"]*)"/i);

    const oldName = oldNameMatch?.[1] || '-';
    const newName = newNameMatch?.[1] || '-';
    return `${this.translate.instant('PROFILE_LOGS.NOTES.REASSIGNED_FROM')}: ${oldName} | ${this.translate.instant('PROFILE_LOGS.NOTES.REASSIGNED_TO')}: ${newName}`;
  }

  private formatReviewItemDecision(note: any): string {
    const section = note.section ? this.translateSection(note.section) : '-';
    const status = this.mapStatusLabel(note.status);
    const itemLabel = note.entityName || note.fieldPath || this.translate.instant('PROFILE_LOGS.ACTIONS.ReviewItemDecision');
    const reviewerNote = note.reviewerNote ? ` | ${this.translate.instant('PROFILE_LOGS.NOTES.NOTE')}: ${note.reviewerNote}` : '';
    return `${this.translate.instant('PROFILE_LOGS.NOTES.ITEM_REVIEW_UPDATED')} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section} | ${this.translate.instant('PROFILE_LOGS.NOTES.ITEM')}: ${itemLabel} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}${reviewerNote}`;
  }

  private formatReviewSectionDecision(note: any): string {
    const section = note.section ? this.translateSection(note.section) : '-';
    const status = this.mapStatusLabel(note.status);
    const reviewerNote = note.reviewerNote ? ` | ${this.translate.instant('PROFILE_LOGS.NOTES.NOTE')}: ${note.reviewerNote}` : '';
    return `${this.translate.instant('PROFILE_LOGS.NOTES.SECTION_REVIEW_UPDATED')} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}${reviewerNote}`;
  }

  private formatProfileChangeEvent(note: any): string {
    const section = note.section ? this.translateSection(note.section) : '-';
    const target = note.entityName || note.fieldPath || note.attachmentTitle || note.targetType || '-';
    const action = note.eventType === 'ProfileChangeRequested'
      ? this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileChangeRequested')
      : this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileChangeUpdated');
    return `${action} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section} | ${this.translate.instant('PROFILE_LOGS.NOTES.TARGET')}: ${target}`;
  }

  private formatLegacyNote(raw: string): string {
    const reviewItemPattern = /^Review item [0-9a-f-]{36} marked (\w+)$/i;
    const reviewSectionPattern = /^Section (\w+) marked (\w+)$/i;
    const changePattern = /^(Field|Row|Section|Attachment) change for (.+)$/i;

    const reviewItemMatch = raw.match(reviewItemPattern);
    if (reviewItemMatch) {
      const status = this.mapStatusLabel(reviewItemMatch[1]);
      return `${this.translate.instant('PROFILE_LOGS.NOTES.ITEM_REVIEW_UPDATED')} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}`;
    }

    const reviewSectionMatch = raw.match(reviewSectionPattern);
    if (reviewSectionMatch) {
      const section = this.translateSection(reviewSectionMatch[1]);
      const status = this.mapStatusLabel(reviewSectionMatch[2]);
      return `${this.translate.instant('PROFILE_LOGS.NOTES.SECTION_REVIEW_UPDATED')} | ${this.translate.instant('PROFILE_LOGS.NOTES.SECTION')}: ${section} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}`;
    }

    const changeMatch = raw.match(changePattern);
    if (changeMatch) {
      const targetType = changeMatch[1];
      const targetValue = changeMatch[2];
      const sanitizedTarget = /^[0-9a-f-]{36}$/i.test(targetValue) ? this.translate.instant('PROFILE_LOGS.NOTES.PROFILE_DATA') : targetValue;
      return `${this.translate.instant('PROFILE_LOGS.NOTES.PROFILE_CHANGE_UPDATED')} | ${this.translate.instant('PROFILE_LOGS.NOTES.TYPE')}: ${targetType} | ${this.translate.instant('PROFILE_LOGS.NOTES.TARGET')}: ${sanitizedTarget}`;
    }

    return raw;
  }

  private mapStatusLabel(status: string | number | null | undefined): string {
    if (status === null || status === undefined) return '-';
    const normalized = String(status).toLowerCase();
    if (normalized === 'pending' || normalized === String(ReviewStatus.Pending)) {
      return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.PENDING');
    }
    if (normalized === 'approved' || normalized === String(ReviewStatus.Approved)) {
      return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.APPROVED');
    }
    if (normalized === 'rejected' || normalized === String(ReviewStatus.Rejected)) {
      return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.REJECTED');
    }
    if (normalized === 'needscorrection' || normalized === 'needs_correction' || normalized === String(ReviewStatus.NeedsCorrection)) {
      return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.NEEDS_CORRECTION');
    }
    if (normalized === 'notreviewed' || normalized === 'not_reviewed' || normalized === String(ReviewStatus.NotReviewed)) {
      return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.NOT_REVIEWED');
    }
    return String(status);
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
      permissionKeys: 'Permissions',
      oldAssignedUserName: 'Old Assigned User',
      newAssignedUserName: 'New Assigned User',
      oldAssignedUserId: 'Old Assigned User Id',
      newAssignedUserId: 'New Assigned User Id'
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

