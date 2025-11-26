import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {QualificationModal} from './dialogs/qualification.modal/qualification.modal';
import {TranslateService} from '@ngx-translate/core';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';

@Component({
  selector: 'app-step-degrees',
  templateUrl: './step-qualification.component.html',
  styleUrl: './step-qualification.component.scss',
  standalone: false,
})
export class StepQualificationComponent {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['degrees'];
  }

  add(){
    this.dialog.open(QualificationModal,{
      header: this.translate.instant('wizard.degrees.add'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'visible' },
      baseZIndex: 10000,
      closable: true
    })?.onClose.subscribe(e => {
      if(e){
        this.ds.addDegree(e)
      }
    })
  }
  del(i:number){ this.ds.delDegree(i); }
}
