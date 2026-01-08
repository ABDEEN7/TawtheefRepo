import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { FileRefDto, ProfileStatusDto, QualificationDto } from '../../../../../../core/models/auth/auth-response.model';
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
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  protected fieldUnderReview(fieldKey: string = ''){
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }

  protected noteForRecord(record: QualificationDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!record?.id) return null;
    return (
      this.notes.find(
        note =>
          note.targetType === ReviewTargetTypeEnum.Row &&
          note.entityId?.toLowerCase() === record.id.toLowerCase()
      ) ?? null
    );
  }

  protected addQualification() {
    this.lookups.loadAll().subscribe(() => {
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
    });
  }

  protected editQualification(qualification: QualificationDto) {
    const mapped = this.mapQualificationToDegree(qualification);
    this.lookups.loadAll().subscribe(() => {
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
    });
  }

  protected replaceQualificationFile(qualification: QualificationDto, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    const payload = this.mapQualificationToDegree(qualification);
    payload.file = file;
    payload.fileName = file.name;

    this.profileService.saveEducationSection([payload]).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.saved'));
        this.refresh.emit();
      },
      error: () => {
        this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
      }
    });
  }

  private mapQualificationToDegree(qualification: QualificationDto): Degree {
    return {
      id: qualification.id,
      degree: qualification.degree ?? null,
      gradCountry: qualification.gradCountry ?? null,
      university: qualification.university ?? null,
      major: qualification.major ?? null,
      subMajor: qualification.subMajor ?? null,
      gradYear: qualification.graduationYear ?? 0,
      studySystem: qualification.studyType ?? null,
      gpa: qualification.gpa ?? 0,
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
    };
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
