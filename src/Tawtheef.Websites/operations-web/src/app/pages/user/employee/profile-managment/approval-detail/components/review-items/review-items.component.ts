import { CommonModule } from '@angular/common';
import { Component, DestroyRef, EventEmitter, Input, Output, inject } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import {ProfileApprovalItem, ReviewStatus, ReviewTargetType} from '../../../approval-list/models/profile-approval.models';

export type ReviewAction = 'approve' | 'reject' | 'changes';

type DiffRow = {
  field: string;
  label: string;
  labelKey?: string;
  oldValue: string;
  newValue: string;
  oldUrl?: string | null;
  newUrl?: string | null;
};

@Component({
  selector: 'app-profile-review-items',
  standalone: true,
  imports: [CommonModule, TranslateModule, ButtonModule, TagModule],
  templateUrl: './review-items.component.html',
  styleUrls: ['../../profile-approval-detail.page.scss'],
})
export class ReviewItemsComponent {
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  @Input() items: ProfileApprovalItem[] | null = [];
  @Input() titleKey = '';
  @Output() review = new EventEmitter<{ item: ProfileApprovalItem; action: ReviewAction }>();
  @Output() viewFile = new EventEmitter<string>();

  private readonly diffCache = new Map<string, { oldRaw?: string; newRaw?: string; rows: DiffRow[] | null }>();

  protected readonly ReviewStatus = ReviewStatus;
  protected readonly ReviewTargetType = ReviewTargetType;

  private readonly fieldLabelKeyMap: Record<string, string> = {
    CandidateTypeId: 'profileApproval.detail.snapshot.candidateType',
    TargetEntityId: 'profileApproval.detail.snapshot.targetEntity',
    OfficeId: 'profileApproval.detail.snapshot.office',
    QidExpiry: 'profileApproval.detail.snapshot.idExpiry',
    ResumeAttachmentId: 'profileApproval.detail.snapshot.resumeAttachment',
    NationalCardId: 'profileApproval.detail.snapshot.nationalCard',
    BirthCertificateId: 'profileApproval.detail.snapshot.birthdayCertificate',
    MarriageCertificateId: 'profileApproval.detail.snapshot.marriageCertificate',

    FullNameAr: 'profileApproval.detail.snapshot.fullNameAr',
    FullNameEn: 'profileApproval.detail.snapshot.fullNameEn',
    NationalNumber: 'profileApproval.detail.snapshot.nationalNumber',
    BirthDate: 'profileApproval.detail.snapshot.birthDate',
    NationalityId: 'profileApproval.detail.snapshot.nationality',
    GenderId: 'profileApproval.detail.snapshot.gender',
    ReligionId: 'profileApproval.detail.snapshot.religion',
    MaritalStatusId: 'profileApproval.detail.snapshot.maritalStatus',
    ChildrenCount: 'profileApproval.detail.snapshot.childrenCount',
    HasDisability: 'profileApproval.detail.snapshot.disability',
    DisabilityDetails: 'profileApproval.detail.snapshot.disabilityDetails',
    SponsorTypeId: 'profileApproval.detail.snapshot.sponsorType',
    SponsorEmployerName: 'profileApproval.detail.snapshot.sponsorEmployerName',
    SponsorEmployerNumber: 'profileApproval.detail.snapshot.sponsorEmployerNumber',
    SponsorQidExpiry: 'profileApproval.detail.snapshot.sponsorQidExpiry',
    SponsorCardResourceId: 'profileApproval.detail.snapshot.sponsorCard',

    ResidenceCountryId: 'profileApproval.detail.snapshot.residenceCountry',
    InterviewLocationId: 'profileApproval.detail.snapshot.interviewLocation',
    Address: 'profileApproval.detail.snapshot.address',
    Zone: 'profileApproval.detail.changeFields.zone',
    Street: 'profileApproval.detail.changeFields.street',
    Building: 'profileApproval.detail.changeFields.building',
    Unit: 'profileApproval.detail.changeFields.unit',
    NationalAddressCertificateId: 'profileApproval.detail.snapshot.residenceAddressCertificate',

    DegreeId: 'profileApproval.detail.qualifications.level',
    GradCountryId: 'profileApproval.detail.qualifications.country',
    UniversityId: 'profileApproval.detail.qualifications.university',
    MajorId: 'profileApproval.detail.qualifications.major',
    SubMajorId: 'profileApproval.detail.qualifications.subMajor',
    StudyTypeId: 'profileApproval.detail.qualifications.studySystem',
    GradeId: 'profileApproval.detail.qualifications.grade',
    GradYear: 'profileApproval.detail.qualifications.year',
    Gpa: 'profileApproval.detail.qualifications.gpa',
    AttachmentResourceId: 'profileApproval.detail.qualifications.attachment',

    EmployerName: 'profileApproval.detail.changeFields.employerName',
    JobTitle: 'profileApproval.detail.changeFields.jobTitle',
    StartDate: 'profileApproval.detail.changeFields.startDate',
    EndDate: 'profileApproval.detail.changeFields.endDate',
    CountryId: 'profileApproval.detail.changeFields.country',
    CertificateId: 'profileApproval.detail.changeFields.certificate',
    Description: 'profileApproval.detail.changeFields.description',
    QualificationId: 'profileApproval.detail.experiences.qualification',

    Title: 'profileApproval.detail.changeFields.title',
    Provider: 'profileApproval.detail.changeFields.provider',

    AchievementTypeId: 'profileApproval.detail.changeFields.achievementType',
    IssuingAuthority: 'profileApproval.detail.changeFields.issuingAuthority',
    IssueDate: 'profileApproval.detail.certificates.issueDate',

    SkillId: 'profileApproval.detail.changeFields.skill',
    LevelId: 'profileApproval.detail.skills.level',

    LanguageId: 'profileApproval.detail.changeFields.language',
    SpeakingLevelId: 'profileApproval.detail.snapshot.speaking',
    WritingLevelId: 'profileApproval.detail.snapshot.writing',
    ReadingLevelId: 'profileApproval.detail.snapshot.reading',

    FileName: 'profileApproval.detail.attachments.fileName',
  };

