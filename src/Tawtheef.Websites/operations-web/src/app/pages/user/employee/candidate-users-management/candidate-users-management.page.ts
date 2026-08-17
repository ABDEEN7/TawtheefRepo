import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { Select } from 'primeng/select';
import { CandidateUsersService } from './services/candidate-users.service';
import { CandidateUserDto } from './models/candidate-user.dto';
import {
  CandidateUserFilters,
  CandidateUsersResultScope
} from './models/candidate-user-filters.dto';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../core/services/notification.service';
import { routes } from '../../../../routes/routes';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { Permissions } from '../../../../core/constants/permissions';
import { ProfileStatus, ProfileStatusNumber } from '../../../../core/enums/lookups.enum';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { debounceTime, distinctUntilChanged, finalize, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProfileLogDto } from '../profile-logs/models/profile-log.dto';
import { ReviewStatus } from '../profile-managment/approval-list/models/profile-approval.models';
import { FileUtilsService } from '../../../../core/utils/file-utils';
import { parseFilterYear } from '../../../../core/utils/year-filter.util';

@Component({
  selector: 'app-candidate-users-management',
  standalone: true,
  templateUrl: './candidate-users-management.page.html',
  styleUrls: ['./candidate-users-management.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    Dialog,
    Select,
    PaginationComponent,
    I18nNamespaceDirective,
    HasPermissionDirective
  ]
})
export class CandidateUsersManagementPage implements OnInit {
  private candidateUsersService = inject(CandidateUsersService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fileUtils = inject(FileUtilsService);

  private _users = signal<CandidateUserDto[]>([]);
  private _paginationMetadata = signal<PaginationMetadata | null>(null);
  private _profileLogs = signal<ProfileLogDto[]>([]);
  private _profileLogsLoading = signal(false);
  readonly exporting = signal(false);

  users = this._users.asReadonly();
  paginationMetadata = this._paginationMetadata.asReadonly();
  profileLogs = this._profileLogs.asReadonly();
  profileLogsLoading = this._profileLogsLoading.asReadonly();

  filters = signal<CandidateUserFilters>({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    email: '',
    qid: '',
    mobileNumber: '',
    scope: CandidateUsersResultScope.Default
  });

  nameFilter = '';
  emailFilter = '';
  qidFilter = '';
  mobileFilter = '';
  readonly profileStatusOptions = Object.values(ProfileStatus).map(status => ({
    value: status,
    labelKey: `common.${status}`
  }));
  private searchChanges$ = new Subject<string>();

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  totalItems = computed(() => this.paginationMetadata()?.totalCount || 0);

  protected readonly Permissions = Permissions;
  protected readonly ProfileStatus = ProfileStatusNumber;
  profileLogsDialogVisible = false;
  profileLogsCandidateName = '';

  ngOnInit(): void {
    this.setupSearchListener();
    const profileStatus = this.parseProfileStatus(this.route.snapshot.queryParamMap.get('profileStatus'));
    const scope = this.parseResultScope(this.route.snapshot.queryParamMap.get('scope'));
    const year = parseFilterYear(this.route.snapshot.queryParamMap.get('year'));
    this.filters.update(filters => ({ ...filters, profileStatus, scope, year }));
    this.loadUsers();
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  loadUsers(): void {
    this.candidateUsersService.getCandidateUsers(this.filters()).subscribe({
      next: (response: PaginatedResult<CandidateUserDto>) => {
        this._users.set(response.items);
        this._paginationMetadata.set(response.metadata);

        if (response.metadata) {
          this.filters.update(f => ({
            ...f,
            pageNumber: response.metadata.currentPage,
            pageSize: response.metadata.pageSize
          }));
        }
      }
    });
  }

  onSearchChange(): void {
    this.searchChanges$.next(`${this.nameFilter}|${this.emailFilter}|${this.qidFilter}|${this.mobileFilter}`);
  }

  private setupSearchListener(): void {
    this.searchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.filters.update(f => ({
          ...f,
          pageNumber: 1,
          name: this.nameFilter,
          email: this.emailFilter,
          qid: this.qidFilter,
          mobileNumber: this.mobileFilter
        }));
        this.loadUsers();
      });
  }

  onPageChange(page: number): void {
    this.filters.update(f => ({ ...f, pageNumber: page }));
    this.loadUsers();
  }

  onPageSizeChange(size: number): void {
    this.filters.update(f => ({ ...f, pageSize: size, pageNumber: 1 }));
    this.loadUsers();
  }

  exportUsers(): void {
    if (this.exporting()) return;
    this.exporting.set(true);
    this.candidateUsersService.exportCandidateUsers(this.filters())
      .pipe(finalize(() => this.exporting.set(false)))
      .subscribe(response => void this.fileUtils.downloadResponse(response, 'candidate-users.xlsx'));
  }

  onProfileStatusChange(profileStatus: ProfileStatus | null): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      profileStatus: profileStatus ?? undefined
    }));
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { profileStatus: profileStatus ?? null },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
    this.loadUsers();
  }

  clearYear(): void {
    this.filters.update(filters => ({ ...filters, pageNumber: 1, year: undefined }));
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { year: null },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
    this.loadUsers();
  }

  toggleBlock(user: CandidateUserDto): void {
    const desiredState = !user.isBlocked;
    this.candidateUsersService.updateBlockStatus(user.id, desiredState).subscribe({
      next: () => {
        this._users.update(users =>
          users.map(item => (item.id === user.id ? { ...item, isBlocked: desiredState } : item))
        );
        this.notification.success(
          this.translate.instant(
            desiredState ? 'CANDIDATE_USERS.BLOCK_SUCCESS' : 'CANDIDATE_USERS.UNBLOCK_SUCCESS'
          )
        );
      }
    });
  }

  viewProfile(user: CandidateUserDto): void {
    const profileId = user.candidateProfileId || user.userProfileId || user.id;
    if (!profileId) return;

    this.router.navigate([routes.portal.candidateUserProfile(profileId)], {
      queryParams: { email: user.email }
    });
  }

  viewProfileLogs(user: CandidateUserDto): void {
    if (!user.id) return;

    this.profileLogsCandidateName = user.fullNameEn || user.fullNameAr || user.email;
    this.profileLogsDialogVisible = true;
    this._profileLogsLoading.set(true);

    this.candidateUsersService.getCandidateProfileLogs(user.id).subscribe({
      next: (res) => {
        this._profileLogs.set(res.items || []);
        this._profileLogsLoading.set(false);
      },
      error: () => {
        this._profileLogs.set([]);
        this._profileLogsLoading.set(false);
      }
    });
  }

  closeProfileLogsDialog(): void {
    this.profileLogsDialogVisible = false;
    this._profileLogs.set([]);
    this.profileLogsCandidateName = '';
  }

  private parseProfileStatus(value: string | null): ProfileStatus | undefined {
    return value && Object.values(ProfileStatus).includes(value as ProfileStatus)
      ? value as ProfileStatus
      : undefined;
  }

  private parseResultScope(value: string | null): CandidateUsersResultScope {
    return value === CandidateUsersResultScope.AccessibleProfiles || value === 'DashboardAccessible'
      ? CandidateUsersResultScope.AccessibleProfiles
      : CandidateUsersResultScope.Default;
  }

  translateSection(section?: string | null): string {
    if (!section) return '-';
    const key = 'PROFILE_LOGS.SECTIONS.' + section;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : section;
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
    return String(status);
  }

  formatProfileLogNote(log: ProfileLogDto): string {
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
}
