import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {FileRefDto, ProfileStatusDto} from '../../../../../../core/models/auth/auth-response.model';
import {MyProfileReviewNoteDto, ReviewTargetTypeEnum} from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {Tooltip} from 'primeng/tooltip';
import {Attachment} from '../../../wizard-profile/models/attachment.model';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {ProfileOverviewVisibility} from '../../../overview/services/profile-overview.visibility';

@Component({
  selector: 'app-profile-contact-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, Tooltip],
  templateUrl: './contact-section.component.html',
  styleUrls: ['./contact-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileContactSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Input() visibility: ProfileOverviewVisibility | null = null;
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    const showNationalAddress = this.visibility?.showNationalAddress ?? true;
    return [
      showNationalAddress
        ? { key: 'residenceAddressCertificate', titleKey: 'profileOverview.attachments.residenceAddressCertificate', file: p.residenceAddressCertificate ?? null }
        : null,
    ]
      .filter((item): item is { key: string; titleKey: string; file: FileRefDto | null } => !!item)
      .filter(item => item.file);
  });
  protected fieldUnderReview(fieldKey: string = ''){
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }
  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
  protected onEdit() {
    this.edit.emit();
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

  protected replaceAttachment(att: { key: string; title?: string | null; file: FileRefDto | null }, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    const payload: Attachment = {
      id: att.file?.resourceId,
      title: att.title ?? '',
      fileName: file.name,
      attachmentId: att.file?.resourceId,
      file,
    };

    this.profileService.saveAttachmentsSection([payload]).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.saved'));
        this.refresh.emit();
      }
    });
  }
}
