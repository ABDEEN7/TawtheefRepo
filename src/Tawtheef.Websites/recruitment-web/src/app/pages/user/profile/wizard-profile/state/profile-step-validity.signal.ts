import { computed, Signal } from '@angular/core';
import { VALIDATION_KEYS } from '../constants/profile-validation-keys';
import {
  FieldError, StepValidationResult,
  StepValidityResult,
} from '../models/profile-validation.model';
import {ProfileState} from '../models/profile-state.model';
import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {CandidateType, SponsorType} from '../../../../../core/enums/lookups.enum';
import { StringUtils } from '../../../../../core/utils/string-utils';

function parseDate(value?: string | null): Date | null {
  if (!value) return null;

  const d = new Date(value);
  return isNaN(d.getTime()) ? null : d;
}

function startOfToday(): Date {
  const now = new Date();
  now.setHours(0, 0, 0, 0);
  return now;
}

function isCertificateType(type?: DropdownOptionVM | null): boolean {
  const backendName = type?.backendName?.toLowerCase() ?? '';
  return backendName.includes('certificate');
}

function normalizeTitle(value: unknown): string {
  return StringUtils.normalize((value ?? '').toString());
}

function keyPart(value: unknown): string {
  return normalizeTitle(value);
}

function optionKey(option: DropdownOptionVM | null | undefined): string {
  return keyPart(option?.id ?? option?.backendName ?? option?.name);
}

function compositeKey(parts: unknown[]): string {
  return parts.map(keyPart).join('|');
}

function hasDuplicateKeys<T>(items: T[] | undefined, selector: (item: T) => string): boolean {
  const seenKeys = new Set<string>();
  for (const item of items ?? []) {
    const key = selector(item);
    if (!key || key.split('|').every(part => !part)) {
      continue;
    }

    if (seenKeys.has(key)) {
      return true;
    }

    seenKeys.add(key);
  }

  return false;
}

function degreeDuplicateKey(degree: ProfileState['degrees'][number]): string {
  return compositeKey([
    optionKey(degree.degree),
    optionKey(degree.gradCountry),
    optionKey(degree.university),
    optionKey(degree.major),
    optionKey(degree.subMajor),
    degree.gradYear,
    optionKey(degree.studySystem),
    degree.gpa,
    optionKey(degree.grade),
  ]);
}

function experienceDuplicateKey(experience: ProfileState['experiences'][number]): string {
  return compositeKey([
    experience.employerName,
    experience.jobTitle,
    optionKey(experience.country),
    experience.from,
    experience.current ? '' : experience.to,
    experience.qualificationId,
  ]);
}

function courseDuplicateKey(course: ProfileState['courses'][number]): string {
  return compositeKey([
    course.provider,
    course.title,
    optionKey(course.country),
    course.from,
    course.to,
  ]);
}

function achievementDuplicateKey(achievement: ProfileState['achievements'][number]): string {
  return compositeKey([
    optionKey(achievement.achievementType),
    achievement.title,
    achievement.issuingAuthority,
    achievement.countryId ?? optionKey(achievement.country),
    achievement.issueDate,
  ]);
}

export function isFilledField(value: unknown): boolean {
  if (value === null || value === undefined) return false;

  if (typeof value === 'string') return value.trim().length > 0;
  if (typeof value === 'number') return Number.isFinite(value);
  if (typeof value === 'boolean') return value;

  if (typeof value === 'object') {
    const v: any = value;
    if ('id' in v && v.id !== null && v.id !== undefined) return true;
    if ('value' in v && v.value !== null && v.value !== undefined) return true;
  }

  return !!value;
}

export function candidateTypeFromState(state: ProfileState): CandidateType | undefined {
  return state.candidateType?.backendName as CandidateType | undefined;
}

export function candidateTypeNeedsSponsor(type: CandidateType | undefined): boolean {
  return !!type && [CandidateType.ResidentQatar, CandidateType.WifeOfQatari].includes(type);
}

