import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewChangedItemDto, MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TranslateService } from '@ngx-translate/core';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { Attachment } from '../../../wizard-profile/models/attachment.model';
import { AddAttachmentDialogComponent } from './dialogs/add-attachment-dialog.component';
import { GUID } from '../../../../../../shared/types/guid.type';

type ProfileAttachmentView = {
  id?: GUID | null;
  key: string;
  titleKey?: string | null;
  file: FileRefDto | null;
};

@Component({
  selector: 'app-profile-attachments-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule, ConfirmDialogModule],
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
  private readonly confirmationService = inject(ConfirmationService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() editableItems: MyProfileReviewChangedItemDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as ProfileAttachmentView[];
    const approvedItems: ProfileAttachmentView[] = p.additionalAttachments?.map((a,index)=> {
      return { id: a.id, key: `additionalAttachments[${index}]`, titleKey: a.title, file: a.file ?? null }
    }) || [];

    const underReview: ProfileAttachmentView[] = this.changesRequest.map(i=>i.newValue).map((cr,index)=>{
      return { id: null, key: `additionalAttachmentsNew[${index}]`, titleKey: cr.Title, file: {resourceId: cr.AttachmentResourceId, fileName: cr.FileName} as FileRefDto };
    })
    return approvedItems.concat(underReview) ;
  });
  protected fieldUnderReview(fieldKey: string | null | undefined): boolean {
    if (!fieldKey) return (this.changesRequest ?? []).length > 0;
    return this.changesRequest.find(cr=> cr.newValue.AttachmentResourceId?.toLowerCase() === fieldKey.toLowerCase()) !== undefined;
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

  protected canDeleteAttachment(att: ProfileAttachmentView): boolean {
    return !!att.id && (!!this.noteForFile(att.file) || !!this.editableItemForFile(att.file));
  }

  protected deleteAttachment(att: ProfileAttachmentView): void {
    if (!att.id) return;
    this.confirmDelete(() => this.executeDeleteAttachment(att));
  }

  private executeDeleteAttachment(att: ProfileAttachmentView): void {
    if (!att.id) return;
    this.profileService.deleteAttachment(att.id).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.deleted'));
        this.refresh.emit();
      },
      error: () => {
        this.notify.error(this.translate.instant('profileView.notifications.deleteFailed'));
      },
    });
  }

  private confirmDelete(accept: () => void): void {
    this.confirmationService.confirm({
      header: this.translate.instant('wizard.buttons.delete'),
      message: this.translate.instant('profileView.confirmDeleteRow'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('common.yes'),
      rejectLabel: this.translate.instant('common.no'),
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept,
    });
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

  protected addAttachment() {
    this.dialogService
      .open(AddAttachmentDialogComponent, {
        header: this.translate.instant('profileView.actions.addAttachment'),
        styleClass: 'w-50'
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

  protected replaceAttachment(att: ProfileAttachmentView, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    const payload: Attachment = {
      id: att.file?.resourceId,
      title: att.titleKey ?? '',
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
