import {Component, EventEmitter, Output, inject, OnInit} from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { DataService } from '../../services/data.service';
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
export class StepPersonalComponent implements OnInit{
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);
  fb = inject(FormBuilder);
  dialog = inject(DialogService);
  translate = inject(TranslateService);

  avatarPreviewUrl: string | null = null;
  genders = [ { "id": 1, "name": "ذكر" }, { "id": 2, "name": "أنثى" } ];
  nationalities = [ { "id": 1, "name": "قطري" }, { "id": 2, "name": "سعودي" }, { "id": 3, "name": "إماراتي" }, { "id": 4, "name": "بحريني" }, { "id": 5, "name": "كويتي" }, { "id": 6, "name": "أردني" }, { "id": 7, "name": "مصري" }, { "id": 8, "name": "سوداني" }, { "id": 9, "name": "تونسي" }, { "id": 10, "name": "مغربي" }, { "id": 11, "name": "لبناني" }, { "id": 12, "name": "سوري" }, { "id": 13, "name": "فلسطيني" }, { "id": 14, "name": "عراقي" }, { "id": 15, "name": "يمني" }, { "id": 16, "name": "هندي" }, { "id": 17, "name": "باكستاني" } ];
  religions = [ { "id": 1, "name": "الإسلام" }, { "id": 2, "name": "المسيحية" }, { "id": 3, "name": "أخرى" } ];
  maritals = [ { "id": 1, "name": "أعزب" }, { "id": 2, "name": "متزوج" }, { "id": 3, "name": "مطلق" }, { "id": 4, "name": "أرمل" } ];

  form = this.fb.group({
    fullName: ['', Validators.required],
    fullNameEn: ['', Validators.required],
    qid: ['', Validators.required],
    dob: ['', Validators.required],
    nationality: ['', Validators.required],
    gender: ['', Validators.required],
    religion: ['', Validators.required],
    marital: ['', Validators.required],
    children: [0, [Validators.min(0)]]
  });

  ngOnInit(){
    this.form.patchValue(this.ds.state());
    if(!this.ds.state().candidateType){
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
      if(croppedImage) {
        this.ds.up('avatarUrl', croppedImage!);
        this.avatarPreviewUrl = croppedImage;
      }
    })
  }
  editPrereq(){
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
    })?.onClose.subscribe((e: { candidateType: string; targetEntity: string; cvFile: File; idFile: File })=>{
      if(e){
        this.ds.up('candidateType', e.candidateType);
        this.ds.up('targetEntity', e.targetEntity);
        this.ds.up('cvName', e.cvFile.name);
        this.ds.up('idName', e.idFile.name);
      }
    })
  }
  get prereqDone(): boolean {
    const s = this.ds.state();
    return !!(s.candidateType && s.targetEntity && s.cvName && s.idName);
  }
  save(){
    Object.entries(this.form.value).forEach(([k,v]) => this.ds.up(k as any, v as any));
    this.next.emit();
  }
}