export function candidateTypeNeedsBirthCertificate(type: CandidateType | undefined): boolean {
  return !!type && [CandidateType.SonOfQatariMother].includes(type);
}

export function candidateTypeNeedsMarriageCertificate(type: CandidateType | undefined): boolean {
  return !!type && [CandidateType.WifeOfQatari].includes(type);
}

export function candidateTypeIsResident(
  type: CandidateType | undefined,
  provider: 'Google' | 'QatarPass' | 'QatarResidentOtp'
): boolean {
  if (!type) return false;

  const normalizedProvider = (provider ?? '').toString().toLowerCase();
  const providerAllowsGcc = ['qatarpass', 'qatarresidentotp'].includes(normalizedProvider);

  return [
    CandidateType.ResidentQatar,
    CandidateType.QidHolder,
    CandidateType.Qatari,
    CandidateType.SonOfQatariMother,
    CandidateType.WifeOfQatari
  ].includes(type) || (type == CandidateType.GCC && providerAllowsGcc);
}

/** Small helper to push a "required" error using VALIDATION_KEYS */
function addRequiredError(
  errors: FieldError[],
  group: keyof typeof VALIDATION_KEYS,
  field: string
) {
  const keyGroup = VALIDATION_KEYS[group] as any;

  if (!keyGroup || !keyGroup[field]) {
    // fail-safe: don't crash if key is missing
    console.warn(`Missing validation key mapping for ${group}.${field}`);
    return;
  }

  errors.push({
    field,
    i18nKey: keyGroup[field],
  });
}

