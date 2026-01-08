import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
  ExperienceDto,
  FileRefDto,
  ProfileStatusDto,
  QualificationDto
} from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../../overview/models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { DegreeModal } from '../../../components/profile-steps/step-degree/dialogs/degree.modal/degree.modal';
import { Degree } from '../../../wizard-profile/models/degree.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';

@Component({
  selector: 'app-profile-qualifications-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './qualifications-section.component.html',
  styleUrls: ['./qualifications-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileQualificationsSectionComponent {
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

  protected qualificationsUnderReview() {
    return this.changesRequest.map(i => i.newValue).map((cr, index) => {
      return {
        id: cr.Id,
        degreeId: cr.DegreeId,
        degree: this.lookups.degrees().find(d => d.id === cr.DegreeId) ?? null,
        universityId: cr.UniversityId,
        //university: this.lookups.universities().find(u => u.id === cr.UniversityId) ?? null,
        majorId: cr.MajorId,
        //major: this.lookups.majors().find(m => m.id === cr.MajorId) ?? null,
        subMajorId: cr.SubMajorId,
        //subMajor: this.lookups.subMajors().find(sm => sm.id === cr.SubMajorId) ?? null,
        graduationYear: cr.GradYear,
        studyTypeId: cr.StudyTypeId,
        studyType: this.lookups.studyTypes().find(st => st.id === cr.StudyTypeId) ?? null,
        gpa: cr.GPA,
        gradeId: cr.GradeId,
        grade: this.lookups.ratingGrades().find(g => g.id === cr.GradeId) ?? null,
        gradCountryId: cr.GradCountryId,
        gradCountry: this.lookups.countries().find(c => c.id === cr.GradCountryId) ?? null,
        attachment: { resourceId: cr.AttachmentResourceId, fileName: cr.FileName } as FileRefDto,
      } as QualificationDto;
    });
  }

  protected noteForRaw(qua: QualificationDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!qua?.id) return null;
    return (
      this.notes.find(
        note =>
          note.targetType === ReviewTargetTypeEnum.Row &&
          note.entityId?.toLowerCase() === qua.id.toLowerCase()
      ) ?? null
    );
  }

  protected addQualification() {
    this.dialogService
      .open(DegreeModal, {
        header: this.translate.instant('profileView.actions.addQualification'),
        width: '80%',
        contentStyle: { 'max-height': '80vh', overflow: 'auto' },
        baseZIndex: 10000,
        closable: true,
      })
      ?.onClose.subscribe((degree: Degree | null) => {
      if (!degree) return;
      this.profileService.saveEducationSection([degree]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    });
  }

  protected editQualification(qualification: QualificationDto) {
    const mapped = this.mapQualificationToDegree(qualification);
    this.dialogService
      .open(DegreeModal, {
        header: this.translate.instant('profileView.actions.editQualification'),
        width: '80%',
        contentStyle: { 'max-height': '80vh', overflow: 'auto' },
        baseZIndex: 10000,
        closable: true,
        data: { initialValue: mapped, disableFileUpload: true },
      })?.onClose.subscribe((degree: Degree | null) => {
      if (!degree) return;
      this.profileService.saveEducationSection([degree]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    });
  }

  private mapQualificationToDegree(qualification: QualificationDto): Degree {
    return {
      id: qualification.id,
      degreeId: qualification.degreeId,
      degree: qualification.degree ?? null,
      gradCountryId: qualification.gradCountryId,
      gradCountry: qualification.gradCountry ?? null,
      universityId: qualification.universityId,
      university: qualification.university ?? null,
      majorId: qualification.majorId,
      major: qualification.major ?? null,
      subMajorId: qualification.subMajorId,
      subMajor: qualification.subMajor ?? null,
      gradYear: qualification.graduationYear ?? 0,
      studyTypeId: qualification.studyTypeId,
      studySystem: qualification.studyType ?? null,
      gpa: qualification.gpa ?? 0,
      gradeId: qualification.gradeId,
      grade: qualification.grade ?? null,
      certificate: qualification.attachment
        ? {
            resourceId: qualification.attachment.resourceId,
            resourceName: qualification.attachment.fileName,
            url: qualification.attachment.url ?? null,
          }
        : null,
      attachmentId: qualification.attachment?.resourceId ?? null,
      fileName: qualification.attachment?.fileName ?? null,
    } as Degree;
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
