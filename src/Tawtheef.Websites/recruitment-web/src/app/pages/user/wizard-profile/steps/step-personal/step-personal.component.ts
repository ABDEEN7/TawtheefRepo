import { Component, EventEmitter, Output, inject, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';
import { DialogService } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { AvatarModal } from './dialogs/avatar.modal/avatar.modal';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { ProfileState } from '../../models/profile-state.model';

@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  standalone: false,
})
export class StepPersonalComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);

  avatarPreviewUrl: string | null = null;

  ngOnInit() {
    const s = this.ds.state();
    this.avatarPreviewUrl = s.avatarUrl ?? null;
  }

  updateField<K extends keyof ProfileState>(key: K, value: ProfileState[K]) {
    if (this.ds.isLocked(key as any)) return;
    this.ds.up(key as any, value as any);
  }

  updateChildren(value: any) {
    const num = value === null || value === '' ? 0 : Number(value);
    this.updateField('children', isNaN(num) ? 0 : num);
  }

  get showChildrenField(): boolean {
    const marital = this.ds.state().marital as any;
    return !!marital && marital.backendName !== 'Single';
  }

  openAvatarDialog() {
    this.dialog.open(AvatarModal, {
      header: this.translate.instant('wizard.personal.avatar.title'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'visible' },
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
      }
    });
  }
}
