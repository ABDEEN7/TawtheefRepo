import { Component, inject, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { JobDetailsService } from '../services/job-details.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { GUID } from '../../../../shared/types/guid.type';
import { InvitationStatus } from '../../../../core/enums/lookups.enum';
import { JOB_INVITATION_STATUSES } from '../../dashboard/constants/constants';
import { DrawerModule } from 'primeng/drawer';
import { FileUtilsService } from '../../../../core/utils/file-utils';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-job-attachments',
  standalone: true,
  imports: [CommonModule, TranslatePipe, DrawerModule],
  templateUrl: './job-attachments.component.html'
})
export class JobAttachmentsComponent {
  invitationId = input.required<string | null>();
  private detailsService = inject(JobDetailsService);
  private notifier = inject(NotificationService);
  private translate = inject(TranslateService);
  private fileUtils = inject(FileUtilsService);
  private sanitizer = inject(DomSanitizer);

  job = this.detailsService.job;
  
  showPreview = signal(false);
  previewUrl = signal<SafeResourceUrl | null>(null);
  previewTitle = signal('');
  canModifiedAttachment() {
    const invitationStatus = this.job()?.invitationStatus?.backendName?.toLowerCase();
    if (invitationStatus != JOB_INVITATION_STATUSES.NEW_INVITATION &&
      invitationStatus != JOB_INVITATION_STATUSES.READ &&
      invitationStatus != JOB_INVITATION_STATUSES.PENDING_ATTACHMENT_APPROVAL &&
      invitationStatus != JOB_INVITATION_STATUSES.REQUIRES_UPDATE)
      return false;
    return true;
  }

  onFileSelected(event: any, jobRequiredAttachmentId: GUID) {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        this.notifier.error(this.translate.instant('JOB_DETAILS.FILE_SIZE_ERROR'));
        return;
      }
      this.detailsService.uploadInvitationAttachment(this.invitationId()!, jobRequiredAttachmentId, file).subscribe({
        next: () => {
          this.detailsService.loadJobDetails(this.invitationId()!).subscribe();
          this.notifier.success(this.translate.instant('JOB_DETAILS.UPLOAD_SUCCESS'));
        },
        error: () => {
          this.notifier.error(this.translate.instant('JOB_DETAILS.UPLOAD_ERROR'));
        }
      });
    }
  }

  deleteAttachment(attachmentId: GUID) {
    this.detailsService.deleteInvitationAttachment(this.invitationId()!, attachmentId).subscribe({
      next: () => {
        this.detailsService.loadJobDetails(this.invitationId()!).subscribe();
        this.notifier.success(this.translate.instant('JOB_DETAILS.DELETE_SUCCESS'));
      },
      error: () => {
        this.notifier.error(this.translate.instant('JOB_DETAILS.DELETE_ERROR'));
      }
    });
  }

  async previewAttachment(attachment: any) {
    this.previewTitle.set(attachment.resourceName || attachment.title);
    this.showPreview.set(true);
    this.previewUrl.set(null); // Reset

    try {
      let blobToPreview: Blob;
      
      if (attachment.file && (attachment.file instanceof File || attachment.file instanceof Blob)) {
        blobToPreview = attachment.file;
      } else if (attachment.resourceUrl) {
        // Use FileUtils to fetch as blob with auth
        blobToPreview = await this.fileUtils['http'].get<Blob>(attachment.resourceUrl, undefined, { responseType: 'blob' }).toPromise() as Blob;
      } else {
        this.showPreview.set(false);
        return;
      }

      const url = URL.createObjectURL(blobToPreview);
      this.previewUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(url));
    } catch (err) {
      this.notifier.error(this.translate.instant('JOB_DETAILS.PREVIEW_ERROR'));
      this.showPreview.set(false);
    }
  }
}
