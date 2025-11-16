import {Component, EventEmitter, Output, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DataService} from '../../services/data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {PrerequisitesModal} from './dialogs/prerequisites.modal/prerequisites.modal';
import {TranslateService} from '@ngx-translate/core';
import {AvatarModal} from './dialogs/avatar.modal/avatar.modal';

@Component({
  selector: 'app-step-personal',
  templateUrl: './step-personal.component.html',
  styleUrl: './step-personal.component.scss',
  standalone: false,
})
export class StepPersonalComponent implements OnInit {
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
  fb = inject(FormBuilder);
  dialog = inject(DialogService);
  translate = inject(TranslateService);

  form!: FormGroup; // <-- create in ngOnInit instead of inline

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

  ngOnInit() {
    const s = this.ds.state();

    // show avatar if it came from bootstrap or previous step
    this.avatarPreviewUrl = s.avatarUrl ?? null;

    // ⚠️ build the form using current state + lock info
    this.form = this.fb.group({
      fullName: [{
        value: s.fullName ?? '',
        disabled: this.ds.isLocked('fullName')
      }, Validators.required],

      fullNameEn: [{
        value: s.fullNameEn ?? '',
        disabled: this.ds.isLocked('fullNameEn')
      }, Validators.required],

      qid: [{
        value: s.qid ?? '',
        disabled: this.ds.isLocked('qid')
      }, Validators.required],

      dob: [{
        value: s.dob ?? '',
        disabled: this.ds.isLocked('dob')
      }, Validators.required],

      nationality: [{
        value: s.nationality ?? '',
        disabled: this.ds.isLocked('nationality')
      }, Validators.required],

      gender: [{
        value: s.gender ?? '',
        disabled: this.ds.isLocked('gender')
      }, Validators.required],

      religion: [{
        value: s.religion ?? '',
        disabled: this.ds.isLocked('religion')
      }, Validators.required],

      marital: [{
        value: s.marital ?? '',
        disabled: this.ds.isLocked('marital')
      }, Validators.required],

      children: [{
        value: s.children ?? 0,
        disabled: this.ds.isLocked('children')
      }, [Validators.min(0)]]
    });

    // 🔄 live sync to DataService, but skip locked fields
    this.form.valueChanges.subscribe(val => {
      Object.entries(val).forEach(([key, value]) => {
        const k = key as keyof typeof s;
        if (!this.ds.isLocked(k as any)) {
          this.ds.up(k as any, value as any);
        }
      });
    });

    // keep your prerequisite logic as-is
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
      closable: true,
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
    Object.entries(this.form.getRawValue()).forEach(([k, v]) => {
      const key = k as keyof typeof this.ds.state;
      if (!this.ds.isLocked(key as any)) {
        this.ds.up(key as any, v as any);
      }
    });

    this.next.emit();
  }
}
