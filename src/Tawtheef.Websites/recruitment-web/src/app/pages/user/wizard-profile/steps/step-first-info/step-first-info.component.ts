import { Component, EventEmitter, Output, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { DataService } from '../../services/data.service';
import { CandidateType } from '../../../../../core/enums/lookups.enum';
import { ProfileService } from '../../services/profile.service';
import {mapPrereqSection} from '../../services/profile.mapper';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../core/utils/file-utils';

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
  fileUtils = inject(FileUtilsService);

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
        this.ds.up('cvFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('cvName', file.name);
        break;
      case 'id':
        this.idFile = file;
        this.ds.up('idFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('idName', file.name);
        break;
      case 'birth':
        this.birthCertificateFile = file;
        this.ds.up('birthCertificateFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('birthCertificateName', file.name);
        break;
      case 'marriage':
        this.marriageCertificateFile = file;
        this.ds.up('marriageCertificateFile', {resourceId: 'local', resourceName: file.name, file: file });
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

  canPreview(kind: 'cv' | 'id' | 'birth' | 'marriage'): boolean {
    return !!this.getLocalFile(kind) || !!this.getFileRef(kind)?.url;
  }

  previewFile(kind: 'cv' | 'id' | 'birth' | 'marriage', ev?: Event) {
    ev?.stopPropagation();
    const local = this.getLocalFile(kind);
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    const ref = this.getFileRef(kind);
    if (ref?.url) {
      this.fileUtils.previewUrl(ref.url, ref.resourceName || '', false);
    }
  }

  private getLocalFile(kind: 'cv' | 'id' | 'birth' | 'marriage'): File | null {
    switch (kind) {
      case 'cv':
        if(this.cvFile){
          return this.cvFile;
        }else{
          const state = this.ds.state();
          return state.cvFile?.file ?? null;
        }
      case 'id':
        if(this.idFile){
          return this.idFile;
        }else{
          const state = this.ds.state();
          return state.idFile?.file ?? null;
        }
      case 'birth':
        if(this.birthCertificateFile){
          return this.birthCertificateFile;
        }else{
          const state = this.ds.state();
          return state.birthCertificateFile?.file ?? null;
        }
      case 'marriage':
        if(this.marriageCertificateFile){
          return this.marriageCertificateFile;
        }else{
          const state = this.ds.state();
          return state.marriageCertificateFile?.file ?? null;
        }
    }
  }

  private getFileRef(kind: 'cv' | 'id' | 'birth' | 'marriage') {
    const state = this.ds.state();
    switch (kind) {
      case 'cv':
        return state.cvFile;
      case 'id':
        return state.idFile;
      case 'birth':
        return state.birthCertificateFile;
      case 'marriage':
        return state.marriageCertificateFile;
    }
  }
}
