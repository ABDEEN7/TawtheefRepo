import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {ProfileStatusDto, SkillDto} from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto } from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';

@Component({
  selector: 'app-profile-skills-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './skills-section.component.html',
  styleUrls: ['./skills-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileSkillsSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Output() edit = new EventEmitter<void>();
  lookups = inject(ProfileLookupsService);

  protected skillsUnderReview(){
    return this.changesRequest.map(i=>i.newValue).map((cr,index)=>{
      return {
        skillId: cr.SkillId,
        // skill: this.lookups.searchSkills().find(s=> s.id == cr.SkillId),
        levelId: cr.LevelId,
        level: this.lookups.skillLevels().find(l=> l.id == cr.LevelId),
      } as SkillDto;
    })
  }
  protected fieldUnderReview(fieldKey: string = '') {
    if (!fieldKey) return (this.changesRequest ?? []).length > 0;
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }
  protected onEdit() {
    this.edit.emit();
  }
}
