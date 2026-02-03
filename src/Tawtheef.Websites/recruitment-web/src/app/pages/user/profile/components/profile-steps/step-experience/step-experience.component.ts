import {Component, EventEmitter, inject, isDevMode, OnInit, Output} from '@angular/core';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from '@ngx-translate/core';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {ExperienceModal} from './dialogs/experience.modal/experience.modal';
import {Experience, TrainingCourse} from '../../../wizard-profile/models/experience.model';
import {CourseModal} from './dialogs/course.modal/course.modal';
import {UploadedFileRef} from '../../../wizard-profile/models/profile-state.model';


@Component({
  selector: 'app-step-experience',
  templateUrl: './step-experience.component.html',
  styleUrl: './step-experience.component.scss',
  standalone: false
})
export class StepExperienceComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  notify = inject(NotificationService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['experience'];
  }

  ngOnInit(): void {
    const state = this.ds.state();
    const signature = this.buildSignature(state.experiences, state.courses);
    this.lastSubmittedSignature = null;
  }
  // ========== EXPERIENCES ==========

  addExperience() {
    this.dialog.open(ExperienceModal, {
      header: this.translate.instant('wizard.experience.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { degrees: this.ds.state().degrees },
    })?.onClose.subscribe(result => {
      if (result) {
        this.ds.addExp(result);
      }
    });
  }

  editExperience(index: number) {
    const experience = this.ds.state().experiences[index];
    this.dialog.open(ExperienceModal, {
      header: this.translate.instant('wizard.experience.edit'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { degrees: this.ds.state().degrees, initialValue: experience },
    })?.onClose.subscribe(result => {
      if (result) {
        this.ds.updateExp(index, result);
      }
    });
  }

  removeExperience(index: number) {
    var exp = this.ds.state().experiences[index];
    if(exp.id){
      this.profile.deleteExperience(exp.id).subscribe({
        next: () => {
          this.ds.delExp(index);
        }
      });
    } else {
      this.ds.delExp(index);
    }
  }

  previewExperience(exp: Experience, ev?: Event): void {
    this.previewAttachment(exp.attachment, exp.file, exp.fileName, ev);
  }

  // ========== COURSES ==========

  addCourse() {
    this.dialog.open(CourseModal, {
      header: this.translate.instant('wizard.courses.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe(result => {
      if (result) {
        this.ds.addCourse(result);
      }
    });
  }

  editCourse(index: number) {
    const course = this.ds.state().courses[index];
    this.dialog.open(CourseModal, {
      header: this.translate.instant('wizard.courses.edit'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { initialValue: course },
    })?.onClose.subscribe(result => {
      if (result) {
        this.ds.updateCourse(index, result);
      }
    });
  }

  removeCourse(index: number) {
    const course = this.ds.state().courses[index];
    if(course.id){
      this.profile.deleteTrainingCourse(course.id).subscribe({
        next: () => {
          this.ds.delCourse(index);
        }
      });
    } else {
      this.ds.delCourse(index);
    }
  }

  previewCourse(course: TrainingCourse, ev?: Event): void {
    this.previewAttachment(course.attachment, course.file, course.fileName, ev);
  }

  onNext() {
    if (!this.step.valid) {
      this.notify.error(
        `${this.translate.instant('wizard.validationErrorTitle')}: ${this.step.errors
          .map(e => `* ${this.translate.instant(e.i18nKey)}`)
          .join('\n')}`,
      );
      return;
    }
    const state = this.ds.state();
    const experiences = state.experiences || [];
    const courses = state.courses || [];
    const signature = this.buildSignature(experiences, courses);

    if (signature && signature === this.lastSubmittedSignature) {
      this.notify.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    this.saving = true;
    this.profile.saveExperienceSection(experiences, courses).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        if (this.profile.isChangeRequestMode()) {
          this.notify.success(this.translate.instant('profileView.notifications.changeRequestSent'));
        }
        this.next.emit();
      },
      error: (err: any) => {
        if(isDevMode())
          console.error(err);
        this.saving = false;
      },
    });
  }

  private previewAttachment(ref?: UploadedFileRef | null, file?: File | null, fallbackName?: string | null, ev?: Event) {
    ev?.stopPropagation();
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    if (ref?.url) {
      this.fileUtils.previewUrl(ref.url, ref.resourceName || fallbackName || '', false);
    }else if(ref?.file){
      this.fileUtils.previewBlob(ref!.file);
    }
  }

  private buildSignature(experiences: Experience[], courses: TrainingCourse[]): string {
    const experienceSignature = (experiences ?? []).map(e => ({
      id: e.id ?? null,
      employerName: e.employerName ?? '',
      jobTitle: e.jobTitle ?? '',
      from: e.from ?? null,
      to: e.to ?? null,
      countryId: e.country?.id ?? null,
      current: e.current ?? false,
      description: e.description ?? '',
      attachmentId: e.attachmentId ?? null,
      fileName: e.file?.name ?? e.attachment?.resourceName ?? null,
      qualificationId: e.qualificationId ?? null,
    }));

    const courseSignature = (courses ?? []).map(c => ({
      id: c.id ?? null,
      title: c.title ?? '',
      provider: c.provider ?? '',
      from: c.from ?? null,
      to: c.to ?? null,
      countryId: c.country?.id ?? null,
      description: c.description ?? '',
      attachmentId: c.attachmentId ?? null,
      fileName: c.file?.name ?? c.attachment?.resourceName ?? null,
    }));

    return JSON.stringify({ experienceSignature, courseSignature });
  }

  get calculatedExperienceYears(): number {
    const experiences = this.ds.state().experiences ?? [];
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const ranges = experiences
      .map(exp => {
        if (!exp.from) return null;
        const start = new Date(exp.from);
        const end = exp.current ? today : exp.to ? new Date(exp.to) : today;
        if (isNaN(start.getTime()) || isNaN(end.getTime()) || start.getTime() > end.getTime()) {
          return null;
        }
        start.setHours(0, 0, 0, 0);
        end.setHours(0, 0, 0, 0);
        return { start, end };
      })
      .filter((r): r is { start: Date; end: Date } => !!r)
      .sort((a, b) => a.start.getTime() - b.start.getTime());

    if (!ranges.length) return 0;

    let mergedStart = ranges[0].start;
    let mergedEnd = ranges[0].end;
    let totalMs = 0;

    for (let i = 1; i < ranges.length; i++) {
      const current = ranges[i];
      if (current.start.getTime() <= mergedEnd.getTime()) {
        if (current.end.getTime() > mergedEnd.getTime()) {
          mergedEnd = current.end;
        }
      } else {
        totalMs += mergedEnd.getTime() - mergedStart.getTime();
        mergedStart = current.start;
        mergedEnd = current.end;
      }
    }

    totalMs += mergedEnd.getTime() - mergedStart.getTime();

    const years = totalMs / (1000 * 60 * 60 * 24 * 365.25);
    return Math.round(years * 10) / 10;
  }
}
