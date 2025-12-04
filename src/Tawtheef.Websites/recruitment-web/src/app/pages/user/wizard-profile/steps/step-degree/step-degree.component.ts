import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { DialogService } from 'primeng/dynamicdialog';
import { DegreeModal } from './dialogs/degree.modal/degree.modal';
import { TranslateService } from '@ngx-translate/core';
import { createStepValiditySignal } from '../../state/profile-step-validity.signal';
import { ProfileService } from '../../services/profile.service';
import { MessageService } from 'primeng/api';
import {Degree} from '../../models/degree.model';
import {FileUtilsService} from '../../../../../core/utils/file-utils';

@Component({
  selector: 'app-step-degrees',
  templateUrl: './step-degree.component.html',
  styleUrl: './step-degree.component.scss',
  standalone: false,
})
export class StepDegreeComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);
  messageService = inject(MessageService);
  fileUtils = inject(FileUtilsService);

  savingDegrees = false;
  private lastSubmittedSignature: string | null = null;

  get step() {
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['degrees'];
  }

  ngOnInit(): void {
    const state = this.ds.state();
    const signature = this.buildSignature(state.degrees);
    this.lastSubmittedSignature = null;
  }

  add() {
    this.dialog.open(DegreeModal, {
        header: this.translate.instant('wizard.degrees.add'),
        width: '80%',
        contentStyle: { 'max-height': '80vh', 'overflow': 'auto' },
        baseZIndex: 10000,
        closable: true,
      })?.onClose.subscribe((e: Degree) => {
      if (e) {
        this.ds.addDegree(e);
      }
    });
  }

  del(i: number) {
    // reterive degree and check if it already submit to server should be remove from server before remove iot from client
    const degree = this.ds.state().degrees[i];
    if(degree.id){
      this.profile.deleteEduction(degree.id).subscribe({
        next: () => {
          this.ds.delDegree(i);
        },
        error: err => {
          console.error(err);
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.degrees.deleteError'),
            life: 5000,
          });
        },
      });
    } else {
      this.ds.delDegree(i);
    }
  }

  preview(r: Degree, ev?: Event) {
    ev?.stopPropagation();
    const local = r.file ?? r.certificate?.file ?? null;
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    if (r.certificate?.url) {
      this.fileUtils.previewUrl(r.certificate.url, r.certificate.resourceName || '', false);
    }else if(r.certificate?.file){
      this.fileUtils.previewBlob(r.certificate!.file);
    }
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
    const signature = this.buildSignature(degrees);

    if (signature && signature === this.lastSubmittedSignature) {
      this.next.emit();
      return;
    }

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
          this.lastSubmittedSignature = signature;
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

  private buildSignature(degrees: Degree[]): string {
    return JSON.stringify(
      (degrees ?? []).map(d => ({
        id: d.id ?? null,
        degreeId: d.degree?.id ?? null,
        gradCountryId: d.gradCountry?.id ?? null,
        universityId: d.university?.id ?? null,
        majorId: d.major?.id ?? null,
        subMajorId: d.subMajor?.id ?? null,
        gradYear: d.gradYear ?? null,
        studySystemId: d.studySystem?.id ?? null,
        gpa: d.gpa ?? null,
        gradeId: d.grade?.id ?? null,
        attachmentId: d.attachmentId ?? null,
        fileName: d.file?.name ?? d.certificate?.resourceName ?? null,
      }))
    );
  }
}
