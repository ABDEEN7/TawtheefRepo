import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { DialogService } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { ProfileState } from '../../models/profile-state.model';
import {CandidateType, MaritalStatus, SponsorType} from '../../../../../core/enums/lookups.enum';
import {mapPersonalSection} from '../../services/profile.mapper';
import {finalize} from 'rxjs/operators';
import {ProfileService} from '../../services/profile.service';
import {dateToDateOnly} from '../../../../../shared/types/dateOnly.type';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';

@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  standalone: false,
})
export class StepPersonalComponent {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);
  profileService = inject(ProfileService);
  messageService = inject(MessageService);

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['personal'];
  }

  savingPersonal = false;
  private sponsorCardLocalFile: File | null = null;
  get isNeedSponsor(){
    return [CandidateType.ResidentQatar].includes(
      this.ds.state().candidateType?.backendName as CandidateType
    );
  }
  updateField<K extends keyof ProfileState>(key: K, value: ProfileState[K]) {
    if (this.ds.isLocked(key as any)) return;
    this.ds.up(key as any, value as any);
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

  get showChildrenField(): boolean {
    const marital = this.ds.state().marital as any;
    return !!marital && marital.backendName !== MaritalStatus.Single;
  }
  get showDisabilityType(): boolean {
    return this.ds.state().hasDisability;
  }

  onSponsorCardSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.sponsorCardLocalFile = file;
    this.ds.up('sponsorCardFile', { resourceId: 'local', fileName: file.name } as any);
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

    this.savingPersonal = true;

    this.profileService
      .savePersonalSection(dto, { sponsorCardFile: this.sponsorCardLocalFile })
      .pipe(finalize(() => this.savingPersonal = false))
      .subscribe({
        next: () => {
          this.next.emit();
        },
        error: (err) => {
          console.error(err);
        }
      });
  }

  protected readonly dateToDateOnly = dateToDateOnly;
  protected readonly SponsorType = SponsorType;
}
