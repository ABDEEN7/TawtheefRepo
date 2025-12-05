import { computed, Signal } from '@angular/core';
import { VALIDATION_KEYS } from '../constants/profile-validation-keys';
import {
  FieldError, StepValidationResult,
  StepValidityResult,
} from '../models/profile-validation.model';
import {ProfileState} from '../models/profile-state.model';
import {CandidateType, SponsorType} from '../../../../core/enums/lookups.enum';

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
  return !!type && [CandidateType.ResidentQatar].includes(type);
}

export function candidateTypeNeedsBirthCertificate(type: CandidateType | undefined): boolean {
  return !!type && [CandidateType.SonOfQatariMother].includes(type);
}

export function candidateTypeNeedsMarriageCertificate(type: CandidateType | undefined): boolean {
  return !!type && [CandidateType.WifeOfQatari].includes(type);
}

export function candidateTypeIsResident(type: CandidateType | undefined): boolean {
  if (!type) return false;

  return [
    CandidateType.ResidentQatar,
    CandidateType.Qatari,
    CandidateType.SonOfQatariMother,
    CandidateType.WifeOfQatari
  ].includes(type);
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
  const isResident = candidateTypeIsResident(backendType);

  if (!isFilledField(s.candidateType)) {
    addRequiredError(errors, 'basic', 'candidateType');
  }

  if (!isFilledField(s.targetEntity)) {
    addRequiredError(errors, 'basic', 'targetEntity');
  }

  if (!isResident && !isFilledField(s.office)) {
    addRequiredError(errors, 'basic', 'office');
  }

  if (!isFilledField(s.cvName)) {
    addRequiredError(errors, 'basic', 'cvName');
  }

  if (!isFilledField(s.idName)) {
    addRequiredError(errors, 'basic', 'idName');
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
  const isResident = candidateTypeIsResident(candidateTypeFromState(s));
  const isIndividualSponsor = s.sponsorType?.backendName === SponsorType.Individual;

  if (!isFilledField(s.fullNameAr)) {
    addRequiredError(errors, 'personal', 'fullNameAr');
  }

  if (!isFilledField(s.fullNameEn)) {
    addRequiredError(errors, 'personal', 'fullNameEn');
  }

  if (!isFilledField(s.qid)) {
    addRequiredError(errors, 'personal', 'qid');
  }

  if (isResident && !isFilledField(s.qidExpiry)) {
    addRequiredError(errors, 'personal', 'qidExpiry');
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

  if (s.hasDisability && !isFilledField(s.disabilityDetails)) {
    addRequiredError(errors, 'personal', 'disabilityDetails');
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
  }

  return {
    valid: errors.length === 0,
    errors,
  };
}

/* ========== CONTACT STEP ========== */
function validateContactStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  if (!isFilledField(s.country)) {
    addRequiredError(errors, 'contact', 'country');
  }

  // Phone object (PhoneNumber | null): نتحقق من null فقط
  if (!s.phone) {
    addRequiredError(errors, 'contact', 'phone');
  }

  // توثيق الهاتف: يجب أن يكون true
  if (!s.phoneVerified) {
    addRequiredError(errors, 'contact', 'phoneVerified');
  }

  if (!isFilledField(s.email)) {
    addRequiredError(errors, 'contact', 'email');
  }

  // توثيق البريد: يجب أن يكون true
  if (!s.emailVerified) {
    addRequiredError(errors, 'contact', 'emailVerified');
  }

  const isResident = candidateTypeIsResident(candidateTypeFromState(s));

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
        i18nKey: VALIDATION_KEYS.contact.naFiler || 'wizard.profile.contact.naFileName.required',
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

  if (!hasDegrees) {
    errors.push({
      field: 'degrees',
      i18nKey: 'wizard.profile.degrees.atLeastOne.required',
    });
  }

  s.degrees?.forEach((degree, index) => {
    if (!degree?.certificate || !isFilledField(degree.certificate.resourceName)) {
      errors.push({
        field: `degrees[${index}].certificate`,
        i18nKey: 'wizard.profile.degrees.certificate.required',
      });
    }

    if (!(degree?.file || degree?.attachmentId)) {
      errors.push({
        field: `degrees[${index}].attachment`,
        i18nKey: 'wizard.profile.degrees.attachment.required',
      });
    }
  });

  return { valid: errors.length === 0, errors };
}

function validateExperienceStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];

  const hasExperiences = Array.isArray(s.experiences) && s.experiences.length > 0;
  const hasCourses = Array.isArray(s.courses) && s.courses.length > 0;

  if (!hasExperiences) {
    errors.push({
      field: 'experiences',
      i18nKey: 'wizard.profile.experience.atLeastOne.required',
    });
  }

  if (!hasCourses) {
    errors.push({
      field: 'courses',
      i18nKey: 'wizard.profile.courses.atLeastOne.required',
    });
  }

  s.experiences?.forEach((experience, index) => {
    if (!experience?.attachment || !isFilledField(experience.attachment.resourceName)) {
      errors.push({
        field: `experiences[${index}].attachment`,
        i18nKey: 'wizard.profile.experience.attachment.required',
      });
    }

    if (!(experience?.file || experience?.attachmentId)) {
      errors.push({
        field: `experiences[${index}].file`,
        i18nKey: 'wizard.profile.experience.file.required',
      });
    }
  });

  s.courses?.forEach((course, index) => {
    if (!course?.attachment || !isFilledField(course.attachment.resourceName)) {
      errors.push({
        field: `courses[${index}].attachment`,
        i18nKey: 'wizard.profile.courses.attachment.required',
      });
    }

    if (!(course?.file || course?.attachmentId)) {
      errors.push({
        field: `courses[${index}].file`,
        i18nKey: 'wizard.profile.courses.file.required',
      });
    }
  });

  return { valid: errors.length === 0, errors };
}

function validateSkillsStep(s: ProfileState): StepValidationResult {
  const hasSkills = Array.isArray(s.skills) && s.skills.length > 0;

  if (!hasSkills) {
    return {
      valid: false,
      errors: [
        {
          field: 'skills',
          i18nKey: 'wizard.profile.skills.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

function validateLanguagesStep(s: ProfileState): StepValidationResult {
  const hasLanguages = Array.isArray(s.languages) && s.languages.length > 0;

  if (!hasLanguages) {
    return {
      valid: false,
      errors: [
        {
          field: 'languages',
          i18nKey: 'wizard.profile.languages.required',
        },
      ],
    };
  }

  return { valid: true, errors: [] };
}

function validateAttachmentsStep(s: ProfileState): StepValidationResult {
  const errors: FieldError[] = [];
  const hasAttachments = Array.isArray(s.attachments) && s.attachments.length > 0;

  if (!hasAttachments) {
    errors.push({
      field: 'attachments',
      i18nKey: 'wizard.profile.attachments.atLeastOne.required',
    });
  }

  s.attachments?.forEach((attachment, index) => {
    if (!isFilledField(attachment?.fileName ?? attachment?.name)) {
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
      skills:      validateSkillsStep(s),
      languages:   validateLanguagesStep(s),
      attachments: validateAttachmentsStep(s),
    };
  });
}
