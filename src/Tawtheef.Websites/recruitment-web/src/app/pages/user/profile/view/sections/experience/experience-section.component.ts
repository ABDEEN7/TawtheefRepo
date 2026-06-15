import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
  ExperienceDto,
  FileRefDto,
  ProfileStatusDto,
} from '../../../../../../core/models/auth/auth-response.model';
import {
  MyProfileReviewNoteDto,
  MyProfileReviewChangedItemDto,
  ReviewTargetTypeEnum,
} from '../../models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ExperienceModal } from '../../../components/profile-steps/step-experience/dialogs/experience.modal/experience.modal';
import { Experience } from '../../../wizard-profile/models/experience.model';
import { Degree } from '../../../wizard-profile/models/degree.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';

type PendingExperienceChange = {
  EmployerName?: string;
  JobTitle?: string;
  CountryId?: string | number | null;
  StartDate?: string | null;
  EndDate?: string | null;
  IsCurrent?: boolean | null;
  Description?: string | null;
  QualificationId?: string | number | null;
  AttachmentResourceId?: string | null;
  FileName?: string | null;
};

@Component({
  selector: 'app-profile-experience-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './experience-section.component.html',
  styleUrls: ['./experience-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class ProfileExperienceSectionComponent {
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() editableItems: MyProfileReviewChangedItemDto[] = [];
  @Input() changesRequest: FieldChange[] = [];
  @Input() isProfileApproved = false;

  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  protected experiencesUnderReview(): ExperienceDto[] {
    return this.normalizePendingItems().map((cr) => {
      return {
        employerName: cr.EmployerName ?? '',
        jobTitle: cr.JobTitle ?? '',
        countryId: cr.CountryId ?? null,
        country: this.lookups.graduationCountry().find((c) => c.id === cr.CountryId) ?? null,
        startDate: cr.StartDate ?? null,
        endDate: cr.EndDate ?? null,
        isCurrent: !!cr.IsCurrent,
        description: cr.Description ?? null,
        qualificationId: cr.QualificationId ?? null,
        qualification:
          this.profile?.qualifications?.find((q) => q.id === cr.QualificationId) ?? null,
        attachment: cr.AttachmentResourceId
          ? ({
              resourceId: cr.AttachmentResourceId,
              fileName: cr.FileName ?? '',
            } as FileRefDto)
          : (null as any),
      } as ExperienceDto;
    });
  }

  protected noteForRaw(exp: ExperienceDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!exp) return null;

    const rowNote = this.rowNoteForRaw(exp);

    const attachmentNote =
      exp.attachment?.resourceId
        ? this.notes.find(
            (note) =>
              note.targetType === ReviewTargetTypeEnum.Attachment &&
              note.resourceId?.toLowerCase() === exp.attachment?.resourceId.toLowerCase()
          ) ?? null
        : null;

    return rowNote ?? attachmentNote ?? null;
  }

  protected canEditRaw(exp: ExperienceDto | null | undefined): boolean {
    return !!this.noteForRaw(exp) || !!this.editableItemForRaw(exp);
  }

  protected canDeleteRaw(exp: ExperienceDto | null | undefined): boolean {
    const rowNote = this.rowNoteForRaw(exp);
    const rowItem = this.editableRowItemForRaw(exp);
    return !!exp?.id && (!!rowNote || !!rowItem);
  }

  protected deleteExperience(exp: ExperienceDto): void {
    if (!exp.id) return;
    if (!window.confirm(this.translate.instant('profileView.confirmDeleteRow'))) return;

    this.profileService.deleteExperience(exp.id).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.deleted'));
        this.refresh.emit();
      },
      error: () => {
        this.notify.error(this.translate.instant('profileView.notifications.deleteFailed'));
      },
    });
  }

  private rowNoteForRaw(exp: ExperienceDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!exp?.id) return null;
    return this.notes.find(
      note =>
        note.targetType === ReviewTargetTypeEnum.Row &&
        note.entityId?.toLowerCase() === exp.id?.toLowerCase()
    ) ?? null;
  }

  private editableItemForRaw(exp: ExperienceDto | null | undefined): MyProfileReviewChangedItemDto | null {
    if (!exp) return null;

    const rowItem = this.editableRowItemForRaw(exp);

    const attachmentItem =
      exp.attachment?.resourceId
        ? this.editableItems.find(
            item =>
              item.targetType === ReviewTargetTypeEnum.Attachment &&
              item.resourceId?.toLowerCase() === exp.attachment?.resourceId.toLowerCase()
          ) ?? null
        : null;

    return rowItem ?? attachmentItem ?? null;
  }

  private editableRowItemForRaw(exp: ExperienceDto | null | undefined): MyProfileReviewChangedItemDto | null {
    if (!exp?.id) return null;
    return this.editableItems.find(
      item =>
        item.targetType === ReviewTargetTypeEnum.Row &&
        item.entityId?.toLowerCase() === exp.id?.toLowerCase()
    ) ?? null;
  }

  protected addExperience() {
    this.dialogService
      .open(ExperienceModal, {
        header: this.translate.instant('profileView.actions.addExperience'),
        width: '50%',
        contentStyle: { 'max-height': '80vh', overflow: 'auto' },
        baseZIndex: 10000,
        closable: true,
        data: {
          degrees: this.mapDegrees(),
          // create mode => do not disable upload
          disableFileUpload: false,
        },
      })
      ?.onClose.subscribe((experience: Experience | null) => {
        if (!experience) return;

        this.profileService.saveExperienceSection([experience], []).subscribe({
          next: () => {
            this.notify.success(this.translate.instant('profileView.notifications.saved'));
            this.refresh.emit();
          },
          error: () => {
            this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
          },
        });
      });
  }

  protected editExperience(exp: ExperienceDto) {
    const initialValue = {
      employerName: exp.employerName,
      jobTitle: exp.jobTitle,
      country: exp.country ?? null,
      from: exp.startDate ?? null,
      to: exp.endDate ?? null,
      current: exp.isCurrent ?? false,
      description: exp.description ?? '',
      fileName: exp.attachment?.fileName ?? '',
      attachment: exp.attachment
        ? {
            resourceId: exp.attachment.resourceId,
            resourceName: exp.attachment.fileName,
            url: exp.attachment.url ?? null,
          }
        : null,
      qualificationId: exp.qualificationId ?? null,
      id: exp.id ?? null,
      attachmentId: exp.attachment?.resourceId ?? null,
    };

    this.dialogService
      .open(ExperienceModal, {
        header: this.translate.instant('profileView.actions.editExperience'),
        width: '50%',
        contentStyle: { 'max-height': '80vh', overflow: 'auto' },
        baseZIndex: 10000,
        closable: true,
        data: {
          degrees: this.mapDegrees(),
          initialValue,
          initialId: exp.id,
          attachmentId: exp.attachment?.resourceId ?? null,
          // edit mode in your case => prevent changing the file
          disableFileUpload: true,
        },
      })
      ?.onClose.subscribe((experience: Experience | null) => {
        if (!experience) return;

        this.profileService.saveExperienceSection([experience], []).subscribe({
          next: () => {
            this.notify.success(this.translate.instant('profileView.notifications.saved'));
            this.refresh.emit();
          },
          error: () => {
            this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
          },
        });
      });
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }

  private mapDegrees(): Degree[] {
    return (this.profile?.qualifications ?? []).map((q) => ({
      id: q.id,
      degree: q.degree ?? null,
      gradCountry: q.gradCountry ?? null,
      university: q.university ?? null,
      major: q.major ?? null,
      subMajor: q.subMajor ?? null,
      gradYear: q.graduationYear ?? 0,
      studySystem: q.studyType ?? null,
      gpa: q.gpa ?? 0,
      grade: q.grade ?? null,
      certificate: q.attachment
        ? {
            resourceId: q.attachment.resourceId,
            resourceName: q.attachment.fileName,
            url: q.attachment.url ?? null,
          }
        : null,
      attachmentId: q.attachment?.resourceId ?? null,
      fileName: q.attachment?.fileName ?? undefined,
    }));
  }

  private normalizePendingItems(): PendingExperienceChange[] {
    const items = this.changesRequest
      .map((change) => change.newValue)
      .flatMap((value) => {
        if (!value) return [];
        return Array.isArray(value) ? value : [value];
      }) as PendingExperienceChange[];

    return items.filter((item) => !!item?.EmployerName || !!item?.JobTitle);
  }
}
