import {PhoneNumber} from './phone-number.model';
import {UploadedFileRef} from './profile-state.model';
export interface NationalAddressDto {
  zone: string | null;
  street: string | null;
  building: string | null;
  unit: string | null;
  document: UploadedFileRef | null;
}

export interface SaveProfileContactRequestDto {
  submit: boolean;

  residenceCountryId: string | null;
  dialCode: string | null;
  address: string | null;
  interviewLocation: string | null;

  phone: PhoneNumber | null;
  phoneVerified: boolean;
  email: string | null;
  emailVerified: boolean;

  nationalAddress: NationalAddressDto | null;
}
