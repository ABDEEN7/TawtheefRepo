import {ChangeDetectionStrategy, Component, EventEmitter, Input, Output, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {
  AchievementDto, FileRefDto,
  ProfileStatusDto,
  QualificationDto, TrainingCourseDto
} from '../../../../../../core/models/auth/auth-response.model';
import {MyProfileReviewNoteDto, ReviewTargetTypeEnum} from '../../../overview/models/profile-overview.model';
import {changeRequestDto} from '../../dtos/change-request-dto';
import {FieldChange} from '../../utils/detect-change-fields';
import {DegreeModal} from '../../../components/profile-steps/step-degree/dialogs/degree.modal/degree.modal';
import {Degree} from '../../../wizard-profile/models/degree.model';
import {DialogService} from 'primeng/dynamicdialog';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {CourseModal} from '../../../components/profile-steps/step-experience/dialogs/course.modal/course.modal';
import {TrainingCourse} from '../../../wizard-profile/models/experience.model';
import {Tooltip} from 'primeng/tooltip';
import {UploadedFileRef} from '../../../wizard-profile/models/profile-state.model';

@Component({
  selector: 'app-profile-training-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, Tooltip],
  templateUrl: './training-section.component.html',
  styleUrls: ['./training-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileTrainingSectionComponent {
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  protected coursesUnderReview() {
    return this.changesRequest.map(i => i.newValue).map((cr, index) => {
      return {
        title: cr.Title,
        provider: cr.Provider,
        countryId: cr.CountryId,
        country: this.lookups.countries().find(c => c.id === cr.CountryId) ?? null,
        startDate: cr.StartDate,
        endDate: cr.EndDate,
        description: cr.Description,
        attachment: {resourceId: cr.AttachmentResourceId, fileName: cr.FileName} as any,
      } as TrainingCourse;
    });
  }

  protected fieldUnderReview(fieldKey: string = '') {
    if (!fieldKey) return (this.changesRequest ?? []).length > 0;
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }


  protected noteForRaw(course: TrainingCourse | null | undefined): MyProfileReviewNoteDto | null {
    if (!course?.id) return null;
    return (
      this.notes.find(
        note =>
          note.targetType === ReviewTargetTypeEnum.Row &&
          note.entityId?.toLowerCase() === course.id?.toLowerCase()
      ) ?? null
    );
  }

  protected addCourseTraining() {
    this.dialogService
      .open(CourseModal, {
        header: this.translate.instant('profileView.actions.addQualification'),
        width: '80%',
        contentStyle: {'max-height': '80vh', overflow: 'auto'},
        baseZIndex: 10000,
        closable: true,
      })?.onClose.subscribe((course: TrainingCourse | null) => {
      if (!course) return;
      this.profileService.saveExperienceSection([], [course]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    });
  }


  protected editCourseTraining(course: TrainingCourseDto) {
    const mapped = this.mapTrainingCourse(course);
    this.dialogService
      .open(CourseModal, {
        header: this.translate.instant('profileView.actions.editTrainingCourse'),
        width: '80%',
        contentStyle: {'max-height': '80vh', overflow: 'auto'},
        baseZIndex: 10000,
        closable: true,
        data: {initialValue: mapped, disableFileUpload: true},
      })?.onClose.subscribe((course: TrainingCourse | null) => {
      if (!course) return;
      this.profileService.saveExperienceSection([], [course]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    });
  }

  private mapTrainingCourse(qualification: TrainingCourseDto): TrainingCourse {
    return {
      id: qualification.id,
      title: qualification.title ?? '',
      provider: qualification.provider ?? '',
      country: qualification.country ?? null,
      from: qualification.startDate ?? null,
      to: qualification.endDate ?? null,
      description: qualification.description ?? '',
      attachment: qualification.attachment == null ? null : {
          resourceId: qualification.attachment.resourceId,
          resourceName: qualification.attachment.fileName
        } as UploadedFileRef
    } as TrainingCourse;
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
