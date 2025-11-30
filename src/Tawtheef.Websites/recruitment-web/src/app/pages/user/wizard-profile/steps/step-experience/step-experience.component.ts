import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ExperienceModal } from './dialogs/experience.modal/experience.modal';
import { CourseModal } from './dialogs/course.modal/course.modal';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';

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
    this.ds.delExp(index);
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
    this.ds.delCourse(index);
  }
}
