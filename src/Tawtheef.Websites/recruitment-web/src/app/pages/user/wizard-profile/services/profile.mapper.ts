import {SaveProfilePrereqRequestModel} from '../models/save-profile-prereq-request.model';
import {SaveProfilePersonalRequestDto} from '../models/save-profile-personal-request.model';
import {SaveProfileContactRequestDto} from '../models/save-user-contact-request.model';
import {ProfileState, UploadedFileRef} from '../models/profile-state.model';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {FileRefDto, PrefillData, ProfileStatusDto} from '../../../../core/models/auth/auth-response.model';
import {ProfileLookupsService} from './profile-lookups.service';
import {PhoneMapperService} from './phone-mapper.service';
import {Degree} from '../models/degree.model';
import {Experience, TrainingCourse} from '../models/experience.model';
import {Skill} from '../models/skill.model';
import {Language} from '../models/language.model';
import {Attachment} from '../models/attachment.model';

export function mapPrereqSection(state: ProfileState): SaveProfilePrereqRequestModel {
  return {
    submit: false,
    candidateTypeId: state.candidateType!.id,
    targetEntityId: state.targetEntity!.id,
    officeId: state.office?.id ?? null,
    cvFileName: state.cvName,
    idFileName: state.idName,
    birthCertificateFileName: state.birthCertificateName,
    marriageCertificateFileName: state.marriageCertificateName,
  };
}
export function mapPersonalSection(state: ProfileState): SaveProfilePersonalRequestDto {
  return {
    submit: false,

    fullNameAr: state.fullNameAr ?? null,
    fullNameEn: state.fullNameEn ?? null,
    nationalNumber: state.qid ?? null,
    qidExpiry: state.qidExpiry ?? null,
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
    sponsorQidExpiry: state.sponsorQidExpiry ?? null,
    sponsorTypeId: state.sponsorType?.id ?? null,
    sponsorCardFileName: state.sponsorCardName ?? null,
  };
}
export function mapContactSection(state: ProfileState): SaveProfileContactRequestDto {
  const hasNa =
    !!state.naZone ||
    !!state.naStreet ||
    !!state.naBuilding ||
    !!state.naUnit ||
    !!state.naFile ||
    !!state.naFileName;

  return {
    submit: false,

    residenceCountryId: state.country?.id ?? null,
    interviewLocationId: state.interviewPlace?.id ?? null,

    address: state.address ?? null,
    nationalAddress: hasNa
      ? {
        zone: state.naZone ?? null,
        street: state.naStreet ?? null,
        building: state.naBuilding ?? null,
        unit: state.naUnit ?? null,
        nationalAddressFileName: state.naFileName ?? null,
      } : null,
  };
}