/* ========== BASIC STEP ========== */
function validateBasicStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  const backendType = candidateTypeFromState(s);
  const needsMarriageCertificate = candidateTypeNeedsMarriageCertificate(backendType);
  const needsBirthCertificate = candidateTypeNeedsBirthCertificate(backendType);
  const isResident = candidateTypeIsResident(backendType, s.provider);

  if (!isFilledField(s.candidateType)) {
    addRequiredError(errors, 'basic', 'candidateType');
  }

  if (!isFilledField(s.targetEntity)) {
    addRequiredError(errors, 'basic', 'targetEntity');
  }

  if (isResident && !isFilledField(s.qidExpiry)) {
    addRequiredError(errors, 'basic', 'qidExpiry');
  }

  if (!isFilledField(s.cvName)) {
    addRequiredError(errors, 'basic', 'cvName');
  }

  if (isFilledField(s.candidateType)) {
    if(s.candidateType?.backendName != CandidateType.GCC) {
      if (!isFilledField(s.idName)) {
        addRequiredError(errors, 'basic', 'idName');
      }
    }else {
      if (!isFilledField(s.idName)) {
        addRequiredError(errors, 'basic', 'passport');
      }
    }
  }

  if (needsMarriageCertificate && !isFilledField(s.marriageCertificateName)) {
    addRequiredError(errors, 'basic', 'marriageCertificateName');
  }

  if (needsBirthCertificate && !isFilledField(s.birthCertificateName)) {
    addRequiredError(errors, 'basic', 'birthCertificateName');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== PERSONAL STEP ========== */
function validatePersonalStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  const needsSponsor = candidateTypeNeedsSponsor(candidateTypeFromState(s));
  const isIndividualSponsor = s.sponsorType?.backendName === SponsorType.Individual;

  if (!isFilledField(s.fullNameAr)) {
    addRequiredError(errors, 'personal', 'fullNameAr');
  }

  if (!isFilledField(s.fullNameEn)) {
    addRequiredError(errors, 'personal', 'fullNameEn');
  }

  if (!isFilledField(s.qid)) {
    addRequiredError(errors, 'personal', 'qid');
  } else {
    const qid = (s.qid ?? '').trim();
    if (!/^\d+$/.test(qid)) {
      errors.push({ field: 'qid', i18nKey: 'wizard.personal.qid.digitsOnly' });
    }
    if (qid.length > 20) {
      errors.push({ field: 'qid', i18nKey: 'wizard.personal.qid.maxLength20' });
    }
  }

  if (!isFilledField(s.dob)) {
    addRequiredError(errors, 'personal', 'dob');
  }

  if (!isFilledField(s.nationality)) {
    addRequiredError(errors, 'personal', 'nationality');
  }

  if (!isFilledField(s.gender)) {
    addRequiredError(errors, 'personal', 'gender');
  }

  if (!isFilledField(s.religion)) {
    addRequiredError(errors, 'personal', 'religion');
  }

  if (!isFilledField(s.marital)) {
    addRequiredError(errors, 'personal', 'marital');
  }

  if (needsSponsor) {
    if (!isFilledField(s.sponsorType)) {
      addRequiredError(errors, 'personal', 'sponsorType');
    }

    if (!isFilledField(s.sponsorEmployerName)) {
      addRequiredError(errors, 'personal', 'sponsorEmployerName');
    }

    if (!isFilledField(s.sponsorEmployerNumber)) {
      addRequiredError(errors, 'personal', 'sponsorEmployerNumber');
    }

    if (isIndividualSponsor && !isFilledField(s.sponsorQidExpiry)) {
      addRequiredError(errors, 'personal', 'sponsorQidExpiry');
    }

    if (!isFilledField(s.sponsorCardName)) {
      addRequiredError(errors, 'personal', 'sponsorCardName');
    }

    // sponsorEmployerNumber validation
    const sponsorType = s.sponsorType?.backendName;
    const sponsorNo = (s.sponsorEmployerNumber ?? '').trim();
    if (sponsorType) {
      if (!sponsorNo) {
        addRequiredError(errors, 'personal', 'sponsorEmployerNumber');
      } else if (!/^\d+$/.test(sponsorNo)) {
        errors.push({ field: 'sponsorEmployerNumber', i18nKey: 'wizard.personal.sponsorEmployerNumber.digitsOnly' });
      } else {
        const requiredLen = sponsorType === SponsorType.Company ? 8 : 11;
        if (sponsorNo.length !== requiredLen) {
          errors.push({
            field: 'sponsorEmployerNumber',
            i18nKey:
              sponsorType === SponsorType.Company
                ? 'wizard.personal.sponsor.company.numberInvalid8'
                : 'wizard.personal.sponsor.individual.qidInvalid11'
          });
        }
      }
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== CONTACT STEP ========== */
function validateContactStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  const backendType = candidateTypeFromState(s);
  const isResident = candidateTypeIsResident(backendType, s.provider);
  const phoneE164 = s.phone?.e164Number ?? '';
  const isQatarPhone = phoneE164.startsWith('+974');

  if (!isFilledField(s.country)) {
    addRequiredError(errors, 'contact', 'country');
  }
  if (!isFilledField(s.interviewPlace)) {
    addRequiredError(errors, 'contact', 'interviewPlace');
  }

  if (!s.phone) {
    addRequiredError(errors, 'contact', 'phone');
  } else {
    // Qatar only: require verification
    if (isQatarPhone && !s.phoneVerified) {
      addRequiredError(errors, 'contact', 'phoneVerified');
    }
  }

  if (!isFilledField(s.email)) {
    addRequiredError(errors, 'contact', 'email');
  }

  if (!s.emailVerified) {
    addRequiredError(errors, 'contact', 'emailVerified');
  }

  if (!isResident) {
    if (!isFilledField(s.address)) {
      addRequiredError(errors, 'contact', 'address');
    }
  } else {
    if (!isFilledField(s.naZone)) {
      addRequiredError(errors, 'contact', 'naZone');
    }

    if (!isFilledField(s.naStreet)) {
      addRequiredError(errors, 'contact', 'naStreet');
    }

    if (!isFilledField(s.naBuilding)) {
      addRequiredError(errors, 'contact', 'naBuilding');
    }

    if (!isFilledField(s.naFileName)) {
      errors.push({
        field: 'naFileName',
        i18nKey: VALIDATION_KEYS.contact.naFile || 'wizard.profile.contact.naFileName.required',
      });
    }
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== DEGREES / EXPERIENCE / SKILLS / ATTACHMENTS ========== */

function validateDegreesStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  const hasDegrees = Array.isArray(s.degrees) && s.degrees.length > 0;
  const today = startOfToday();
  const dobYear = parseDate(s.dob)?.getFullYear() ?? null;

  if (!hasDegrees) {
    errors.push({
      field: 'degrees',
      i18nKey: 'wizard.profile.degrees.atLeastOne.required',
    });
  }

  if (hasDuplicateKeys(s.degrees, degreeDuplicateKey)) {
    errors.push({
      field: 'degrees',
      i18nKey: 'wizard.validation.duplicateTitle',
    });
  }

  s.degrees?.forEach((degree, index) => {
    if ((!degree.file || !degree.fileName) && !degree.attachmentId) {
      errors.push({
        field: `degrees[${index}].certificate`,
        i18nKey: 'wizard.profile.degrees.certificate.required',
      });
    }

    if (degree?.gradYear) {
      const gradYear = Number(degree.gradYear);
      if (Number.isFinite(gradYear)) {
        if (gradYear > today.getFullYear()) {
          errors.push({
            field: `degrees[${index}].gradYear`,
            i18nKey: 'wizard.profile.degrees.gradYear.future',
          });
        }

        if (dobYear && gradYear < dobYear) {
          errors.push({
            field: `degrees[${index}].gradYear`,
            i18nKey: 'wizard.profile.degrees.gradYear.beforeDob',
          });
        }
      }
    }
  });

  return { valid: errors.length === 0, errors };
}

function validateExperienceStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  const today = startOfToday();
  const graduationDates = new Map<string, Date>();
  s.degrees?.forEach(d => {
    if (d.id && Number.isFinite(d.gradYear)) {
      graduationDates.set(d.id, new Date(d.gradYear, 0, 1));
    }
  });
  s.experiences?.forEach((experience, index) => {

    if ((!experience.file || !experience.fileName) && !experience.attachmentId) {
      errors.push({
        field: `experiences[${index}].attachment`,
        i18nKey: 'wizard.profile.experience.attachment.required',
      });
    }

    const startDate = parseDate(experience?.from);
    const endDate = parseDate(experience?.to);

    if (startDate && startDate.getTime() > today.getTime()) {
      errors.push({
        field: `experiences[${index}].from`,
        i18nKey: 'wizard.profile.experience.futureDate',
      });
    }

    if (endDate && endDate.getTime() > today.getTime()) {
      errors.push({
        field: `experiences[${index}].to`,
        i18nKey: 'wizard.profile.experience.futureDate',
      });
    }

    if (startDate && endDate && startDate.getTime() > endDate.getTime()) {
      errors.push({
        field: `experiences[${index}].to`,
        i18nKey: 'wizard.profile.experience.invalidRange',
      });
    }

    if (experience.qualificationId && startDate) {
      const gradDate = graduationDates.get(experience.qualificationId as string);
      if (gradDate && startDate.getTime() <= gradDate.getTime()) {
        errors.push({
          field: `experiences[${index}].from`,
          i18nKey: 'wizard.profile.experience.beforeLinkedGraduation',
        });
      }
    }
  });
  if (hasDuplicateKeys(s.experiences, experienceDuplicateKey)) {
    errors.push({
      field: 'experiences',
      i18nKey: 'wizard.validation.duplicateTitle',
    });
  }

  s.courses?.forEach((course, index) => {
    if ((!course.file || !course.fileName) && !course.attachmentId) {
      errors.push({
        field: `courses[${index}].attachment`,
        i18nKey: 'wizard.profile.courses.attachment.required',
      });
    }

    const startDate = parseDate(course?.from);
    const endDate = parseDate(course?.to);

    if ((startDate && startDate.getTime() > today.getTime()) || (endDate && endDate.getTime() > today.getTime())) {
      errors.push({
        field: `courses[${index}].from`,
        i18nKey: 'wizard.profile.courses.futureDate',
      });
    }
  });
  if (hasDuplicateKeys(s.courses, courseDuplicateKey)) {
    errors.push({
      field: 'courses',
      i18nKey: 'wizard.validation.duplicateTitle',
    });
  }

  return { valid: errors.length === 0, errors };
}

function validateAchievementsStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  s.achievements?.forEach((achievement, index) => {
    if (!achievement?.achievementType) {
      errors.push({ field: `achievements[${index}].achievementType`, i18nKey: 'wizard.profile.achievements.type.required' });
    }
    if ((!achievement.file || !achievement.fileName) && !achievement.attachmentId) {
      errors.push({
        field: `achievements[${index}].attachment`,
        i18nKey: 'wizard.profile.achievements.attachment.required',
      });
    }

    if (isCertificateType(achievement.achievementType) && (achievement.relatedToSpecialization === null || achievement.relatedToSpecialization === undefined)) {
      errors.push({
        field: `achievements[${index}].relatedToSpecialization`,
        i18nKey: 'wizard.profile.achievements.specialization.required',
      });
    }
  });
  if (hasDuplicateKeys(s.achievements, achievementDuplicateKey)) {
    errors.push({
      field: 'achievements',
      i18nKey: 'wizard.validation.duplicateTitle',
    });
  }

  return { valid: errors.length === 0, errors };
}

function validateSkillsStep(s: ProfileState): StepValidationResult {
  return { valid: true, errors: [] };
}

function validateLanguagesStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  const hasLanguages = Array.isArray(s.languages) && s.languages.length > 0;

  if (!hasLanguages) {
    errors.push({
      field: 'languages',
      i18nKey: 'wizard.profile.languages.required',
    });
  }
  s.languages?.forEach((lang, index) => {
    if (!isFilledField(lang?.speakingLevelId ?? lang?.speakingLevel?.id)) {
      errors.push({
        field: `languages[${index}].speakingLevelId`,
        i18nKey: 'wizard.profile.languages.speaking.required',
      });
    }

    if (!isFilledField(lang?.writingLevelId ?? lang?.writingLevel?.id)) {
      errors.push({
        field: `languages[${index}].writingLevelId`,
        i18nKey: 'wizard.profile.languages.writing.required',
      });
    }

    if (!isFilledField(lang?.readingLevelId ?? lang?.readingLevel?.id)) {
      errors.push({
        field: `languages[${index}].readingLevelId`,
        i18nKey: 'wizard.profile.languages.reading.required',
      });
    }
  });
  if (errors.length) {
    return { valid: false, errors };
  }
  return { valid: true, errors: [] };
}

function validateAttachmentsStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  if (hasDuplicateKeys(s.attachments, attachment => normalizeTitle(attachment.title))) {
    errors.push({
      field: 'attachments',
      i18nKey: 'wizard.validation.duplicateTitle',
    });
  }

  s.attachments?.forEach((attachment, index) => {
    if (!isFilledField(attachment?.title)) {
      errors.push({
        field: `attachments[${index}].fileName`,
        i18nKey: 'wizard.profile.attachments.fileName.required',
      });
    }

    if (!(attachment?.file || attachment?.attachmentId)) {
      errors.push({
        field: `attachments[${index}].file`,
        i18nKey: 'wizard.profile.attachments.file.required',
      });
    }
  });
  return { valid: errors.length === 0, errors };
}

/* ========== MAIN SIGNAL ========== */

export function createStepValiditySignal(
  stateSignal: Signal<ProfileState>
) {
  return computed<StepValidityResult>(() => {
    const s = stateSignal();

    return {
      basic:       validateBasicStep(s),
      personal:    validatePersonalStep(s),
      contact:     validateContactStep(s),
      degrees:     validateDegreesStep(s),
      experience:  validateExperienceStep(s),
      achievements: validateAchievementsStep(s),
      skills:      validateSkillsStep(s),
      languages:   validateLanguagesStep(s),
      attachments: validateAttachmentsStep(s),
    };
  });
}
