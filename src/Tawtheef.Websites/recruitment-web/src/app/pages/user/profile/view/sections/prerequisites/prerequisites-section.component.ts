import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import {Attachment} from '../../../wizard-profile/models/attachment.model';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {NotificationService} from '../../../../../../core/services/notification.service';

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
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();
  @Input() isProfileApproved!: boolean;
  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    return [
      { key: 'birthdayCertificate', titleKey: 'profileOverview.attachments.birthdayCertificate', file: p.birthdayCertificate ?? null },
      { key: 'marriageCertificate', titleKey: 'profileOverview.attachments.marriageCertificate', file: p.marriageCertificate ?? null },
    ].filter(p => p.file);
  });
  @Input() changesRequest!: FieldChange[];

  protected fieldUnderReview(fieldKey: string){
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
