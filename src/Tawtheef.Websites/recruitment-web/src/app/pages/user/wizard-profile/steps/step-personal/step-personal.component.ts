import {Component, EventEmitter, Output, inject, OnInit, OnDestroy, effect} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DataService} from '../../services/data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {PrerequisitesModal} from './dialogs/prerequisites.modal/prerequisites.modal';
import {TranslateService} from '@ngx-translate/core';
import {AvatarModal} from './dialogs/avatar.modal/avatar.modal';
import {Subject} from 'rxjs';
import {toObservable} from '@angular/core/rxjs-interop';
import {takeUntil} from 'rxjs/operators';
import {ProfileState} from '../../models/profile-state.model';
import {ProfileLookupsService} from '../../services/profile-lookups.service';

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

  genders = [
    { id: 1, name: 'ذكر' },
    { id: 2, name: 'أنثى' }
  ];

  nationalities = [
    { id: 1, name: 'قطري' }, { id: 2, name: 'سعودي' }, { id: 3, name: 'إماراتي' },
    { id: 4, name: 'بحريني' }, { id: 5, name: 'كويتي' }, { id: 6, name: 'أردني' },
    { id: 7, name: 'مصري' }, { id: 8, name: 'سوداني' }, { id: 9, name: 'تونسي' },
    { id: 10, name: 'مغربي' }, { id: 11, name: 'لبناني' }, { id: 12, name: 'سوري' },
    { id: 13, name: 'فلسطيني' }, { id: 14, name: 'عراقي' }, { id: 15, name: 'يمني' },
    { id: 16, name: 'هندي' }, { id: 17, name: 'باكستاني' }
  ];

  religions = [
    { id: 1, name: 'الإسلام' },
    { id: 2, name: 'المسيحية' },
    { id: 3, name: 'أخرى' }
  ];

  maritals = [
    { id: 1, name: 'أعزب' },
    { id: 2, name: 'متزوج' },
    { id: 3, name: 'مطلق' },
    { id: 4, name: 'أرمل' }
  ];

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
    })?.onClose.subscribe((e: { candidateType: string; targetEntity: string; cvFile: File; idFile: File }) => {
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
