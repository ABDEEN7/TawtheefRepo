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

  private cvFile: File | null = null;
  private idFile: File | null = null;
  private birthCertificateFile: File | null = null;
  private marriageCertificateFile: File | null = null;
  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['basic'];
  }

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
    switch (kind) {
      case 'cv':
        this.cvFile = file;
        this.ds.up('cvFile', { resourceId: 'local', resourceName: file.name });
        this.ds.up('cvName', file.name);
        break;
      case 'id':
        this.idFile = file;
        this.ds.up('idFile', { resourceId: 'local', resourceName: file.name });
        this.ds.up('idName', file.name);
        break;
      case 'birth':
        this.birthCertificateFile = file;
        this.ds.up('birthCertificateFile', { resourceId: 'local', resourceName: file.name });
        this.ds.up('birthCertificateName', file.name);
        break;
      case 'marriage':
        this.marriageCertificateFile = file;
        this.ds.up('marriageCertificateFile', {resourceId: 'local', resourceName: file.name });
        this.ds.up('marriageCertificateName', file.name);
        break;
    }
    input.value = '';
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
    const payload = mapPrereqSection(state);
    this.saving = true;
    this.profile
      .savePrereq(payload, {
        cvFile: this.cvFile,
        idFile: this.idFile,
        birthCertificateFile: this.birthCertificateFile,
        marriageCertificateFile: this.marriageCertificateFile
      }).subscribe({
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
