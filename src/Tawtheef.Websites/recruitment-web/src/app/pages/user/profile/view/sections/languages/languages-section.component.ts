import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {FileRefDto, LanguageDto, ProfileStatusDto} from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto } from '../../models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';

@Component({
  selector: 'app-profile-languages-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './languages-section.component.html',
  styleUrls: ['./languages-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileLanguagesSectionComponent {
  @Input() profile: ProfileStatusDto | null = null;
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Output() edit = new EventEmitter<void>();

  lookups = inject(ProfileLookupsService);

  protected languageUnderReview(){
    return this.changesRequest.map(i=>i.newValue).map((cr,index)=>{
      return {
        id: cr.id,
        languageId: cr.LanguageId,
        language: this.lookups.languages().find(l=> l.id == cr.LanguageId),
        readingLevelId: cr.ReadingLevelId,
        readingLevel:this.lookups.languageLevels().find(l=> l.id == cr.ReadingLevelId),
        writingLevelId: cr.WritingLevelId,
        writingLevel: this.lookups.languageLevels().find(l=> l.id == cr.WritingLevelId),
        speakingLevelId: cr.SpeakingLevelId,
        speakingLevel: this.lookups.languageLevels().find(l=> l.id == cr.SpeakingLevelId),
      } as LanguageDto;
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
