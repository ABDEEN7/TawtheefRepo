import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewChangedItemDto, MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileOverviewVisibility } from '../../services/profile-overview.visibility';

@Component({
  selector: 'app-profile-prerequisites-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './prerequisites-section.component.html',
  styleUrls: ['./prerequisites-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePrerequisitesSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly translate = inject(TranslateService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() editableItems: MyProfileReviewChangedItemDto[] = [];
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();
  @Input() canReplaceAttachment!: boolean;
  @Input() visibility: ProfileOverviewVisibility | null = null;
  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    const needsBirth = this.visibility?.needsBirth ?? true;
    const needsMarriage = this.visibility?.needsMarriage ?? true;
    return [
      { key: 'resumeAttachment', titleKey: 'profileOverview.attachments.resume', file: p.resumeAttachment ?? null },
      { key: 'nationalCard', titleKey: 'profileOverview.attachments.nationalCard', file: p.nationalCard ?? null },
      needsBirth
        ? { key: 'birthdayCertificate', titleKey: 'profileOverview.attachments.birthdayCertificate', file: p.birthdayCertificate ?? null }
        : null,
      needsMarriage
        ? { key: 'marriageCertificate', titleKey: 'profileOverview.attachments.marriageCertificate', file: p.marriageCertificate ?? null }
        : null,
    ]
      .filter((item): item is { key: string; titleKey: string; file: FileRefDto | null } => !!item)
      .filter(item => item.file);
  });
  @Input() changesRequest!: FieldChange[];

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

    const keyMap: Record<string, string> = {
      'resumeAttachment': 'resume',
      'nationalCard': 'nationalCard',
      'birthdayCertificate': 'birth',
      'marriageCertificate': 'marriage'
    };

    const field = keyMap[att.key];
    if (field) {
      info[field] = payload;
      files[field] = file;

      const obs = (field === 'resume' || field === 'nationalCard')
        ? this.profileService.savePersonalAttachmentsSection({ ...this.profile, ...info }, files)
        : this.profileService.savePereqAttachmentsSection({ ...this.profile, ...info }, files);

      obs.subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    }
  }
}
