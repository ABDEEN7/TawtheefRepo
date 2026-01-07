import {Component, EventEmitter, inject, OnInit, Output} from '@angular/core';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {TranslateService} from '@ngx-translate/core';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {
  createFileSlot,
  FileSlot,
  fileSlotSignature,
  fileToUpload,
  previewFileFromSlot,
  previewUrlFromSlot,
  setLocalFile,
  updateRemote
} from '../../../wizard-profile/utils/file-slot';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {dateToDateOnly} from '../../../../../../shared/types/dateOnly.type';
import {mapPrereqSection} from '../../../wizard-profile/services/profile.mapper';
import {catchError, finalize, map, switchMap, tap} from 'rxjs/operators';
import {normalizeMoiResponse} from '../../../wizard-profile/services/moi-response-normalizer';
import {of} from 'rxjs';
import {NotificationService} from '../../../../../../core/services/notification.service';


@Component({
  selector: 'app-step-prereq',
  templateUrl: './step-prereq.component.html',
  styleUrl: './step-prereq.component.scss',
  standalone: false
})
export class StepPrereqComponent implements OnInit {
  @Output() next = new EventEmitter<void>();

  ds        = inject(ProfileDataService);
  translate = inject(TranslateService);
  lookups   = inject(ProfileLookupsService);
  profile   = inject(ProfileService);
  notificationService   = inject(NotificationService);
  fileUtils = inject(FileUtilsService);

  private cvFile: FileSlot = createFileSlot();
  private idFile: FileSlot = createFileSlot();
  private birthCertificateFile: FileSlot = createFileSlot();
  private marriageCertificateFile: FileSlot = createFileSlot();
  private lastSubmittedSignature: string | null = null;
  private hasCheckedProfile = false;
  today = new Date();
  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['basic'];
  }

  saving = false;

  ngOnInit(): void {
    const state = this.ds.state();
    updateRemote(this.cvFile, state.cvFile);
    updateRemote(this.idFile, state.idFile);
    updateRemote(this.birthCertificateFile, state.birthCertificateFile);
    updateRemote(this.marriageCertificateFile, state.marriageCertificateFile);

    this.lastSubmittedSignature = null;
  }

  onCandidateTypeChange(option: any) {
    if (this.ds.isCandidateTypeLocked) return;
    this.ds.up('candidateType', option);
    this.hasCheckedProfile = false;
  }

  onQidExpirySelect(date: Date) {
    this.ds.up('qidExpiry', dateToDateOnly(date));
    this.hasCheckedProfile = false;
  }

  onFileSelected(kind: 'cv' | 'id' | 'birth' | 'marriage', event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    switch (kind) {
      case 'cv':
        setLocalFile(this.cvFile, file);
        this.ds.up('cvFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('cvName', file.name);
        break;
      case 'id':
        setLocalFile(this.idFile, file);
        this.ds.up('idFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('idName', file.name);
        break;
      case 'birth':
        setLocalFile(this.birthCertificateFile, file);
        this.ds.up('birthCertificateFile', { resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('birthCertificateName', file.name);
        break;
      case 'marriage':
        setLocalFile(this.marriageCertificateFile, file);
        this.ds.up('marriageCertificateFile', {resourceId: 'local', resourceName: file.name, file: file });
        this.ds.up('marriageCertificateName', file.name);
        break;
    }
    input.value = '';
  }

  onNext() {
    if (!this.step.valid) {
      this.notificationService.error(this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'), this.translate.instant('wizard.validationErrorTitle'));
      return;
    }

    const state = this.ds.state();
    const payload = mapPrereqSection(state);
    const signature = this.buildSignature(payload);
    const shouldCheckProfile = this.shouldCheckProfile();

    // Check is needed only if feature is enabled AND not already done
    const needsCheckNow = shouldCheckProfile && !this.hasCheckedProfile;

    // If nothing changed and we don't need to re-check → just go next
    if (signature && signature === this.lastSubmittedSignature && !needsCheckNow) {
      this.next.emit();
      return;
    }

    const { qid, qidExpiry } = state;

    // If we must check profile and data for the check is missing → block and show error
    if (needsCheckNow && (!qid || !qidExpiry)) {
      this.notificationService.error(this.translate.instant('wizard.personal.verify.missingData'), this.translate.instant('wizard.personal.verify.title'));
      return;
    }

    this.saving = true;

    const check$ = needsCheckNow
      ? this.profile.checkProfile(qid!, qidExpiry!).pipe(
        tap(res => {
          this.ds.applyMoiPersonalInfo(normalizeMoiResponse(res));
          this.hasCheckedProfile = true;
          this.notificationService.success(this.translate.instant('wizard.personal.verify.success'), this.translate.instant('wizard.personal.verify.title'));
        }),
        map(() => true as const),
        catchError(err => {
          console.error(err);
          // Block submission if check failed
          return of(false as const);
        })
      )
      : of(true as const);

    check$
      .pipe(
        switchMap(canProceed => {
          if (!canProceed) {
            // Check failed → do not save or go next
            return of(false as const);
          }
          return this.profile
            .savePrereq(payload, {
              cvFile: fileToUpload(this.cvFile),
              idFile: fileToUpload(this.idFile),
              birthCertificateFile: fileToUpload(this.birthCertificateFile),
              marriageCertificateFile: fileToUpload(this.marriageCertificateFile),
            })
            .pipe(map(() => true as const));
        }),
        finalize(() => (this.saving = false))
      )
      .subscribe({
        next: (canProceed: any) => {
          this.lastSubmittedSignature = signature;
          if (canProceed) {
            this.next.emit();
          }
        }
      });
  }

  canPreview(kind: 'cv' | 'id' | 'birth' | 'marriage'): boolean {
    return !!this.getSlot(kind) || !!this.getSlot(kind)?.remote?.url;
  }

  previewFile(kind: 'cv' | 'id' | 'birth' | 'marriage', ev?: Event) {
    ev?.stopPropagation();
    const local = previewFileFromSlot(this.getSlot(kind));
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    const ref = previewUrlFromSlot(this.getSlot(kind));
    if (ref) {
      this.fileUtils.previewUrl(ref, this.getSlot(kind).remote?.resourceName || '', false);
    }
  }

  private buildSignature(payload: ReturnType<typeof mapPrereqSection>): string | null {
    try {
      const files = {
        cv: fileSlotSignature(this.cvFile),
        id: fileSlotSignature(this.idFile),
        birth: fileSlotSignature(this.birthCertificateFile),
        marriage: fileSlotSignature(this.marriageCertificateFile),
      };
      return JSON.stringify({ payload, files });
    } catch {
      return null;
    }
  }

  private shouldCheckProfile(): boolean {
    const { qid, qidExpiry } = this.ds.state();
    return this.ds.isResidentQatar && !!qid && !!qidExpiry;
  }

  private getSlot(kind: 'cv' | 'id' | 'birth' | 'marriage'): FileSlot {
    switch (kind) {
      case 'cv':
        return this.cvFile;
      case 'id':
        return this.idFile;
      case 'birth':
        return this.birthCertificateFile;
      case 'marriage':
        return this.marriageCertificateFile;
    }
  }

  protected readonly dateToDateOnly = dateToDateOnly;
}
