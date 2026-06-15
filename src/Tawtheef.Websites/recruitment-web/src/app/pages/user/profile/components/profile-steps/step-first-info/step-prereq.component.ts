import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  computed,
  input,
  output,
  ChangeDetectionStrategy,
  signal
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
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
import { dateToDateOnly } from '../../../../../../shared/types/dateOnly.type';
import { mapPrereqSection } from '../../../wizard-profile/services/profile.mapper';
import { catchError, finalize, map, switchMap, tap } from 'rxjs/operators';
import { normalizeMoiResponse } from '../../../wizard-profile/services/moi-response-normalizer';
import { of } from 'rxjs';
import { NotificationService } from '../../../../../../core/services/notification.service';


@Component({
  selector: 'app-step-prereq',
  templateUrl: './step-prereq.component.html',
  styleUrl: './step-prereq.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    SelectModule,
    DatePickerModule,
    ButtonModule,
    InputTextModule
  ]
})
export class StepPrereqComponent implements OnInit {
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  requireChanges = input<boolean>(false);
  ds = inject(ProfileDataService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);
  profile = inject(ProfileService);
  notificationService = inject(NotificationService);
  fileUtils = inject(FileUtilsService);

  private cvFile: FileSlot = createFileSlot();
  private idFile: FileSlot = createFileSlot();
  private birthCertificateFile: FileSlot = createFileSlot();
  private marriageCertificateFile: FileSlot = createFileSlot();
  private lastSubmittedSignature: string | null = null;
  private hasCheckedProfile = signal(false);
  today = new Date();

  step = computed(() => this.ds.stepValidationDetailed().basic);

  saving = signal(false);
  private static readonly MAX_FILE_SIZE = 1 * 1024 * 1024; // 1MB

  private readonly FILE_RULES: Record<
    'cv' | 'id' | 'birth' | 'marriage',
    { exts: string[]; mimes: string[]; labelKey: string }
  > = {
      cv: {
        exts: ['.pdf', '.jpg', '.jpeg', '.png'],
        mimes: ['application/pdf', 'image/jpeg', 'image/png'],
        labelKey: 'wizard.files.formats.cv'
      },
      id: {
        exts: ['.jpg', '.jpeg', '.png', '.pdf'],
        mimes: ['image/jpeg', 'image/png', 'application/pdf'],
        labelKey: 'wizard.files.formats.id'
      },
      birth: {
        exts: ['.jpg', '.jpeg', '.png', '.pdf'],
        mimes: ['image/jpeg', 'image/png', 'application/pdf'],
        labelKey: 'wizard.files.formats.birth'
      },
      marriage: {
        exts: ['.jpg', '.jpeg', '.png', '.pdf'],
        mimes: ['image/jpeg', 'image/png', 'application/pdf'],
        labelKey: 'wizard.files.formats.marriage'
      }
    };
  ngOnInit(): void {
    const state = this.ds.state();
    updateRemote(this.cvFile, state.cvFile);
    updateRemote(this.idFile, state.idFile);
    updateRemote(this.birthCertificateFile, state.birthCertificateFile);
    updateRemote(this.marriageCertificateFile, state.marriageCertificateFile);

    const payload = mapPrereqSection(state);
    this.lastSubmittedSignature = this.buildSignature(payload);
  }

  onCandidateTypeChange(option: any) {
    if (this.ds.isCandidateTypeLocked || this.profile.isChangeRequestMode()) return;
    this.ds.up('candidateType', option);
    this.hasCheckedProfile.set(false);
  }

  onQidExpirySelect(date: Date) {
    if (this.profile.isChangeRequestMode()) return;
    this.ds.up('qidExpiry', dateToDateOnly(date));
    this.hasCheckedProfile.set(false);
  }

  private getFileExt(name: string): string {
    const i = name.lastIndexOf('.');
    return i >= 0 ? name.slice(i).toLowerCase() : '';
  }

  acceptFor(kind: 'cv' | 'id' | 'birth' | 'marriage'): string {
    // For the file picker UI
    return this.FILE_RULES[kind].exts.join(',');
  }

