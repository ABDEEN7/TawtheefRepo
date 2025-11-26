import { Component, EventEmitter, Output, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { DataService } from '../../services/data.service';
import { CandidateType } from '../../../../../core/enums/lookups.enum';
import { ProfileService } from '../../services/profile.service';
import {mapPrereqSection} from '../../services/profile.mapper';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';

@Component({
  selector: 'app-step-first-info',
  templateUrl: './step-first-info.component.html',
  styleUrl: './step-first-info.component.scss',
  standalone: false
})
export class StepFirstInfoComponent {
  @Output() next = new EventEmitter<void>();

  ds        = inject(DataService);
  translate = inject(TranslateService);
  lookups   = inject(ProfileLookupsService);
  profile   = inject(ProfileService);
  messageService   = inject(MessageService);

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['basic'];
  }
  uploading: Record<'cv' | 'id' | 'birth' | 'marriage', boolean> = {
    cv: false,
    id: false,
    birth: false,
    marriage: false,
  };

  saving = false;

  get isNeedBirthCertificate() {
    const t = this.ds.state().candidateType?.backendName as CandidateType | undefined;
    if (!t) return false;
    return [CandidateType.SonOfQatariMother].includes(t);
  }

  get isNeedMarriageCertificate() {
    const t = this.ds.state().candidateType?.backendName as CandidateType | undefined;
    if (!t) return false;
    return [CandidateType.WifeOfQatari].includes(t);
  }

  onFileSelected(kind: 'cv' | 'id' | 'birth' | 'marriage', event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    this.uploading[kind] = true;
    this.profile.uploadFile(file).subscribe({
      next: res => {
        switch (kind) {
          case 'cv':
            this.ds.up('cvFile', { resourceId: res.resourceId, resourceName: res.resourceName });
            this.ds.up('cvName', res.resourceName);
            break;
          case 'id':
            this.ds.up('idFile', { resourceId: res.resourceId, resourceName: res.resourceName });
            this.ds.up('idName', res.resourceName);
            break;
          case 'birth':
            this.ds.up('birthCertificateFile', { resourceId: res.resourceId, resourceName: res.resourceName });
            this.ds.up('birthCertificateName', res.resourceName);
            break;
          case 'marriage':
            this.ds.up('marriageCertificateFile', { resourceId: res.resourceId, resourceName: res.resourceName });
            this.ds.up('marriageCertificateName', res.resourceName);
            break;
        }
        input.value = '';
        this.uploading[kind] = false;
      },
      error: err => {
        console.error(err);
        this.uploading[kind] = false;
        input.value = '';
      }
    });
  }

  onNext() {
    if (!this.step.valid) {
      const firstError = this.step.errors[0];
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant(firstError.i18nKey),
        life: 5000,
      });
      return;
    }

    const state = this.ds.state();
    const payload = mapPrereqSection(state);
    this.saving = true;
    this.profile.savePrereq(payload).subscribe({
      next: () => {
        this.saving = false;
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
      }
    });
  }
}
