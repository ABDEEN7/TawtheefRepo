import { ChangeDetectionStrategy, Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { FileRefDto, ProfileStatusDto } from '../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../core/utils/file-utils';

@Component({
  selector: 'app-profile-attachments-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, ButtonDirective],
  templateUrl: './attachments-section.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileAttachmentsSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;

  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
