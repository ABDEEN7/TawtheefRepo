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
import { ReviewStatus } from '../profile-managment/approval-list/models/profile-approval.models';

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

    const normalized = this.parseStructuredNote(log.notes);
    if (!normalized) {
      this.selectedLogDetails = log.notes;
      this.detailsDialogVisible = true;
      return;
    }

    let formattedText = '';
    if (normalized.humanNote) {
      formattedText += `### ${this.translateNoteMessage(normalized.parsedPayload?.message || normalized.humanNote)}\n\n`;
    }
    if (normalized.changes) {
      formattedText += `**${this.translate.instant('SYSTEM_ADMIN_LOGS.NOTES.CHANGES')}:**\n${normalized.changes}\n\n`;
    }
    if (normalized.parsedPayload) {
      formattedText += `**${this.translate.instant('SYSTEM_ADMIN_LOGS.NOTES.FULL_PAYLOAD')}:**\n${this.formatAllFields(normalized.parsedPayload)}`;
    } else if (normalized.payloadText) {
      formattedText += `**${this.translate.instant('SYSTEM_ADMIN_LOGS.NOTES.PAYLOAD')}:**\n${normalized.payloadText}`;
    }

    this.selectedLogDetails = formattedText || log.notes;
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
      const normalized = this.parseStructuredNote(log.notes);
      const parsed = normalized?.parsedPayload;
      if (parsed) {
        const target = this.extractPrimaryIdentifier(parsed);
        if (target) {
          return `${finalAction}: ${target}`;
        }
      }
    }

    return finalAction;
  }

  translateSection(section?: string | null): string {
    if (!section) return '-';

    const adminKey = 'ADMIN_SECTIONS.' + section;
    const adminTranslated = this.translate.instant(adminKey);
    if (adminTranslated !== adminKey) return adminTranslated;

    const profileKey = 'PROFILE_LOGS.SECTIONS.' + section;
    const profileTranslated = this.translate.instant(profileKey);
    if (profileTranslated !== profileKey) return profileTranslated;

    // Fallback: Format the raw section name
    return section
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, (str) => str.toUpperCase())
      .trim();
  }

  formatNotes(log: any): string {
    if (!log.notes) {
      return log.userProfileOwnerName || '-';
    }

    const normalized = this.parseStructuredNote(log.notes);
    const parsed = normalized?.parsedPayload;
    if (parsed) {
      const eventSummary = this.formatStructuredEvent(log, parsed);
      if (eventSummary) {
        return this.joinSummaryWithOwner(log.userProfileOwnerName, eventSummary);
      }

      const adminSummary = this.formatAdminStructuredEvent(log, parsed, normalized.changes);
      if (adminSummary) {
        return this.joinSummaryWithOwner(log.userProfileOwnerName, adminSummary);
      }

      if (parsed.message && typeof parsed.message === 'string') {
        return this.joinSummaryWithOwner(log.userProfileOwnerName, this.translateNoteMessage(parsed.message));
      }

      const payloadSummary = this.formatAllFields(parsed);
      if (payloadSummary !== '-') {
        return this.joinSummaryWithOwner(log.userProfileOwnerName, payloadSummary.replace(/\n/g, ' | '));
      }
    }

    if (normalized?.humanNote && !normalized.looksLikeJson) {
      const summary = normalized.changes
        ? `${this.translateNoteMessage(normalized.humanNote)} | ${this.formatLegacyChanges(normalized.changes)}`
        : this.translateNoteMessage(normalized.humanNote);
      return this.joinSummaryWithOwner(log.userProfileOwnerName, summary);
    }

    const legacyEventSummary = this.tryFormatAssignmentReassigned(log.notes) || this.formatLegacyNote(log.notes);
    if (legacyEventSummary && legacyEventSummary !== log.notes) {
      return this.joinSummaryWithOwner(log.userProfileOwnerName, legacyEventSummary);
    }

    if (log.notes.length < 200) {
      return this.joinSummaryWithOwner(log.userProfileOwnerName, log.notes);
    }

    return log.userProfileOwnerName || '-';
  }
  private joinSummaryWithOwner(owner: string | null | undefined, summary: string): string {
    if (owner && summary && summary !== owner) {
      return `${owner} - ${summary}`;
    }
    return summary || owner || '-';
  }

  private parseStructuredNote(raw: string): {
    humanNote: string;
    changes: string | null;
    payloadText: string | null;
    parsedPayload: any | null;
    looksLikeJson: boolean;
  } | null {
    if (!raw) return null;

    const parts = raw.split('| Details:');
    const humanAndChanges = parts[0].trim();
    const payloadText = parts.length > 1 ? parts[1].trim() : null;

    const changeParts = humanAndChanges.split('--- Changes ---');
    const humanNote = changeParts[0].trim();
    const changes = changeParts.length > 1 ? changeParts[1].trim() : null;
    const looksLikeJson = humanAndChanges.startsWith('{') || humanAndChanges.startsWith('[');

    let parsedPayload: any | null = null;
    try {
      parsedPayload = JSON.parse(payloadText || raw);
    } catch {
      parsedPayload = null;
    }

    return { humanNote, changes, payloadText, parsedPayload, looksLikeJson };
  }

  private formatStructuredEvent(log: any, parsed: any): string | null {
    const reassigned = this.tryFormatAssignmentReassigned(parsed);
    if (reassigned) return reassigned;

    if (parsed?.eventType === 'AssignmentCreated') {
      const assignedTo = parsed.newAssignedUserName || '-';
      return `${this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileAssigned')} | ${this.translate.instant('PROFILE_LOGS.NOTES.TARGET')}: ${assignedTo}`;
    }
    if (parsed?.eventType === 'AssignmentClosed') {
      return this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileUnassigned');
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
      const status = this.mapStatusLabel(parsed.result);
      return `${this.translate.instant('PROFILE_LOGS.ACTIONS.ProfileReviewFinalized')} | ${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}`;
    }
    if (parsed?.eventType === 'ReviewItemDecision') {
      return this.formatReviewItemDecision(parsed);
    }
    if (parsed?.eventType === 'ReviewSectionDecision') {
      return this.formatReviewSectionDecision(parsed);
    }
    if (parsed?.eventType === 'ProfileChangeRequested' || parsed?.eventType === 'ProfileChangeUpdated') {
      return this.formatProfileChangeEvent(parsed);
    }

    return null;
  }

  private formatAdminStructuredEvent(log: any, parsed: any, changes: string | null): string | null {
    if (log?.source !== 'ActionLog') return null;

    const command = this.extractAdminCommandPayload(parsed);
    const target = this.extractPrimaryIdentifier(command || parsed);
    const status = this.extractStatusValue(command || parsed);
    const actionKind = this.resolveAdminActionKind(log, parsed);
    const changedFieldsSummary = this.buildChangedFieldsSummary(parsed, changes);
    const parts: string[] = [];

    if (actionKind) {
      parts.push(`${this.translate.instant('PROFILE_LOGS.NOTES.TYPE')}: ${this.translateActionKind(actionKind)}`);
    }
    if (target) {
      parts.push(`${this.translate.instant('PROFILE_LOGS.NOTES.TARGET')}: ${target}`);
    }
    if (status) {
      parts.push(`${this.translate.instant('PROFILE_LOGS.NOTES.STATUS')}: ${status}`);
    }
    if (changedFieldsSummary) {
      parts.push(changedFieldsSummary);
    }

    if (parts.length > 0) {
      return parts.join(' | ');
    }

    const commandDetails = this.formatAllFields(command || parsed);
    return commandDetails !== '-' ? commandDetails.replace(/\n/g, ' | ') : null;
  }

  private extractAdminCommandPayload(payload: any): any {
    if (!payload || typeof payload !== 'object') return payload;

    if (payload.command && typeof payload.command === 'object') {
      return payload.command;
    }

    const firstObjectEntry = Object.values(payload).find(v => v && typeof v === 'object' && !Array.isArray(v));
    return firstObjectEntry || payload;
  }

  private extractStatusValue(payload: any): string | null {
    if (!payload || typeof payload !== 'object') return null;
    const statusKey = Object.keys(payload).find(k =>
      k.toLowerCase() === 'status' ||
      k.toLowerCase() === 'isactive' ||
      k.toLowerCase() === 'isblocked'
    );
    if (!statusKey) return null;

    const rawStatus = payload[statusKey];
    return this.formatNoteValue(rawStatus);
  }

  private resolveAdminActionKind(log: any, parsed: any): string {
    const kind = parsed?.actionKind || parsed?.operation || '';
    const normalizedKind = String(kind).toLowerCase();
    if (normalizedKind === 'create') return 'Create';
    if (normalizedKind === 'update') return 'Update';
    if (normalizedKind === 'delete') return 'Delete';
    if (kind) return String(kind);

    const actionName = String(parsed?.actionName || log?.actionType || '');
    const actionLeaf = actionName.split('.').pop() || actionName;
    if (/^create/i.test(actionLeaf)) return 'Create';
    if (/^update|^set|^block/i.test(actionLeaf)) return 'Update';
    if (/^delete/i.test(actionLeaf)) return 'Delete';
    return 'Action';
  }

  private buildChangedFieldsSummary(parsed: any, legacyChanges: string | null): string | null {
    const changedFields = parsed?.changedFields;
    if (Array.isArray(changedFields) && changedFields.length > 0) {
      const lines = changedFields
        .filter((c: any) => c && c.field)
        .map((c: any) => `${this.translateFieldLabel(c.field)}: ${this.formatNoteValue(c.oldValue)} -> ${this.formatNoteValue(c.newValue)}`);
      if (lines.length > 0) {
        return lines.join(' | ');
      }
    }

    if (legacyChanges) {
      return this.formatLegacyChanges(legacyChanges);
    }

    return null;
  }

  private formatLegacyChanges(changes: string): string {
    return changes
      .split('\n')
      .map(line => {
        const match = line.match(/^([^:]+):\s*(.*?)\s*->\s*(.*)$/);
        if (!match) return line;
        return `${this.translateFieldLabel(match[1])}: ${this.formatNoteValue(match[2])} -> ${this.formatNoteValue(match[3])}`;
      })
      .join(' | ');
  }

  private translateActionKind(kind: string): string {
    const key = `SYSTEM_ADMIN_LOGS.ACTION_KIND.${String(kind).toUpperCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : String(kind);
  }

  private translateNoteMessage(message: string): string {
    const normalized = message
      .replace(/[^A-Za-z0-9]+/g, '_')
      .replace(/^_+|_+$/g, '')
      .toUpperCase();
    const key = `SYSTEM_ADMIN_LOGS.MESSAGES.${normalized}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : message;
  }

  private translateFieldLabel(field: string): string {
    const key = `SYSTEM_ADMIN_LOGS.FIELDS.${field}`;
    const translated = this.translate.instant(key);
    if (translated !== key) return translated;

    return field
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, (str) => str.toUpperCase())
      .trim();
  }

  private formatNoteValue(value: any): string {
    if (value === null || value === undefined || value === 'None') {
      return this.translate.instant('SYSTEM_ADMIN_LOGS.VALUES.NONE');
    }
    if (value === true || value === 'True' || value === 'true') {
      return this.translate.instant('SYSTEM_ADMIN_LOGS.VALUES.YES');
    }
    if (value === false || value === 'False' || value === 'false') {
      return this.translate.instant('SYSTEM_ADMIN_LOGS.VALUES.NO');
    }
    const normalized = String(value).toLowerCase();
    if (normalized === 'approved') return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.APPROVED');
    if (normalized === 'rejected') return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.REJECTED');
    if (normalized === 'pending') return this.translate.instant('PROFILE_LOGS.REVIEW_STATUS.PENDING');
    if (normalized === 'returned') return this.translate.instant('SYSTEM_ADMIN_LOGS.VALUES.RETURNED');

    return String(value);
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

  private extractPrimaryIdentifier(obj: any): string | null {
    if (!obj || typeof obj !== 'object') return null;

    // Prioritized list of keys that represent a "main name"
    const primaryKeys = [
      'nameEn', 'fullNameEn', 'adminNameEn', 'titleEn', 'questionEn', 'email',
      'menuLabel', 'targetUrl', 'attachmentTitle', 'attachmentTitleEn', 'fileName',
      'nameAr', 'fullNameAr', 'adminNameAr', 'titleAr', 'questionAr', 'code'
    ];

    // Check current level
    for (const key of primaryKeys) {
      if (obj[key] && typeof obj[key] !== 'object' && String(obj[key]).length > 0) {
        return this.translatePotentialKey(String(obj[key]));
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

  private translatePotentialKey(value: string): string {
    const translated = this.translate.instant(value);
    return translated !== value ? translated : value;
  }

  private extractReadableFields(obj: any, results: string[]): void {
    if (!obj || typeof obj !== 'object') return;

    const displayKeys: Record<string, string> = {
      nameEn: this.translateFieldLabel('nameEn'),
      nameAr: this.translateFieldLabel('nameAr'),
      fullNameEn: this.translateFieldLabel('fullNameEn'),
      fullNameAr: this.translateFieldLabel('fullNameAr'),
      adminNameEn: this.translateFieldLabel('adminNameEn'),
      adminNameAr: this.translateFieldLabel('adminNameAr'),
      adminEmail: this.translateFieldLabel('adminEmail'),
      phoneNumber: this.translateFieldLabel('phoneNumber'),
      phoneCountryCode: this.translateFieldLabel('phoneCountryCode'),
      email: this.translateFieldLabel('email'),
      isBlocked: this.translateFieldLabel('isBlocked'),
      isActive: this.translateFieldLabel('isActive'),
      status: this.translateFieldLabel('status'),
      questionEn: this.translateFieldLabel('questionEn'),
      questionAr: this.translateFieldLabel('questionAr'),
      answerEn: this.translateFieldLabel('answerEn'),
      answerAr: this.translateFieldLabel('answerAr'),
      titleEn: this.translateFieldLabel('titleEn'),
      titleAr: this.translateFieldLabel('titleAr'),
      descriptionEn: this.translateFieldLabel('descriptionEn'),
      descriptionAr: this.translateFieldLabel('descriptionAr'),
      code: this.translateFieldLabel('code'),
      value: this.translateFieldLabel('value'),
      order: this.translateFieldLabel('order'),
      type: this.translateFieldLabel('type'),
      roleIds: this.translateFieldLabel('roleIds'),
      permissionKeys: this.translateFieldLabel('permissionKeys')
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
        displayVal = this.formatNoteValue(value);
      }

      if (displayVal && displayVal !== 'null' && displayVal !== 'undefined') {
        results.push(`${label}: ${displayVal}`);
      }
    }
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

}
