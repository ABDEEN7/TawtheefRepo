import {ProfileState} from '../models/profile-state.model';
import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';

export function mapPrereqSection(state: ProfileState): SaveProfilePrereqRequestModel {
  return {
    submit: false,
    candidateTypeId: state.candidateType!.id,
    targetEntityId: state.targetEntity!.id,

    cvResourceId: state.cvFile?.resourceId ?? null,
    cvFileName: state.cvFile?.resourceName ?? state.cvName ?? null,

    idResourceId: state.idFile?.resourceId ?? null,
    idFileName: state.idFile?.resourceName ?? state.idName ?? null,

    birthCertResourceId: state.birthCertificateFile?.resourceId ?? null,
    birthCertFileName: state.birthCertificateFile?.resourceName ?? state.birthCertificateName ?? null,

    marriageCertResourceId: state.marriageCertificateFile?.resourceId ?? null,
    marriageCertFileName: state.marriageCertificateFile?.resourceName ?? state.marriageCertificateName ?? null,
  };
}
export function mapPersonalSection(state: ProfileState): SaveProfilePersonalRequestDto {
  return {
    submit: false,

    fullName: state.fullName ?? null,
    fullNameEn: state.fullNameEn ?? null,
    nationalNumber: state.qid ?? null,
    birthDate: state.dob ?? null,

    nationalityId: state.nationality?.id ?? null,
    genderId: state.gender?.id ?? null,
    religionId: state.religion?.id ?? null,
    maritalStatusId: state.marital?.id ?? null,
    childrenCount: state.children ?? null,

    hasDisability: state.hasDisability,
    disabilityDetails: state.disabilityDetails ?? null,

    sponsorEmployerName: state.sponsorEmployerName ?? null,
    sponsorEmployerNumber: state.sponsorEmployerNumber ?? null,
    sponsorTypeId: state.sponsorType?.id ?? null,
    sponsorCard: state.sponsorCardFile ?? null,
  };
}
export function mapContactSection(state: ProfileState): SaveProfileContactRequestDto {
  const hasNa =
    !!state.naZone ||
    !!state.naStreet ||
    !!state.naBuilding ||
    !!state.naUnit ||
    !!state.naFile;

  return {
    submit: false,

    residenceCountryId: state.country?.id ?? null,
    dialCode: state.dialCode ?? null,
    address: state.address ?? null,
    interviewLocation: state.interviewPlace ?? null,

    phone: state.phone ?? null,
    phoneVerified: state.phoneVerified,
    email: state.email ?? null,
    emailVerified: state.emailVerified,

    nationalAddress: hasNa
      ? {
        zone: state.naZone ?? null,
        street: state.naStreet ?? null,
        building: state.naBuilding ?? null,
        unit: state.naUnit ?? null,
        document: state.naFile ?? null,
      } : null,
  };
}
