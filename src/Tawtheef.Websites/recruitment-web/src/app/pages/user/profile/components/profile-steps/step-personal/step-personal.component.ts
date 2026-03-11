import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  computed,
  ChangeDetectionStrategy,
  input,
  output,
  signal
} from '@angular/core';
import { CommonModule, NgClass, LowerCasePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { DialogService } from 'primeng/dynamicdialog';
import {
  canPreviewFile,
  createFileSlot,
  displayedFileName,
  FileSlot,
  fileSlotSignature,
  fileToUpload,
  previewFileFromSlot,
  previewUrlFromSlot,
  setLocalFile,
  updateRemote
} from '../../../wizard-profile/utils/file-slot';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { ProfileState } from '../../../wizard-profile/models/profile-state.model';
import { finalize } from 'rxjs/operators';
import { normalizeMoiResponse } from '../../../wizard-profile/services/moi-response-normalizer';
import { mapPersonalSection } from '../../../wizard-profile/services/profile.mapper';
import { SponsorType } from '../../../../../../core/enums/lookups.enum';
import { dateToDateOnly } from '../../../../../../shared/types/dateOnly.type';
import { NotificationService } from '../../../../../../core/services/notification.service';


@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    InputTextModule,
    SelectModule,
    DatePickerModule,
    ButtonModule,
    FaDirArrowDirective,
    LowerCasePipe
  ]
})
export class StepPersonalComponent implements OnInit {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);
  profileService = inject(ProfileService);
  notificationService = inject(NotificationService);
  fileUtils = inject(FileUtilsService);
  protected readonly dateToDateOnly = dateToDateOnly;
  protected readonly SponsorType = SponsorType;

  step = computed(() => this.ds.stepValidationDetailed().personal);

  savingPersonal = signal(false);
  verifyingSponsor = signal(false);
  private sponsorCard: FileSlot = createFileSlot();
  private lastSubmittedSignature: string | null = null;
  readonly today = new Date();
  defaultBirthDate = (() => {
    const d = new Date();
    d.setFullYear(d.getFullYear() - 25);
    return d;
  })();
  updateField<K extends keyof ProfileState>(key: K, value: ProfileState[K]) {
    if (this.ds.isLocked(key as any)) return;
    this.ds.up(key as any, value as any);
  }
  get sponsorEmployerNumberMaxLen(): number {
    const t = this.ds.state().sponsorType?.backendName;
    if (t === SponsorType.Individual) return 11;
    if (t === SponsorType.Company) return 8;
    return 11; // safe default
  } readonly MIN_AGE = 18;
  readonly MAX_AGE = 100;

  readonly minBirthDate = (() => {
    const d = new Date();
    d.setFullYear(d.getFullYear() - this.MAX_AGE);
    d.setHours(0, 0, 0, 0);
    return d;
  })();

  readonly maxBirthDate = (() => {
    const d = new Date();
    d.setFullYear(d.getFullYear() - this.MIN_AGE);
    d.setHours(23, 59, 59, 999);
    return d;
  })();

  // optional UI error flags
  dobInvalid = signal(false);
  dobErrorKey = signal('wizard.personal.dobInvalid'); // add translations

  onDobSelect(value: unknown) {
    if (this.ds.isLocked('dob')) return;

    const date = this.toDate(value);
    if (!date) {
      this.setDobInvalid('wizard.personal.dobInvalid');
      this.updateField('dob', null as any);
      return;
    }

    // normalize time
    date.setHours(12, 0, 0, 0);

    // validate logical range
    if (date < this.minBirthDate) {
      this.setDobInvalid('wizard.personal.dobTooOld'); // e.g. older than 100
      this.updateField('dob', null as any);
      return;
    }

    if (date > this.maxBirthDate) {
      this.setDobInvalid('wizard.personal.dobTooYoung'); // e.g. younger than 18
      this.updateField('dob', null as any);
      return;
    }

    // good => store as DateOnly string
    this.dobInvalid.set(false);
    this.dobErrorKey.set('');
    this.updateField('dob', dateToDateOnly(date)! as any);
  }

  private setDobInvalid(key: string) {
    this.dobInvalid.set(true);
    this.dobErrorKey.set(key);
  }

  /**
   * Accept Date or 'YYYY-MM-DD' (or anything) and return a valid Date or null.
   */
  private toDate(v: unknown): Date | null {
    if (!v) return null;

    if (v instanceof Date && !isNaN(v.getTime())) return v;

    if (typeof v === 'string') {
      // expect YYYY-MM-DD (because you store DateOnly string)
      const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(v.trim());
      if (!m) return null;

      const y = Number(m[1]);
      const mo = Number(m[2]);
      const d = Number(m[3]);

      const dt = new Date(y, mo - 1, d);
      // strict check (avoid 2026-02-31 rolling)
      if (dt.getFullYear() !== y || dt.getMonth() !== mo - 1 || dt.getDate() !== d) return null;

      return dt;
    }

    return null;
  }

  get sponsorEmployerNumberHintKey(): string {
    const t = this.ds.state().sponsorType?.backendName;
    return t === SponsorType.Company
      ? 'wizard.personal.sponsor.company.numberHint8'
      : 'wizard.personal.sponsor.individual.qidHint11';
  }

  get sponsorEmployerNumberInvalid(): boolean {
    const s = this.ds.state();
    const t = s.sponsorType?.backendName;
    const v = (s.sponsorEmployerNumber ?? '').trim();

    if (!t) return false;            // no sponsor type selected yet
    if (!v) return false;            // required validation handled in step validity
    if (!/^\d+$/.test(v)) return true;

    const need = t === SponsorType.Company ? 8 : 11;
    return v.length !== need;
  }

  get sponsorEmployerNumberErrorKey(): string {
    const t = this.ds.state().sponsorType?.backendName;
    return t === SponsorType.Company
      ? 'wizard.personal.sponsor.company.numberInvalid8'
      : 'wizard.personal.sponsor.individual.qidInvalid11';
  }

  onSponsorEmployerNumberChange(raw: string) {
    // keep digits only + enforce max length while typing
    const digitsOnly = (raw ?? '').replace(/\D/g, '');
    const maxLen = this.sponsorEmployerNumberMaxLen;
    const trimmed = digitsOnly.slice(0, maxLen);

    // Bypass isLocked check to allow editing even if previously verified
    this.ds.up('sponsorEmployerNumber', trimmed as any);

    // If the number changes, any previously verified name is no longer valid
    if (this.ds.state().sponsorType?.backendName === SponsorType.Individual) {
      this.ds.up('sponsorEmployerName', null);
    }
  }
  onQidChange(raw: string | null | undefined) {
    if (this.ds.isLocked('qid')) return;

    const normalized = (raw ?? '').replace(/\D/g, '').slice(0, 20);
    this.updateField('qid', normalized as any);
  }

  onQidKeyDown(event: KeyboardEvent, maxLen: number) {
    if (this.ds.isLocked('qid')) {
      event.preventDefault();
      return;
    }

    const allowedKeys = new Set([
      'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Home', 'End'
    ]);

    if (event.ctrlKey || event.metaKey || allowedKeys.has(event.key)) {
      return;
    }

    if (!/^\d$/.test(event.key)) {
      event.preventDefault();
      return;
    }

    const input = event.target as HTMLInputElement;
    const selectionLength = (input.selectionEnd ?? 0) - (input.selectionStart ?? 0);
    const nextLength = input.value.length - selectionLength + 1;

    if (nextLength > maxLen) {
      event.preventDefault();
    }
  }

  onQidInput(raw: string | null | undefined, maxLen: number) {
    if (this.ds.isLocked('qid')) return;

    const sanitized = (raw ?? '').replace(/\D/g, '').slice(0, maxLen);
    this.updateField('qid', sanitized as any);
  }

  onQidPaste(event: ClipboardEvent, currentValue: string | null | undefined, maxLen: number) {
    if (this.ds.isLocked('qid')) {
      event.preventDefault();
      return;
    }

    const pasteText = event.clipboardData?.getData('text') ?? '';
    if (!/^\d+$/.test(pasteText)) {
      event.preventDefault();
      return;
    }

    const input = event.target as HTMLInputElement;
    const current = currentValue ?? '';
    const start = input.selectionStart ?? current.length;
    const end = input.selectionEnd ?? current.length;
    const nextValue = current.slice(0, start) + pasteText + current.slice(end);

    if (nextValue.length > maxLen) {
      event.preventDefault();
      return;
    }

    queueMicrotask(() => this.onQidInput(this.ds.state().qid, maxLen));
  }

  onSponsorTypeChange(value: any) {
    this.updateField('sponsorType', value);

    // trim sponsorEmployerNumber to new max and remove non-digits
    const current = this.ds.state().sponsorEmployerNumber ?? '';
    const digits = current.replace(/\D/g, '').slice(0, this.sponsorEmployerNumberMaxLen);
    this.ds.up('sponsorEmployerNumber', digits as any);
    this.ds.up('sponsorEmployerName', null);
  }
  verifySponsorProfile() {
    const state = this.ds.state();

    if (state.sponsorType?.backendName !== SponsorType.Individual) return;
    if (state.sponsorEmployerNumber == state.qid) {
      this.notificationService.error(this.translate.instant('wizard.personal.verify.selfSponsor'), this.translate.instant('wizard.personal.verify.title'));
      return;
    }

    if (!state.sponsorEmployerNumber || !state.sponsorQidExpiry) {
      this.notificationService.error(this.translate.instant('wizard.personal.verify.missing'), this.translate.instant('wizard.personal.verify.title'));
      return;
    }

    this.verifyingSponsor.set(true);
    this.profileService
      .checkProfile(state.sponsorEmployerNumber, state.sponsorQidExpiry)
      .pipe(finalize(() => this.verifyingSponsor.set(false)))
      .subscribe({
        next: (res: any) => {
          this.ds.applySponsorPersonalInfo(normalizeMoiResponse(res));
          this.notificationService.success(this.translate.instant('wizard.personal.verify.success'), this.translate.instant('wizard.personal.verify.title'));
        },
        error: () => {
          // Clear sponsor name if verification fails
          if (this.ds.state().sponsorType?.backendName === SponsorType.Individual) {
            this.ds.up('sponsorEmployerName', null);
          }
        }
      });
  }

  updateDisability(value: boolean) {
    this.updateField('hasDisability', value as any);
    if (!value) {
      this.updateField('disabilityDetails', null as any);
    }
  }

  ngOnInit(): void {
    const state = this.ds.state();
    const dto = mapPersonalSection(state);
    updateRemote(this.sponsorCard, state.sponsorCard);
    this.lastSubmittedSignature = this.buildSignature(dto);
  }

  get showDisabilityType(): boolean {
    return this.ds.state().hasDisability;
  }

  previewSponsorCard(ev?: Event) {
    ev?.stopPropagation();
    if (!canPreviewFile(this.sponsorCard)) return;

    const local = previewFileFromSlot(this.sponsorCard);
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    const url = previewUrlFromSlot(this.sponsorCard);
    if (url) {
      this.fileUtils.previewUrl(url, displayedFileName(this.sponsorCard), false);
    }
  }

  onSponsorCardSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    setLocalFile(this.sponsorCard, file);
    this.ds.up('sponsorCardName', file.name);
    this.ds.up('sponsorCard', { resourceId: 'local', fileName: file.name, file: file } as any);
    input.value = '';
  }
  onNext() {
    if (!this.step().valid) {
      this.notificationService.error(this.step().errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'), this.translate.instant('wizard.validationErrorTitle'));
      return;
    }

    const s = this.ds.state();
    const dto = mapPersonalSection(s);
    const signature = this.buildSignature(dto);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges() || this.ds.hasUnsolvedCorrections(2)) {
        const msg = this.ds.hasUnsolvedCorrections(2)
          ? 'يجب عمل التعديلات المذكورة في ملاحظات المراجع'
          : this.translate.instant('profileView.notifications.noChanges');
        this.notificationService.error(msg);
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    this.savingPersonal.set(true);

    this.profileService
      .savePersonalSection(dto, { sponsorCard: fileToUpload(this.sponsorCard) })
      .pipe(finalize(() => this.savingPersonal.set(false)))
      .subscribe({
        next: () => {
          this.lastSubmittedSignature = signature;
          this.ds.markStepSubmitted('personal');
          if (this.profileService.isChangeRequestMode()) {
            this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
          }
          this.next.emit();
        }
      });
  }

  private buildSignature(dto: ReturnType<typeof mapPersonalSection>): string | null {
    try {
      return JSON.stringify({ dto, sponsorCard: fileSlotSignature(this.sponsorCard) });
    } catch {
      return null;
    }
  }


}
