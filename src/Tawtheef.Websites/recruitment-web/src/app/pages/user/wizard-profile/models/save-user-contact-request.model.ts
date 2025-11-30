
export interface NationalAddressDto {
  zone: string | null;
  street: string | null;
  building: string | null;
  unit: string | null;
}

export interface SaveProfileContactRequestDto {
  submit: boolean;

  residenceCountryId: string | null;
  interviewLocationId: string | null;

  address: string | null;
  nationalAddress: NationalAddressDto | null;
}
