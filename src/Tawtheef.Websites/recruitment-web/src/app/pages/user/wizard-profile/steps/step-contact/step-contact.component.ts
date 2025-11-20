import {Component, EventEmitter, Output, inject, OnInit} from '@angular/core';
import { DataService } from '../../services/data.service';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {CountryISO, NgxIntlTelInputComponent, SearchCountryField} from 'ngx-intl-tel-input';
import {GeoIpService} from '../../../../../core/services/geo-ip.service';
import { PhoneNumberUtil, PhoneNumber } from 'google-libphonenumber';

@Component({
  selector: 'app-step-contact',
  templateUrl: './step-contact.component.html',
  styleUrl: './step-contact.component.scss',
  standalone: false,
})
export class StepContactComponent implements OnInit{
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);
  lookups = inject(ProfileLookupsService);
  geoIp = inject(GeoIpService);
  protected readonly phoneNumberUtil = PhoneNumberUtil.getInstance();
  protected readonly SearchCountryField = SearchCountryField;
  phoneView: any = null;
  phoneValid = false;
  phoneTouched = false;
  selectedCountryIso2: CountryISO = CountryISO.Qatar;

  ngOnInit(): void {
    this.geoIp.getCountryIso2().subscribe(code => {
      this.selectedCountryIso2 = code.toLowerCase() as CountryISO;
    });
  }
  onPhoneChange(value: any) {
    if(!value) return;

    this.phoneTouched = true;
    var phoneNumber = this.phoneNumberUtil.parseAndKeepRawInput(value.e164Number);
    this.phoneValid = this.phoneNumberUtil.isValidNumber(phoneNumber);
    if (this.phoneValid) {
      this.ds.up('phone', value);
    } else {
      this.ds.up('phone', null);
    }
  }
}
