import {GUID} from '../../../../../shared/types/guid.type';

export interface NationalAddressDto {
  zone: string | null;
  street: string | null;
  building: string | null;
  unit: string | null;
  nationalAddressFileName: string | null;
}

export interface SaveProfileContactRequestDto {
  submit: boolean;

  residenceCountryId: GUID | null;
  interviewLocationId: GUID | null;
  officeId?: GUID | null;

  address: string | null;
  nationalAddress: NationalAddressDto | null;
}
