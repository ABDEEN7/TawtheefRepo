import {PhoneNumber} from './phone-number.model';
import {Skill} from './skill.model';
export interface SaveUserProfileRequestDto {
submit: boolean;

candidateTypeId: string;
targetEntityId: string;

cvFileName?: string | null;
idFileName?: string | null;
birthCertificateFileName?: string | null;
marriageCertificateFileName?: string | null;

fullName?: string | null;
fullNameEn?: string | null;
nationalNumber?: string | null;
birthDate?: string | null; // ISO: 'YYYY-MM-DD'

nationalityId?: string | null;
genderId?: string | null;
religionId?: string | null;
maritalStatusId?: string | null;
childrenCount?: number | null;

residenceCountryId?: string | null;
address?: string | null;
interviewLocationId?: string | null;

phone?: PhoneNumber | null;
phoneVerified: boolean;
email?: string | null;
emailVerified: boolean;

nationalAddress?: {
  zone?: string | null;
  street?: string | null;
  building?: string | null;
  unit?: string | null;
  fileName?: string | null;
} | null;

hasDisability: boolean;
disabilityDetails?: string | null;
sponsorEmployerName?: string | null;
sponsorEmployerNumber?: string | null;
sponsorCardName?: string | null;
sponsorTypeId?: string | null;

degrees: QualificationDto[];
experiences: ExperienceDto[];
trainingCourses: TrainingCourseDto[];
  skills: Skill[];
languages: LanguageDto[];
additionalAttachments: AttachmentDto[];
}

export interface QualificationDto {
  id?: string | null;
  level: string;
  major?: string | null;
  university?: string | null;
  year?: number | null;
  gradCountry?: string | null;
  fileName?: string | null;
}

export interface ExperienceDto {
  id?: string | null;
  org: string;
  title: string;
  name?: string | null;
  from?: string | null; // 'YYYY-MM-DD'
  to?: string | null;
  tasks?: string | null;
  description?: string | null;
  fileName?: string | null;
}

export interface TrainingCourseDto extends ExperienceDto {}

export interface LanguageDto {
  languageId: string;
  languageLevelId: string;
}

export interface AttachmentDto {
  id?: string | null;
  name: string;
  type?: string | null;
  size?: number | null;
  url?: string | null;
}


