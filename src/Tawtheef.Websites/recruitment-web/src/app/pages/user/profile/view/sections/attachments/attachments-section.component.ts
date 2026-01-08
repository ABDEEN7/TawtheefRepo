import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../../overview/models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { Attachment } from '../../../wizard-profile/models/attachment.model';
import { AddAttachmentDialogComponent } from './dialogs/add-attachment-dialog.component';

@Component({
  selector: 'app-profile-attachments-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './attachments-section.component.html',
  styleUrls: ['./attachments-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileAttachmentsSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    return p.additionalAttachments?.map((a,index)=> {
      return { key: `additionalAttachments[${index}]`, titleKey: a.title, file: a.file ?? null }
    }) || [];
  });
  protected fieldUnderReview(fieldKey: string = ''){
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

  protected addAttachment() {
    this.dialogService
      .open(AddAttachmentDialogComponent, {
        header: this.translate.instant('profileView.actions.addAttachment'),
        styleClass: 'w-100 w-md-50'
      })
      ?.onClose.subscribe((attachment: Attachment | null | undefined) => {
        if (!attachment) return;
        this.profileService.saveAttachmentsSection([attachment]).subscribe({
          next: () => {
            this.notify.success(this.translate.instant('profileView.notifications.saved'));
            this.refresh.emit();
          }
        });
      });
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
