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
    dialCode: '',
    phone: '',
    email: '',
    address: '',
    interviewPlace: '',
    naZone: '',
    naStreet: '',
    naBuilding: '',
    naUnit: ''
  };

  countries = ['قطر', 'السعودية', 'الإمارات', 'الكويت', 'الأردن', 'مصر'];
}
