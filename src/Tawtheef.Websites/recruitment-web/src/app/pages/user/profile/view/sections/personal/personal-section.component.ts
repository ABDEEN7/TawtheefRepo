import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import {
  MyProfileReviewNoteDto,
  MyProfileReviewChangedItemDto,
  ProfileChangeActionEnum,
  ProfileSectionEnum,
  ReviewTargetTypeEnum
} from '../../models/profile-overview.model';
import { changeRequestDto } from '../../dtos/change-request-dto';
import { detectChangedFields, FieldChange } from '../../utils/detect-change-fields';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { TooltipModule } from 'primeng/tooltip';
import { Attachment } from '../../../wizard-profile/models/attachment.model';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileOverviewVisibility } from '../../services/profile-overview.visibility';
import { SponsorType } from '../../../../../../core/enums/lookups.enum';

export function formatChanges(
  changes: FieldChange[],
  labelMap: Record<string, string>
) {
  return changes.map(c => ({
    label: labelMap[c.field] ?? c.field,
    from: c.oldValue ?? '—',
    to: c.newValue ?? '—',
  }));
}
@Component({
  selector: 'app-profile-personal-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './personal-section.component.html',
  styleUrls: ['./personal-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePersonalSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() editableItems: MyProfileReviewChangedItemDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() canReplaceAttachment!: boolean;
  @Input() visibility: ProfileOverviewVisibility | null = null;
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    const showSponsor = this.visibility?.showSponsorSection ?? true;
    return [
      showSponsor
        ? { key: 'sponsorCard', titleKey: this.sponsorLabelKey('card'), file: p.sponsorCard ?? null }
        : null
    ]
      .filter((item): item is { key: string; titleKey: string; file: FileRefDto | null } => !!item)
      .filter(item => item.file);
  });

  protected showSponsorQidExpiry(): boolean {
    return !this.isCompanySponsor();
  }

  protected sponsorLabelKey(field: 'name' | 'number' | 'card'): string {
    const type = this.sponsorTypeKey();
    if (!type) {
      const fallback: Record<typeof field, string> = {
        name: 'profileOverview.fields.sponsorEmployerName',
        number: 'profileOverview.fields.sponsorEmployerNumber',
        card: 'profileOverview.attachments.sponsorCard'
      };
      return fallback[field];
    }

    return `profileOverview.fields.sponsor.${type}.${field}`;
  }

  private sponsorTypeKey(): 'individual' | 'company' | null {
    const backendName = this.profile?.sponsorType?.backendName;
    if (backendName === SponsorType.Company) return 'company';
    if (backendName === SponsorType.Individual) return 'individual';
    return null;
  }

  private isCompanySponsor(): boolean {
    return this.sponsorTypeKey() === 'company';
  }

  protected fieldUnderReview(fieldKey: string) {
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }

  protected noteForFile(file: FileRefDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!file?.resourceId) return null;
    return (
      this.notes.find(
        note =>
          note.targetType === ReviewTargetTypeEnum.Attachment &&
          note.resourceId?.toLowerCase() === file.resourceId.toLowerCase()
      ) ?? null
    );
  }

  protected canReplaceFile(file: FileRefDto | null | undefined): boolean {
    return !!this.noteForFile(file) || !!this.editableItemForFile(file);
  }

  private editableItemForFile(file: FileRefDto | null | undefined): MyProfileReviewChangedItemDto | null {
    if (!file?.resourceId) return null;
    return (
      this.editableItems.find(
        item =>
          item.targetType === ReviewTargetTypeEnum.Attachment &&
          item.resourceId?.toLowerCase() === file.resourceId.toLowerCase()
      ) ?? null
    );
  }
  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
  protected onEdit() {
    this.edit.emit();
  }

  protected replaceAttachment(att: { key: string; title?: string | null; file: FileRefDto | null }, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    const payload: any = {
      id: att.file?.resourceId,
      attachmentId: att.file?.resourceId,
      title: file.name
    };
    const reviewItem = this.noteForFile(att.file) ?? this.editableItemForFile(att.file);
    if (reviewItem) payload.reviewItemId = reviewItem.reviewItemId;
    const info: any = {}
    const files: any = {}
    switch (att.key) {
      case 'resumeAttachment':
        info['resume'] = payload;
        files['resume'] = file;
        break;
      case 'nationalCard':
        info['nationalCard'] = payload;
        files['nationalCard'] = file;
        break;
      case 'sponsorCard':
        info['sponsorCard'] = payload;
        files['sponsorCard'] = file;
        break;
    }

    this.profileService.savePersonalAttachmentsSection({ ...this.profile, ...info }, files).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.saved'));
        this.refresh.emit();
      }
    });
  }
}
