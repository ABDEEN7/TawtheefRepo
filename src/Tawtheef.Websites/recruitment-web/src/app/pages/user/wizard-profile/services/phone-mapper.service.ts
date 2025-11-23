import {Injectable} from '@angular/core';
import {PhoneNumberUtil, PhoneNumberFormat} from 'google-libphonenumber';
import {PhoneNumber as AppPhoneNumber} from '../models/phone-number.model';

@Injectable({providedIn: 'root'})
export class PhoneMapperService {
  private phoneUtil = PhoneNumberUtil.getInstance();

  /**
   * raw: can be either +962777123456 or 0777123456
   * defaultRegion: used when number doesn't start with + (e.g. JO, SA, AE...)
   */
  toPhoneObject(raw?: string | null | undefined, defaultRegion: string = 'JO'): AppPhoneNumber | null {
    try {
      if (!raw) return null;
      // If number starts with +, let it auto-detect the country
      const parsed = raw.startsWith('+')
        ? this.phoneUtil.parse(raw)
        : this.phoneUtil.parse(raw, defaultRegion);

      if (!this.phoneUtil.isValidNumber(parsed)) {
        console.warn('Invalid phone number', raw);
        return null;
      }

      const e164Number = this.phoneUtil.format(parsed, PhoneNumberFormat.E164);           // +962777123456
      const internationalNumber = this.phoneUtil.format(parsed, PhoneNumberFormat.INTERNATIONAL); // +962 7771 23456 (formatted)
      const nationalNumber = this.phoneUtil.format(parsed, PhoneNumberFormat.NATIONAL);   // 07xx xxx xxx (or similar)
      const countryCode = this.phoneUtil.getRegionCodeForNumber(parsed) ?? defaultRegion; // JO
      const dialCode = `+${parsed.getCountryCode()}`;                                     // +962

      // national number without country code, usually without leading zero
      const number = parsed.getNationalNumber()!.toString(); // 777123456

      return {
        number,
        internationalNumber,
        nationalNumber,
        e164Number,
        countryCode,
        dialCode
      };
    } catch (err) {
      console.error('Error parsing phone number:', raw, err);
      return null;
    }
  }
}
