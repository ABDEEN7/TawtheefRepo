import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { TranslateService} from '@ngx-translate/core';
import {DialogService} from 'primeng/dynamicdialog';
import {ExperienceModal} from './dialogs/experience.modal/experience.modal';
import {CourseModal} from './dialogs/course.modal/course.modal';
import {AchievementModal} from './dialogs/achievement.modal/achievement.modal';

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
  addExperience(){
    this.dialog.open(ExperienceModal,{
      header: this.translate.instant('wizard.experience.add'),
      width: '50%',
      contentStyle: {'max-height': '80vh', 'overflow': 'visible'},
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe(e => {
      if(e){
        this.ds.addExp(e);
      }
    })
  }

  addCourse(){
    this.dialog.open(CourseModal,{
      header: this.translate.instant('wizard.courses.add'),
      width: '50%',
      contentStyle: {'max-height': '80vh', 'overflow': 'visible'},
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe(e => {
      if(e){
        this.ds.addCourse(e)
      }
    })
  }
  addAchievement(){
    this.dialog.open(AchievementModal,{
      header: this.translate.instant('wizard.achievement.add'),
      width: '50%',
      contentStyle: {'max-height': '80vh', 'overflow': 'visible'},
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe(e => {
      if(e){
        this.ds.addAchievement(e)
      }
    })
  }
}
