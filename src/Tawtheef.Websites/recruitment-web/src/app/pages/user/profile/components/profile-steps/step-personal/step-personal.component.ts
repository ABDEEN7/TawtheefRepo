import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {
  canPreviewFile,
  createFileSlot, displayedFileName,
  FileSlot, fileSlotSignature, fileToUpload, previewFileFromSlot,
  previewUrlFromSlot,
  setLocalFile,
  updateRemote
} from '../../../wizard-profile/utils/file-slot';
import {TranslateService} from '@ngx-translate/core';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {ProfileState} from '../../../wizard-profile/models/profile-state.model';
import {finalize} from 'rxjs/operators';
import {normalizeMoiResponse} from '../../../wizard-profile/services/moi-response-normalizer';
import {mapPersonalSection} from '../../../wizard-profile/services/profile.mapper';
import {MaritalStatus, SponsorType} from '../../../../../../core/enums/lookups.enum';
import {dateToDateOnly} from '../../../../../../shared/types/dateOnly.type';


@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  standalone: false
})
export class StepPersonalComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);
  profileService = inject(ProfileService);
  messageService = inject(MessageService);
  fileUtils = inject(FileUtilsService);
  protected readonly dateToDateOnly = dateToDateOnly;
  protected readonly SponsorType = SponsorType;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['personal'];
  }

  savingPersonal = false;
  verifyingSponsor = false;
  private sponsorCard: FileSlot = createFileSlot();
  private lastSubmittedSignature: string | null = null;
  readonly today = new Date();
  updateField<K extends keyof ProfileState>(key: K, value: ProfileState[K]) {
    if (this.ds.isLocked(key as any)) return;
    this.ds.up(key as any, value as any);
  }

  verifySponsorProfile() {
    const state = this.ds.state();

    if (state.sponsorType?.backendName !== SponsorType.Individual) return;

    if (!state.sponsorEmployerNumber || !state.sponsorQidExpiry) {
      this.messageService.add({
        severity: 'warn',
        summary: this.translate.instant('wizard.personal.verify.title'),
        detail: this.translate.instant('wizard.personal.verify.missing'),
        life: 4000,
      });
      return;
    }

    this.verifyingSponsor = true;
    this.profileService
      .checkProfile(state.sponsorEmployerNumber, state.sponsorQidExpiry)
      .pipe(finalize(() => this.verifyingSponsor = false))
      .subscribe({
        next: (res: any) => {
          this.ds.applySponsorPersonalInfo(normalizeMoiResponse(res));
          this.messageService.add({
            severity: 'success',
            summary: this.translate.instant('wizard.personal.verify.title'),
            detail: this.translate.instant('wizard.personal.verify.success'),
            life: 3000,
          });
        },
        error: (err: any) => console.log(err)
      });
  }

  updateChildren(value: any) {
    const num = value === null || value === '' ? 0 : Number(value);
    this.updateField('children', isNaN(num) ? 0 : num);
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
    this.lastSubmittedSignature = null;
  }

  get showChildrenField(): boolean {
    const marital = this.ds.state().marital as any;
    return !!marital && marital.backendName !== MaritalStatus.Single;
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
    if (!this.step.valid) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }

    const s = this.ds.state();
    const dto = mapPersonalSection(s);
    const signature = this.buildSignature(dto);

    if (signature && signature === this.lastSubmittedSignature) {
      this.next.emit();
      return;
    }

    this.savingPersonal = true;

    this.profileService
      .savePersonalSection(dto, { sponsorCard: fileToUpload(this.sponsorCard) })
      .pipe(finalize(() => this.savingPersonal = false))
      .subscribe({
        next: () => {
          this.lastSubmittedSignature = signature;
          this.next.emit();
        },
        error: (err: any) => {
          console.error(err);
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
