import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { MyProfileReviewNoteDto } from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';

@Component({
  selector: 'app-profile-prerequisites-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, ButtonDirective],
  templateUrl: './prerequisites-section.component.html',
  styleUrls: ['./prerequisites-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfilePrerequisitesSectionComponent {
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Output() edit = new EventEmitter<void>();
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
  open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
  protected onEdit() {
    this.edit.emit();
  }
}
