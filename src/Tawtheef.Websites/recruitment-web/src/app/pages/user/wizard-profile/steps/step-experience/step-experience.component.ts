import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ExperienceModal } from './dialogs/experience.modal/experience.modal';
import { CourseModal } from './dialogs/course.modal/course.modal';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';
import {ProfileService} from '../../services/profile.service';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {Experience, TrainingCourse} from '../../models/experience.model';

@Component({
  selector: 'app-step-experience',
  templateUrl: './step-experience.component.html',
  styleUrl: './step-experience.component.scss',
  standalone: false,
})
export class StepExperienceComponent {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  messageService = inject(MessageService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = false;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['experience'];
  }
  // ========== EXPERIENCES ==========

  addExperience() {
    this.dialog.open(ExperienceModal, {
      header: this.translate.instant('wizard.experience.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe(result => {
      if (result) {
        this.ds.addExp(result);
      }
    });
  }

  removeExperience(index: number) {
    var exp = this.ds.state().experiences[index];
    if(exp.id){
      this.profile.deleteExperience(exp.id).subscribe({
        next: () => {
          this.ds.delExp(index);
        },
        error: err => {
          console.error(err);
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.experience.deleteError'),
            life: 5000,
          });
        },
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

  removeCourse(index: number) {
    const course = this.ds.state().courses[index];
    if(course.id){
      this.profile.deleteTrainingCourse(course.id).subscribe({
        next: () => {
          this.ds.delCourse(index);
        },
        error: err => {
          console.error(err);
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.course.deleteError'),
            life: 5000,
          });
        },
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
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }
    const state = this.ds.state();
    const experiences = state.experiences || [];
    const courses = state.courses || [];
    if (!experiences.length) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant('wizard.experience.validation.noRows'),
        life: 5000,
      });
      return;
    }

    this.saving = true;
    this.profile.saveExperienceSection(experiences, courses).subscribe({
      next: () => {
        this.saving = false;
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('wizard.errorTitle'),
          detail: this.translate.instant('wizard.experience.saveError'),
          life: 5000,
        });
      },
    });
  }

  private previewAttachment(ref?: { url?: string | null; resourceName?: string | null } | null, file?: File | null, fallbackName?: string | null, ev?: Event) {
    ev?.stopPropagation();
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    if (ref?.url) {
      this.fileUtils.previewUrl(ref.url, ref.resourceName || fallbackName || '', false);
    }
  }
}