  constructor() {
    this.translate.onLangChange
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.diffCache.clear());
  }

  statusSeverity(status: ReviewStatus | undefined): 'success' | 'danger' | 'info' | 'warn' {
    switch (status) {
      case ReviewStatus.Approved:
        return 'success';
      case ReviewStatus.Rejected:
        return 'danger';
      case ReviewStatus.ChangesRequested:
        return 'warn';
      default:
        return 'info';
    }
  }

  statusLabel(status: ReviewStatus | undefined): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'profileApproval.status.approved';
      case ReviewStatus.Rejected:
        return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested:
        return 'profileApproval.status.changes';
      default:
        return 'profileApproval.status.pending';
    }
  }

  onReview(item: ProfileApprovalItem, action: ReviewAction): void {
    this.review.emit({ item, action });
  }

  preview(item: ProfileApprovalItem): void {
    if (item.resourceUrl) {
      this.viewFile.emit(item.resourceUrl);
    }
  }

  displayTitle(item: ProfileApprovalItem): string {
    if (!item) return '-';

    if (item.targetType === ReviewTargetType.Section) {
      return this.translate.instant('profileApproval.detail.textOnly');
    }

    if (item.targetType === ReviewTargetType.Attachment) {
      return this.translate.instant('profileApproval.detail.attachment');
    }

    if (item.targetType === ReviewTargetType.Row && item.entityName) {
      return this.entityLabel(item.entityName);
    }

    if (item.targetType === ReviewTargetType.Field && item.title) {
      const key = this.fieldLabelKeyMap[item.title];
      if (key) return this.translate.instant(key);
    }

    return item.title || '-';
  }

  entityLabel(entityName?: string | null): string {
    const raw = (entityName ?? '').trim();
    if (!raw) return '';

    const key = `profileApproval.detail.entities.${raw}`;
    const translated = this.translate.instant(key);
    return translated === key ? raw : translated;
  }

  openUrl(url?: string | null): void {
    if (!url) return;
    this.viewFile.emit(url);
  }

  formatValue(raw?: string): string {
    return this.formatLeaf(this.parseJsonish(raw));
  }

  diffRows(item: ProfileApprovalItem): DiffRow[] | null {
    const cached = this.diffCache.get(item.reviewItemId);
    if (cached && cached.oldRaw === item.oldValue && cached.newRaw === item.newValue) {
      return cached.rows;
    }

    const oldParsed = this.parseJsonish(item.oldValue);
    const newParsed = this.parseJsonish(item.newValue);
    const rows = this.buildDiffRows(oldParsed, newParsed);

    this.diffCache.set(item.reviewItemId, { oldRaw: item.oldValue, newRaw: item.newValue, rows });
    return rows;
  }

  private buildDiffRows(oldParsed: unknown, newParsed: unknown): DiffRow[] | null {
    const oldObj = this.isPlainObject(oldParsed) ? (oldParsed as Record<string, unknown>) : null;
    const newObj = this.isPlainObject(newParsed) ? (newParsed as Record<string, unknown>) : null;

    if (!oldObj && !newObj) return null;

    const oldFlat = oldObj ? this.flattenObject(oldObj) : {};
    const newFlat = newObj ? this.flattenObject(newObj) : {};
    const keys = new Set([...Object.keys(oldFlat), ...Object.keys(newFlat)]);

    const rows = Array.from(keys)
      .sort((a, b) => a.localeCompare(b))
      .map(field => {
        const oldRaw = oldFlat[field];
        const newRaw = newFlat[field];
        const oldValue = this.formatLeaf(oldRaw);
        const newValue = this.formatLeaf(newRaw);
        const labelKey = this.fieldLabelKeyMap[field] ?? undefined;

        return {
          field,
          label: this.humanizePath(field),
          labelKey,
          oldValue,
          newValue,
          oldUrl: this.extractUrl(oldRaw),
          newUrl: this.extractUrl(newRaw),
        } satisfies DiffRow;
      })
      .filter(r => r.oldValue !== r.newValue);

    return rows.length ? rows : null;
  }

  private parseJsonish(raw?: string): unknown {
    const value = (raw ?? '').trim();
    if (!value) return undefined;

    try {
      return JSON.parse(value);
    } catch {
      return value;
    }
  }

  private isPlainObject(value: unknown): value is Record<string, unknown> {
    if (!value || typeof value !== 'object') return false;
    if (Array.isArray(value)) return false;
    return Object.getPrototypeOf(value) === Object.prototype;
  }

  private flattenObject(obj: Record<string, unknown>, prefix = ''): Record<string, unknown> {
    const result: Record<string, unknown> = {};

    Object.entries(obj).forEach(([key, val]) => {
      const nextPath = prefix ? `${prefix}.${key}` : key;

      if (this.isPlainObject(val) && !this.isDisplayObject(val)) {
        Object.assign(result, this.flattenObject(val as Record<string, unknown>, nextPath));
        return;
      }

      result[nextPath] = val;
    });

    return result;
  }

  private formatLeaf(value: unknown): string {
    if (value === null || value === undefined) return '';
    if (typeof value === 'string') return value.trim();
    if (typeof value === 'number') return String(value);
    if (typeof value === 'boolean') return this.translate.instant(value ? 'common.yes' : 'common.no');

    if (Array.isArray(value)) {
      const items = value
        .map(v => this.formatLeaf(v))
        .filter(v => !!v);

      if (items.length === 0) return '';
      if (items.length <= 5) return items.join(', ');
      return this.translate.instant('profileApproval.detail.itemsCount', { count: items.length });
    }

    if (this.isPlainObject(value)) {
      const o = value as Record<string, unknown>;
      const preferred =
        this.pickString(o, 'name') ??
        this.pickString(o, 'Name') ??
        this.pickString(o, 'title') ??
        this.pickString(o, 'label') ??
        this.pickString(o, 'fileName') ??
        this.pickString(o, 'FileName');

      return preferred ?? this.translate.instant('profileApproval.detail.object');
    }

    return String(value);
  }

  private pickString(obj: Record<string, unknown>, key: string): string | null {
    const val = obj[key];
    if (typeof val !== 'string') return null;
    const trimmed = val.trim();
    return trimmed.length ? trimmed : null;
  }

  private isDisplayObject(value: unknown): boolean {
    if (!this.isPlainObject(value)) return false;
    const o = value as Record<string, unknown>;
    return (
      typeof o['name'] === 'string' ||
      typeof o['Name'] === 'string' ||
      typeof o['fileName'] === 'string' ||
      typeof o['FileName'] === 'string' ||
      typeof o['url'] === 'string' ||
      typeof o['Url'] === 'string'
    );
  }

  private extractUrl(value: unknown): string | null {
    if (!this.isPlainObject(value)) return null;
    const o = value as Record<string, unknown>;
    const url = o['url'] ?? o['Url'];
    if (typeof url !== 'string') return null;
    const trimmed = url.trim();
    return trimmed.length ? trimmed : null;
  }

  private humanizePath(path: string): string {
    return path
      .split('.')
      .map(segment => this.humanizeSegment(segment))
      .join(' / ');
  }

  private humanizeSegment(segment: string): string {
    const noIdSuffix = segment.replace(/Id$/, '');
    const spaced = noIdSuffix
      .replace(/[_-]+/g, ' ')
      .replace(/([a-z0-9])([A-Z])/g, '$1 $2')
      .trim();

    return spaced.length ? spaced : segment;
  }
}
