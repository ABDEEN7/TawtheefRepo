import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewNoteDto } from '../../../overview/models/profile-overview.model';

@Component({
  selector: 'app-profile-attachments-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './attachments-section.component.html',
  styleUrls: ['./attachments-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileAttachmentsSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Output() edit = new EventEmitter<void>();

  protected fieldUnderReview = computed(() => (this.notes?.length ?? 0) > 0);

  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }

  protected onEdit() {
    this.edit.emit();
  }
}
