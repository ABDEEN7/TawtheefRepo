import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-contact-info',
  standalone: false,
  templateUrl: './contact-info.html',
  styleUrl: './contact-info.scss',
})
export class ContactInfo {
  form = {
    country: '',
    phone: '',
    email: '',
    address: ''
  };

  countries = ['قطر', 'السعودية', 'الإمارات', 'الكويت', 'الأردن', 'مصر'];
}