export function mapProfileStatusToState(
  phoneMapper: PhoneMapperService,
  lookups: ProfileLookupsService,
  dto: ProfileStatusDto,
  prefill?: PrefillData | null
): ProfileState {
  return {
    // ----------- Prereq -----------
    candidateType: mapIdToDropdown(lookups, 'candidateType', dto.candidateTypeId) as dropdownOptionsModel,
    targetEntity: mapIdToDropdown(lookups, 'targetEntity', dto.targetEntityId) as dropdownOptionsModel,
    office: mapIdToDropdown(lookups, 'office', dto.officeId) as dropdownOptionsModel,

    // Attachments
    cvFile: mapFile(dto.resumeAttachment),
    cvName: dto.resumeAttachment?.fileName ?? null,

    idFile: mapFile(dto.nationalCard),
    idName: dto.nationalCard?.fileName ?? null,

    birthCertificateFile: mapFile(dto.birthdayCertificate),
    birthCertificateName: dto.birthdayCertificate?.fileName ?? null,

    marriageCertificateFile: mapFile(dto.marriageCertificate),
    marriageCertificateName: dto.marriageCertificate?.fileName ?? null,

    // ----------- Personal -----------
    fullNameAr: dto.fullNameAr ?? prefill?.fullName ?? undefined,
    fullNameEn: dto.fullNameEn ?? undefined,

    qid: dto.nationalNumber ?? prefill?.qid ?? undefined,
    qidExpiry: dto.qidExpiry ?? undefined,

    nationality: mapIdToDropdown(lookups, 'nationality', dto.nationalityId ?? prefill?.nationality ?? undefined),
    gender: mapIdToDropdown(lookups, 'gender', dto.genderId ?? prefill?.gender ?? undefined),
    religion: mapIdToDropdown(lookups, 'religion', dto.religionId ?? undefined),
    marital: mapIdToDropdown(lookups, 'marital', dto.maritalStatusId ?? undefined),
    sponsorType: mapIdToDropdown(lookups, 'sponsorType', dto.sponsorTypeId ?? undefined),

    children: dto.childrenCount,

    dob: dto.birthDate ?? prefill?.dob ?? undefined,

    hasDisability: dto.hasDisability,
    disabilityDetails: dto.disabilityDetails ?? null,

    sponsorEmployerName: dto.sponsorEmployerName,
    sponsorEmployerNumber: dto.sponsorEmployerNumber,
    sponsorQidExpiry: dto.sponsorQidExpiry,
    sponsorCardName: dto.sponsorCard?.fileName ?? null,
    sponsorCardFile: mapFile(dto.sponsorCard),

    // ----------- Contact -----------
    country: mapIdToDropdown(lookups, 'countries', dto.residenceCountryId),
    address: dto.address ?? undefined,

    phone: dto.phone ? phoneMapper.toPhoneObject(dto.phone) :
      prefill?.phone ? phoneMapper.toPhoneObject(prefill.phone) : null,

    phoneVerified: dto.phoneVerified ?? prefill?.phoneVerified ?? false,

    email: dto.email ?? prefill?.email ?? undefined,
    emailVerified: dto.emailVerified ?? prefill?.emailVerified ?? false,

    interviewPlace: mapIdToDropdown(lookups, 'interviewLocation', dto.interviewLocationId),

    // National Address
    naZone: dto.naZone,
    naStreet: dto.naStreet,
    naBuilding: dto.naBuilding,
    naUnit: dto.naUnit,

    naFileName: dto.residenceAddressCertificate?.fileName ?? null,
    naFile: mapFile(dto.residenceAddressCertificate),

    // ----------- Collections -----------
    degrees: (dto.qualifications ?? []).map(q => ({
      id: q.id,
      degree:  mapIdToDropdown(lookups, 'degree', q.degreeId),
      gradCountry: mapIdToDropdown(lookups, 'graduationCountry', q.gradCountryId),
      university: q.university,
      major: q.major,
      subMajor: q.subMajor,
      gradYear: q.graduationYear!,
      studySystem: mapIdToDropdown(lookups, 'studyType', q.studyTypeId),
      gpa: q.gpa!,
      grade: mapIdToDropdown(lookups, 'ratingGrade', q.gradeId),
      certificate: mapFile(q.attachment),
      attachmentId: q.attachment?.resourceId,
      certificateName: q.attachment?.fileName,
    } as Degree)),

    experiences: (dto.experiences ?? []).map(e => ({
      id: e.id,
      employerName: e.employerName ?? '',
      jobTitle: e.jobTitle ?? '',
      from: e.startDate ?? undefined,
      to: e.endDate ?? undefined,
      country: mapIdToDropdown(lookups, 'countries', e.countryId),
      current: e.isCurrent,
      description: e.description ?? '',
      fileName: e.attachment?.fileName,
      attachmentId: e.attachment?.resourceId,
      attachment: mapFile(e.attachment),
    } as Experience)),

    courses: (dto.trainingCourses ?? []).map(t => ({
      id: t.id,
      title: t.title ?? '',
      provider: t.provider ?? '',
      from: t.startDate ?? undefined,
      to: t.endDate ?? undefined,
      country: mapIdToDropdown(lookups, 'countries', t.countryId),
      description: t.description ?? '',
      fileName: t.attachment?.fileName,
      attachmentId: t.attachment?.resourceId,
      attachment: mapFile(t.attachment),
    } as TrainingCourse)),

    skills: (dto.skills ?? []).map(s => ({
      id: s.id,
      skillId: s.skillId,
      skill: s.skill,
      levelId: s.levelId,
      level: mapIdToDropdown(lookups, 'ratingGrade', s.levelId),
    } as Skill)),

    languages: (dto.languages ?? []).map(l => ({
      id: l.id,
      langId: l.languageId,
      lang: mapIdToDropdown(lookups, 'language', l.languageId),
      levelId: l.levelId,
      level: mapIdToDropdown(lookups, 'languageLevel', l.levelId),
    } as Language)),

    attachments: (dto.additionalAttachments ?? []).map(a => ({
      id: a.id,
      name: a.title ?? '',
      fileName: a.file?.fileName,
      attachmentId: a.file?.resourceId,
      fileRef: mapFile(a.file),
      // file: this.mapFile(a.file)!,
    } as Attachment)),

    // ----------- UI fields -----------
    available: true,
    avatarUrl: dto.avatar ?? prefill?.avatar ?? undefined,
  } as ProfileState;
}

function mapFile(ref?: FileRefDto | null): UploadedFileRef | null {
  if (!ref) return null;

  return {
    resourceId: ref.resourceId,
    resourceName: ref.fileName,
    url: ref.url ?? null,
  };
}

// Convert backend ID → dropdownOptionsModel
function mapIdToDropdown(lookups: ProfileLookupsService, kind: 'candidateType' | 'targetEntity' | 'countries' | 'language' | 'languageLevel' |
'nationality' | 'gender' | 'religion' | 'marital' | 'studyType' | 'degree' | 'ratingGrade' |
'interviewLocation' | 'residenceCountry' | 'graduationCountry' | 'sponsorType' | 'country' | 'office',
  id?: string | null): dropdownOptionsModel | undefined {
  if (!id) return undefined;
  switch (kind) {
    case 'candidateType':
      return lookups.candidateTypes().find(ct => ct.id === id);
    case 'targetEntity':
      return lookups.targetEntities().find(te => te.id === id);
    case 'nationality':
      return lookups.nationalities().find(nat => nat.id === id);
    case 'gender':
      return lookups.genders().find(g => g.id === id);
    case 'religion':
      return lookups.religions().find(r => r.id === id);
    case 'marital':
      return lookups.maritalStatuses().find(m => m.id === id);
    case 'studyType':
      return lookups.studyTypes().find(st => st.id === id);
    case 'degree':
      return lookups.degrees().find(d => d.id === id);
    case 'ratingGrade':
      return lookups.ratingGrades().find(rg => rg.id === id);
    case 'language':
      return lookups.languages().find(l => l.id === id);
    case 'languageLevel':
      return lookups.languageLevels().find(ll => ll.id === id);
    case 'interviewLocation':
      return lookups.interviewLocation().find(il => il.id === id);
    case 'residenceCountry':
      return lookups.residenceCountry().find(rc => rc.id === id);
    case 'graduationCountry':
      return lookups.graduationCountry().find(gc => gc.id === id);
    case 'sponsorType':
      return lookups.sponsorTypes().find(st => st.id === id);
    case 'office':
      return lookups.offices().find(o => o.id === id);
    case 'countries':
      return lookups.countries().find(c => c.id === id);
    default:
      throw Error(`Unknown dropdown: ${kind}`);
  }
}
