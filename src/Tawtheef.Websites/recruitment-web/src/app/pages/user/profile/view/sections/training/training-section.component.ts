import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto } from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';

@Component({
  selector: 'app-profile-training-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './training-section.component.html',
  styleUrls: ['./training-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileTrainingSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Output() edit = new EventEmitter<void>();

  protected fieldUnderReview(fieldKey: string = ''){
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }
  protected onEdit() {
    this.edit.emit();
  }
}
