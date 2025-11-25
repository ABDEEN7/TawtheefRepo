import { Component, EventEmitter, Output, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { DataService } from '../../services/data.service';
import { CandidateType } from '../../../../../core/enums/lookups.enum';
import { ProfileService } from '../../services/profile.service';
import {mapPrereqSection} from '../../services/profile.mapper';

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

  uploading: Record<'cv' | 'id' | 'birth' | 'marriage', boolean> = {
    cv: false,
    id: false,
    birth: false,
    marriage: false,
  };

  saving = false;
  errorMessage: string | null = null;

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
    this.errorMessage = null;

    this.profile.uploadFile(file).subscribe({
      next: res => {
        switch (kind) {
          case 'cv':
            this.ds.up('cvFile', { resourceId: res.resourceId, fileName: res.fileName });
            this.ds.up('cvName', res.fileName);
            break;
          case 'id':
            this.ds.up('idFile', { resourceId: res.resourceId, fileName: res.fileName });
            this.ds.up('idName', res.fileName);
            break;
          case 'birth':
            this.ds.up('birthCertificateFile', { resourceId: res.resourceId, fileName: res.fileName });
            this.ds.up('birthCertificateName', res.fileName);
            break;
          case 'marriage':
            this.ds.up('marriageCertificateFile', { resourceId: res.resourceId, fileName: res.fileName });
            this.ds.up('marriageCertificateName', res.fileName);
            break;
        }
        input.value = '';
        this.uploading[kind] = false;
      },
      error: err => {
        console.error(err);
        this.uploading[kind] = false;
        this.errorMessage = this.translate.instant('wizard.prereq.uploadError');
        input.value = '';
      }
    });
  }

  // استدعاء عند الضغط على Next
  onNext() {
    this.errorMessage = null;

    const state = this.ds.state();

    // تحقق بسيط قبل الإرسال
    if (!state.candidateType || !state.targetEntity) {
      this.errorMessage = this.translate.instant('wizard.prereq.missingTypeOrTarget');
      return;
    }

    if (!state.cvFile) {
      this.errorMessage = this.translate.instant('wizard.prereq.cvRequired');
      return;
    }

    if (!state.idFile) {
      this.errorMessage = this.translate.instant('wizard.prereq.idRequired');
      return;
    }

    if (this.isNeedBirthCertificate && !state.birthCertificateFile) {
      this.errorMessage = this.translate.instant('wizard.prereq.birthRequired');
      return;
    }

    if (this.isNeedMarriageCertificate && !state.marriageCertificateFile) {
      this.errorMessage = this.translate.instant('wizard.prereq.marriageRequired');
      return;
    }

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
        this.errorMessage = this.translate.instant('wizard.prereq.saveError');
      }
    });
  }
}
