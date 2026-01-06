import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto } from '../../../overview/models/profile-overview.model';

@Component({
  selector: 'app-profile-experience-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './experience-section.component.html',
  styleUrls: ['./experience-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileExperienceSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Output() edit = new EventEmitter<void>();

  protected fieldUnderReview = computed(() => (this.notes?.length ?? 0) > 0);

  protected onEdit() {
    this.edit.emit();
  }
}