  formatsLabel(kind: 'cv' | 'id' | 'birth' | 'marriage'): string {
    // For showing user-friendly hint text
    return this.translate.instant(this.FILE_RULES[kind].labelKey);
  }

  private validateFile(kind: 'cv' | 'id' | 'birth' | 'marriage', file: File): boolean {
    // size
    if (file.size > StepPrereqComponent.MAX_FILE_SIZE) {
      this.notificationService.error(
        this.translate.instant('wizard.files.maxSize1mb'),
        this.translate.instant('wizard.validationErrorTitle')
      );
      return false;
    }

    // type
    const rule = this.FILE_RULES[kind];
    const ext = this.getFileExt(file.name);

    const extOk = rule.exts.includes(ext);

    // Some browsers may give empty file.type, so we allow ext check as primary
    const mimeOk = !file.type || rule.mimes.includes(file.type);

    if (!extOk || !mimeOk) {
      this.notificationService.error(
        this.translate.instant('wizard.files.invalidFormat', {
          formats: rule.exts.join(', ')
        }),
        this.translate.instant('wizard.validationErrorTitle')
      );
      return false;
    }

    return true;
  }
  onFileSelected(kind: 'cv' | 'id' | 'birth' | 'marriage', event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.validateFile(kind, file)) {
      input.value = '';
      return;
    }

    switch (kind) {
      case 'cv':
        setLocalFile(this.cvFile, file);
        this.ds.up('cvFile', { resourceId: 'local', resourceName: file.name, file });
        this.ds.up('cvName', file.name);
        break;

      case 'id':
        setLocalFile(this.idFile, file);
        this.ds.up('idFile', { resourceId: 'local', resourceName: file.name, file });
        this.ds.up('idName', file.name);
        break;

      case 'birth':
        setLocalFile(this.birthCertificateFile, file);
        this.ds.up('birthCertificateFile', { resourceId: 'local', resourceName: file.name, file });
        this.ds.up('birthCertificateName', file.name);
        break;

      case 'marriage':
        setLocalFile(this.marriageCertificateFile, file);
        this.ds.up('marriageCertificateFile', { resourceId: 'local', resourceName: file.name, file });
        this.ds.up('marriageCertificateName', file.name);
        break;
    }

    input.value = '';
  }


  onNext() {
    if (this.saving()) return;

    if (!this.step().valid) {
      this.notificationService.error(this.step().errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'), this.translate.instant('wizard.validationErrorTitle'));
      return;
    }

    const state = this.ds.state();
    const payload = mapPrereqSection(state);
    const signature = this.buildSignature(payload);
    const shouldCheckProfile = this.shouldCheckProfile();

    // Check is needed only if feature is enabled AND not already done
    const needsCheckNow = shouldCheckProfile && !this.hasCheckedProfile();

    // If nothing changed and we don't need to re-check → just go next
    if (signature && signature === this.lastSubmittedSignature && !needsCheckNow) {
      if (this.requireChanges() || this.ds.hasUnsolvedCorrections(1)) {
        const msg = this.ds.hasUnsolvedCorrections(1)
          ? 'يجب عمل التعديلات المذكورة في ملاحظات المراجع'
          : this.translate.instant('profileView.notifications.noChanges');
        this.notificationService.error(msg);
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      if (!this.profile.isChangeRequestMode()) {
        this.next.emit();
      }
      return;
    }

    const { qid, qidExpiry } = state;

    // If we must check profile and data for the check is missing → block and show error
    if (needsCheckNow && (!qid || !qidExpiry)) {
      this.notificationService.error(this.translate.instant('wizard.personal.verify.missingData'), this.translate.instant('wizard.personal.verify.title'));
      return;
    }

    this.saving.set(true);

    const check$ = needsCheckNow
      ? this.profile.checkProfile(qid!, qidExpiry!).pipe(
        tap(res => {
          this.ds.applyMoiPersonalInfo(normalizeMoiResponse(res));
          this.hasCheckedProfile.set(true);
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
        finalize(() => (this.saving.set(false)))
      )
      .subscribe({
        next: (canProceed: any) => {
          this.lastSubmittedSignature = signature;
          if (canProceed) {
            this.ds.markStepSubmitted('basic');
            if (this.profile.isChangeRequestMode()) {
              this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
            }
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
