import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ExperienceModal } from './dialogs/experience.modal/experience.modal';
import { CourseModal } from './dialogs/course.modal/course.modal';
import { AchievementModal } from './dialogs/achievement.modal/achievement.modal';
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
    const ref = this.dialog.open(ExperienceModal, {
      header: this.translate.instant('wizard.experience.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        this.ds.addExp(result);
      }
    });
  }

  editExperience(index: number) {
    const current = this.ds.state().experiences[index];
    const ref = this.dialog.open(ExperienceModal, {
      header: this.translate.instant('wizard.experience.edit'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
      data: {
        initialValue: current,
      },
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        const list = [...this.ds.state().experiences];
        list[index] = result;
        this.ds.up('experiences', list as any);
      }
    });
  }

  removeExperience(index: number) {
    this.ds.delExp(index);
  }

  // ========== COURSES ==========

  addCourse() {
    const ref = this.dialog.open(CourseModal, {
      header: this.translate.instant('wizard.courses.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        this.ds.addCourse(result);
      }
    });
  }

  editCourse(index: number) {
    const current = this.ds.state().courses[index];
    const ref = this.dialog.open(CourseModal, {
      header: this.translate.instant('wizard.courses.edit'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
      data: {
        initialValue: current,
      },
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        const list = [...this.ds.state().courses];
        list[index] = result;
        this.ds.up('courses', list as any);
      }
    });
  }

  removeCourse(index: number) {
    this.ds.delCourse(index);
  }

  // ========== ACHIEVEMENTS ==========

  addAchievement() {
    const ref = this.dialog.open(AchievementModal, {
      header: this.translate.instant('wizard.achievement.add'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        this.ds.addAchievement(result);
      }
    });
  }

  editAchievement(index: number) {
    const current = this.ds.state().achievements[index];
    const ref = this.dialog.open(AchievementModal, {
      header: this.translate.instant('wizard.achievement.edit'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'visible' },
      baseZIndex: 10000,
      closable: true,
      data: {
        initialValue: current,
      },
    });

    ref?.onClose.subscribe(result => {
      if (result) {
        const list = [...this.ds.state().achievements];
        list[index] = result;
        this.ds.up('achievements', list as any);
      }
    });
  }

  removeAchievement(index: number) {
    this.ds.delAchievement(index);
  }
}
