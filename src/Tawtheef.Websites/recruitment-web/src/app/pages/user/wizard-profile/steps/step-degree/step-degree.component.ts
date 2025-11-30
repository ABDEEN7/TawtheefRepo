import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { DialogService } from 'primeng/dynamicdialog';
import { DegreeModal } from './dialogs/degree.modal/degree.modal';
import { TranslateService } from '@ngx-translate/core';
import { createStepValiditySignal } from '../../state/profile-step-validity.signal';
import { ProfileService } from '../../services/profile.service';
import { MessageService } from 'primeng/api';
import {Degree} from '../../models/degree.model';

@Component({
  selector: 'app-step-degrees',
  templateUrl: './step-degree.component.html',
  styleUrl: './step-degree.component.scss',
  standalone: false,
})
export class StepDegreeComponent {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);
  messageService = inject(MessageService);

  savingDegrees = false;

  get step() {
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['degrees'];
  }

  add() {
    this.dialog.open(DegreeModal, {
        header: this.translate.instant('wizard.degrees.add'),
        width: '80%',
        contentStyle: { 'max-height': '80vh', 'overflow': 'scroll' },
        baseZIndex: 10000,
        closable: true,
      })?.onClose.subscribe((e: Degree) => {
      if (e) {
        this.ds.addDegree(e);
      }
    });
  }

  del(i: number) {
    this.ds.delDegree(i);
  }

  // ====== NEW: submit to API ======
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
    const degrees = state.degrees || [];
    if (!degrees.length) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant('wizard.degrees.validation.noRows'),
        life: 5000,
      });
      return;
    }

    this.savingDegrees = true;
    this.profile
      .saveEducationSection(degrees)
      .subscribe({
        next: () => {
          this.savingDegrees = false;
          this.next.emit();
        },
        error: err => {
          console.error(err);
          this.savingDegrees = false;
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.degrees.saveError'),
            life: 5000,
          });
        },
      });
  }
}
