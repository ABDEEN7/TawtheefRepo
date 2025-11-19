import {Component, EventEmitter, Output, inject, OnInit, OnDestroy, effect} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DataService} from '../../services/data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {PrerequisitesModal} from './dialogs/prerequisites.modal/prerequisites.modal';
import {TranslateService} from '@ngx-translate/core';
import {AvatarModal} from './dialogs/avatar.modal/avatar.modal';
import {Subject} from 'rxjs';
import {ProfileState} from '../../models/profile-state.model';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  standalone: false,
})
export class StepPersonalComponent implements OnInit, OnDestroy {
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  fb = inject(FormBuilder);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  lookups = inject(ProfileLookupsService);

  avatarPreviewUrl: string | null = null;
  private destroy$ = new Subject<void>();
  form!: FormGroup;
  ngOnInit() {
    const s = this.ds.state();

    this.avatarPreviewUrl = s.avatarUrl ?? null;

    this.form = this.fb.group({
      fullName: [s.fullName ?? null, Validators.required],
      fullNameEn: [s.fullNameEn ?? null, Validators.required],
      qid: [{
        value: s.qid ?? null,
        disabled: this.ds.isLocked('qid')
      }, Validators.required],
      dob: [{
        value: s.dob ?? null,
        disabled: this.ds.isLocked('dob')
      }, Validators.required],
      nationality: [{
        value: s.nationality ?? null,
        disabled: this.ds.isLocked('nationality')
      }, Validators.required],
      gender: [{
        value: s.gender ?? null,
        disabled: this.ds.isLocked('gender')
      }, Validators.required],
      religion: [s.religion ?? null, Validators.required],
      marital: [s.marital ?? null, Validators.required],
      children: [s.children ?? 0, [Validators.min(0)]]
    });

    if (!this.ds.state().candidateType) {
      this.editPrereq();
    }
  }


  openAvatarDialog() {
    this.dialog.open(AvatarModal, {
      header: this.translate.instant('wizard.personal.avatar.title'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'visible' },
      baseZIndex: 10000,
      closable: true
    })?.onClose.subscribe((croppedImage: string | null) => {
      if (croppedImage) {
        this.ds.up('avatarUrl', croppedImage);
        this.avatarPreviewUrl = croppedImage;
      }
    });
  }

  editPrereq() {
    this.dialog.open(PrerequisitesModal, {
      header: this.translate.instant('wizard.personal.prerequisites'),
      width: '80%',
      contentStyle: { 'max-height': '80vh', 'overflow': 'visible' },
      baseZIndex: 10000,
      data: {
        initialValue: {
          candidateType: this.ds.state().candidateType,
          targetEntity: this.ds.state().targetEntity
        }
      },
    })?.onClose.subscribe((e: { candidateType: dropdownOptionsModel; targetEntity: dropdownOptionsModel; cvFile: File; idFile: File }) => {
      if (e) {
        this.ds.up('candidateType', e.candidateType);
        this.ds.up('targetEntity', e.targetEntity);
        this.ds.up('cvName', e.cvFile.name);
        this.ds.up('idName', e.idFile.name);
      }
    });
  }

  get prereqDone(): boolean {
    const s = this.ds.state();
    return !!(s.candidateType && s.targetEntity && s.cvName && s.idName);
  }

  save() {
    const raw = this.form.getRawValue();
    Object.entries(raw).forEach(([k, v]) => {
      const key = k as keyof ProfileState;
      if (!this.ds.isLocked(key as any)) {
        this.ds.up(key as any, v as any);
      }
    });
    this.next.emit();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
