import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-personal-info',
  standalone: false,
  templateUrl: './personal-info.html',
  styleUrl: './personal-info.scss',
})
export class PersonalInfo {
  form = {
    fullNameAr: '',
    fullNameEn: '',
    qid: '',
    dob: '',
    nationality: '',
    gender: ''
  };

  countries = ['قطر', 'السعودية', 'الإمارات', 'الكويت', 'الأردن', 'مصر'];
}
