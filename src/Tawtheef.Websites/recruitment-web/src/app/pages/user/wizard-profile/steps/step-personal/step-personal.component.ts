import { Component, EventEmitter, Output, inject } from '@angular/core';
import { DataService } from '../../services/data.service';
import { DialogService } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { ProfileState } from '../../models/profile-state.model';
import {MaritalStatus} from '../../../../../core/enums/lookups.enum';

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
    return !!this.ds.state().hasDisability;
  }
}
