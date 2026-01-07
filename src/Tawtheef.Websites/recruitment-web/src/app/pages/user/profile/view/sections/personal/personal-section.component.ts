import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {FileRefDto, ProfileStatusDto} from '../../../../../../core/models/auth/auth-response.model';
import {
  MyProfileReviewNoteDto,
  ProfileChangeActionEnum,
  ProfileSectionEnum,
  ReviewTargetTypeEnum
} from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {detectChangedFields, FieldChange} from '../../utils/detect-change-fields';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import { TooltipModule } from 'primeng/tooltip';

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
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Output() edit = new EventEmitter<void>();

  attachments = computed(() => {
    const p = this.profile;
    if (!p) return [] as { key: string; titleKey: string; file: FileRefDto | null }[];
    return [
      { key: 'resumeAttachment', titleKey: 'profileOverview.attachments.resume', file: p.resumeAttachment ?? null },
      { key: 'nationalCard', titleKey: 'profileOverview.attachments.nationalCard', file: p.nationalCard ?? null },
      { key: 'sponsorCard', titleKey: 'profileOverview.attachments.sponsorCard', file: p.sponsorCard ?? null }
    ].filter(p => p.file);
  });

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
}
